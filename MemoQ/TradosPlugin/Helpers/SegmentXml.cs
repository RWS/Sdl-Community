using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text.RegularExpressions;

using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using System.Globalization;

namespace TradosPlugin
{
    /// <summary>
    /// Converts segments to and from XML; conversion into XML is intentionally naive
    /// Not thread-safe, use on an object-per-thread basis
    /// </summary>
    /// <remarks>
    /// It is essential that the output of Segment2Xml not change in any way; please do not modify.
    /// Preceding and following context stored in TM depends directly on the output of this conversion
    /// </remarks>
    public class SegmentXml
    {
        /// <summary>
        /// Represents one tag on the parsing stack
        /// </summary>
        private class XmlElement
        {
            /// <summary>
            /// Element name
            /// </summary>
            public string Name;
            /// <summary>
            /// The element's attribute-value pairs
            /// </summary>
            public Dictionary<string, string> Attributes;
            public XmlElement(string name, Dictionary<string, string> attributes)
            {
                this.Name = name;
                this.Attributes = attributes;
            }
        }

        /// <summary>
        /// Represents one inline tag during Segment->XML serialization: indexed for pairing
        /// Index of unpaired end empty tags is -1
        /// </summary>
        private class InlineTagIndexItem
        {
            /// <summary>
            /// Opening inline tag
            /// </summary>
            public InlineTag itag;
            /// <summary>
            /// Zero-based index; if not -1, bpt's and ept's "i" attribute is this value + 1
            /// </summary>
            public int index;
            public InlineTagIndexItem(InlineTag itag, int index)
            {
                this.itag = itag;
                this.index = index;
            }
        };

        /// <summary>
        /// Character formatting types, used in CharFormat class, during XML parsing
        /// </summary>
        private enum CharFormatType
        {
            Bold,
            Italics,
            Underline,
            Sub,
            Super,
        }

        /// <summary>
        /// Represents from-to character formatting ranges; used during XML parsing
        /// </summary>
        private class CharFormat
        {
            public int Start;
            public int Length;
            public readonly CharFormatType FormatType;
            public CharFormat(CharFormatType formatType)
            {
                Start = Length = 0;
                FormatType = formatType;
            }
            public CharFormat(int start, int length, CharFormatType formatType)
            {
                Start = start;
                Length = length;
                FormatType = formatType;
            }
        }

        // regexes for tags
        string rxTag = "</?[^<>]+/?>";
        //string rxCloseTag = "</[^<>]+>";
        //string rxEmptyTag = "<[\\s\\S]+/>";
        // attribute: non-space characters = non-space characters, except >
        string rxAttr = "\\S+=[\\S-[\\>]]+";

        /// <summary>
        /// Stringbuilder used in Segment -> XML conversion; retained to avoid realloction
        /// </summary>
        private StringBuilder sb = new StringBuilder();


        /// <summary>
        /// Map from BOLD charformat start position in text to charformat index (for linking with closing tag)
        /// </summary>
        private Dictionary<int, int> cfPosToIdxB = new Dictionary<int, int>();

        /// <summary>
        /// Map from ITALIC charformat start position in text to charformat index (for linking with closing tag)
        /// </summary>
        private Dictionary<int, int> cfPosToIdxI = new Dictionary<int, int>();

        /// <summary>
        /// Map from UNDERLINED charformat start position in text to charformat index (for linking with closing tag)
        /// </summary>
        private Dictionary<int, int> cfPosToIdxU = new Dictionary<int, int>();

        /// <summary>
        /// Map from SUBSCRIPT charformat start position in text to charformat index (for linking with closing tag)
        /// </summary>
        private Dictionary<int, int> cfPosToIdxSub = new Dictionary<int, int>();

        /// <summary>
        /// Map from SUPERSCRIPT charformat start position in text to charformat index (for linking with closing tag)
        /// </summary>
        private Dictionary<int, int> cfPosToIdxSuper = new Dictionary<int, int>();

        /// <summary>
        /// Used when parsing XML: Parsing stack
        /// </summary>
        private List<XmlElement> parseStack = new List<XmlElement>();

        /// <summary>
        /// Used when parsing XML: content of current inline tag (ph, bpt, ept, it)
        /// </summary>
        private StringBuilder inlineText = new StringBuilder();

        /// <summary>
        /// Charformats and inline tags during segment's parse: TMX index to position in CharFormats or Tags array
        /// </summary>
        private Dictionary<int, int> inlineIndices = new Dictionary<int, int>();

        /// <summary>
        /// Charformats assembled during XML parsing (to be flattened out at the end)
        /// </summary>
        private List<CharFormat> charFormats = new List<CharFormat>();

        /// <summary>
        /// The Trados segment that will be created from the xml.
        /// </summary>
        private Segment tradosSegment;

        /// <summary>
        /// To build the string for the Trados TU to be displayed on the UI.
        /// </summary>
        private StringBuilder tradosParagraphBuilder = new StringBuilder();

        /// <summary>
        /// Then index/anchor of the current tag. Opening/closing pairs have the same anchor.
        /// </summary>
        private int currentMaxTagAnchor = 0;

        /// <summary>
        /// The anchors for keeping track of tag indexes.
        /// </summary>
        private List<int> anchorStack = new List<int>();

        /// <summary>
        /// Generate string including seg start and end tags; or empty string. Includes tags from the "ParentParagraph" from the TranslationUnit.
        /// </summary>
        /// <param name="seg">Segment to convert</param>
        /// <returns>XML as string</returns>
        /// <remarks>
        /// It is essential that the output of Segment2Xml not change in any way; please do not modify.
        /// Preceding and following context stored in TM depends directly on the output of this conversion
        /// </remarks>
        public string Segment2Xml(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tradosTU, bool source)
        {
            // --------------------------
            // For TMX compatibility with ourselves (export vs. repair dump):
            // Keep this function in synch cMaintenanceSessionImpl::pPrintTMXSegment in memoQ
            // --------------------------
            if (tradosTU == null) return "";

            Segment s = source ? tradosTU.SourceSegment : tradosTU.TargetSegment;
            if (s == null)
                return "";

            // Clear string builder
            sb.Length = 0;
            // collect inline tags from the segment
            List<InlineTag> tags = createTags(tradosTU, source);
            // Pre-index inline tags
            indexInlineTags(tags);
            // go through the elements and make the segment
            writeSegment(s, tags);
            return sb.ToString();
        }

        /// <summary>
        /// Generate string including seg start and end tags; or empty string. No other tags than present in the segment, and no other information about them.
        /// </summary>
        /// <param name="seg">Segment to convert</param>
        /// <returns>XML as string</returns>
        /// <remarks>
        /// It is essential that the output of Segment2Xml not change in any way; please do not modify.
        /// Preceding and following context stored in TM depends directly on the output of this conversion
        /// </remarks>
        public string Segment2Xml(Segment tradosSegment)
        {
            // --------------------------
            // For TMX compatibility with ourselves (export vs. repair dump):
            // Keep this function in synch cMaintenanceSessionImpl::pPrintTMXSegment in memoQ
            // --------------------------
            if (tradosSegment == null) return "";
            // Clear string builder
            sb.Length = 0;
            // collect inline tags from the segment
            List<InlineTag> tags = createSegmentTags(tradosSegment);
            // Pre-index inline tags
            indexInlineTags(tags);
            // go through the elements and make the segment
            writeSegment(tradosSegment, tags);
            return sb.ToString();
        }

        /// <summary>
        /// Writes the segment xml to the sb stringbuilder.
        /// </summary>
        /// <param name="tradosSegment"></param>
        private void writeSegment(Segment tradosSegment, List<InlineTag> tags)
        {
            // Start
            sb.Append("<seg>");
            int i;
            int tagIx = 0;
            for (i = 0; i < tradosSegment.Elements.Count; i++)
            {
                if (tradosSegment.Elements[i] is Text)
                {
                    string text = (tradosSegment.Elements[i] as Text).Value;
                    foreach (char c in text)
                    {
                        if (c == '<')
                            sb.Append("&lt;");
                        else if (c == '>')
                            sb.Append("&gt;");
                        else if (c == '&')
                            sb.Append("&amp;");
                        else if (c == '\t')
                            sb.Append("<ph>&lt;mq:ch val=&quot;&quot; /&gt;</ph>");
                        else
                            sb.Append(c);
                    }
                }
                else if (tradosSegment.Elements[i] is Tag)
                {
                    writeInlineTag(sb, tags[tagIx]);
                    tagIx++;
                }
                else if (tradosSegment.Elements[i] is Token)
                {
                    // TODO is there anything to do here?
                }

            }
            // Finish segment XML
            sb.Append("</seg>");
            sanitizeToSpace(sb);
        }

        /// <summary>
        /// Pre-index a segment's inline tags
        /// Result stored in indexedItagList, which contains as many items as there are itags
        /// Index is preliminary: CharFormats will interfere with this game later on
        /// </summary>
        /// <param name="s">Segment with itags to index</param>
        private void indexInlineTags(List<InlineTag> tags)
        {
            // Make preliminary indexes conspicuously large: this will help when re-indexing
            int currentIndex = 0;
            for (int i = 0; i != tags.Count; ++i)
            {
                InlineTag it = tags[i];
                // Opening: initially assume unpaired
                // Empty: always unpaired
                // Close: move from back to front and try to find pair
                if (tags[i].TagType == TagType.End)
                {
                    for (int revPos = i - 1; revPos >= 0; --revPos)
                    {
                        if (tags[revPos].TagType == TagType.Start && tags[revPos].Name == it.Name && tags[revPos].Index == -1)
                        {
                            tags[revPos].Index = currentIndex;
                            it.Index = currentIndex;
                            ++currentIndex;
                            break;
                        }
                    }
                }
                // Never happens, but we've made an assumption
                else if (it.TagType != TagType.Start && it.TagType != TagType.LockedContent && it.TagType != TagType.Standalone
                    && it.TagType != TagType.TextPlaceholder && it.TagType != TagType.Undefined && it.TagType != TagType.UnmatchedEnd
                    && it.TagType != TagType.UnmatchedStart)
                {
                    throw new Exception("Unknown tag type: should be an existing type");
                }
            }
        }

        /// <summary>
        /// Write the content of a bpt or it or ph tag: actual "tag" text inside, with attributes
        /// </summary>
        /// <param name="sb">String builder to append to (segment XML)</param>
        /// <param name="it">Inline tag to serialize</param>
        private void writeTagText(StringBuilder sb, InlineTag it)
        {
            sb.Append("&lt;");
            if (it.TagType == TagType.End || it.TagType == TagType.UnmatchedEnd)
                sb.Append("/");

            sb.Append(it.Name);

            if (it.Attributes != null)
            {
                foreach (Attribute attr in it.Attributes)
                {
                    // Writing attribute text: double XML escape
                    var sbSub1 = new StringBuilder();
                    // If attribute is translatable, write empty value, i.e., nothing - downward compatibility

                    sbSub1.Append(" " + attr.Name);
                    sbSub1.Replace("&", "&amp;");
                    sbSub1.Append("=&quot;" + attr.Value + "&quot;");
                    sbSub1.Replace("<", "&lt;"); sbSub1.Replace(">", "&gt;"); sbSub1.Replace("\"", "&quot;"); sbSub1.Replace("'", "&apos;");
                    //sbSub1.Replace("&", "&amp;");
                    sb.Append(sbSub1);
                }
            }

            if (it.TagType == TagType.Standalone || it.TagType == TagType.LockedContent || it.TagType == TagType.TextPlaceholder)
                sb.Append(" /");
            sb.Append("&gt;");
        }

        /// <summary>
        /// Write the content of a bpt or it or ph tag: actual "tag" text inside, with attributes
        /// </summary>
        /// <param name="sb">String builder to append to (segment XML)</param>
        /// <param name="it">Inline tag to serialize</param>
        private void writeTagText(StringBuilder sb, Tag it)
        {
            sb.Append("&lt;");
            if (it.Type == TagType.End)
                sb.Append("/");
            // tags have no name
            // sb.Append(it.Name);
            sb.Append("g");
            // Do attributes, if any
            // avarga del: Trados tags have no attributes, just ID and anchors
            //if (it.Attributes != null)
            //{
            //    foreach (AttrValPair attr in it.Attributes)
            //    {
            sb.Append(" ");
            sb.Append("id");
            sb.Append("=&quot;");
            sb.Append(it.TagID);
            sb.Append("&quot;");
            // write type
            sb.Append(" ");
            sb.Append("type");
            sb.Append("=&quot;");
            sb.Append(it.Type.ToString());
            sb.Append("&quot;");
            // write anchor
            sb.Append(" ");
            sb.Append("anchor");
            sb.Append("=&quot;");
            sb.Append(it.Anchor.ToString());
            sb.Append("&quot;");
            // write alignment anchor           
            sb.Append(" ");
            sb.Append("alignmentanchor");
            sb.Append("=&quot;");
            sb.Append(it.AlignmentAnchor.ToString());
            sb.Append("&quot;");
            // write text equivalent
            if (it.TextEquivalent != null)
            {
                sb.Append(" ");
                sb.Append("textequivalent");
                sb.Append("=&quot;");
                sb.Append(it.TextEquivalent);
                sb.Append("&quot;");
            }

            //// Writing attribute text: double XML escape
            //var sbSub1 = new StringBuilder();
            //// If attribute is translatable, write empty value, i.e., nothing - downward compatibility
            //if (!attr.IsSeparateRowTranslatable)
            //{
            //    sbSub1.Insert(0, attr.ValueString);
            //    sbSub1.Replace("&", "&amp;"); sbSub1.Replace("<", "&lt;"); sbSub1.Replace(">", "&gt;"); sbSub1.Replace("\"", "&quot;"); sbSub1.Replace("'", "&apos;");
            //    sbSub1.Replace("&", "&amp;");
            //    sb.Append(sbSub1);
            //}
            // End writing attribute text
            //sb.Append("&quot;");
            //    }
            //}
            if (it.Type == TagType.Standalone)
                sb.Append(" /");
            sb.Append("&gt;");
        }

        /// <summary>
        /// Invalid characters in tag or attribute name
        /// Guarantees compatibility with our own inline tag resolution automaton
        /// </summary>
        private static char[] invalidNameChars = new char[] { '&', '<', '>', '=', '\'', '"', '/' };

        /// <summary>
        /// Checks the validity of name and attributes
        /// Returns with no effect, or throws
        /// </summary>
        /// <param name="it">Inline tag to check</param>
        private void checkCharacterValidity(InlineTag it)
        {
            if (-1 != it.Name.IndexOfAny(invalidNameChars))
                throw new Exception("Tag ID '" + it.Name + "' contains invalid characters");
            if (it.Attributes == null || it.Attributes.Count == 0) return;
            foreach (Attribute a in it.Attributes)
                if (-1 != a.Name.IndexOfAny(invalidNameChars))
                    throw new Exception("Attribute name '" + a.Name + "' contains invalid characters");
        }

        /// <summary>
        /// Checks the validity of name and attributes
        /// Returns with no effect, or throws
        /// </summary>
        /// <param name="it">Inline tag to check</param>
        private void checkCharacterValidity(Tag it)
        {
            if (-1 != it.TagID.IndexOfAny(invalidNameChars))
                throw new Exception("Tag ID '" + it.TagID + "' contains invalid characters");

        }

        /// <summary>
        /// Write inline tag as XML (escaped)
        /// </summary>
        /// <param name="sb">String builder to append to (segment XML)</param>
        /// <param name="it">Inline tag to serialize</param>
        /// <param name="index">Index for use in bpt/ept tags</param>
        private void writeInlineTag(StringBuilder sb, Tag it, int index)
        {
            // Cry out loud if index has been unadjusted (should never happen)
            if (index > 65535)
                throw new Exception("Attempting to write inline tag with unadjusted index");
            // Check for invalid characters in tag name and attributes
            checkCharacterValidity(it);
            // Empty tag: easy
            if (it.Type == TagType.Standalone)
            {

                //if (it.Name == "php" || it.Name == "!--")
                //{
                //    sb.Append("<ph>");
                //    string val = it.GetAttr("val").ValueString;
                //    val = val.Replace("&", "&amp;");
                //    val = val.Replace("<", "&lt;");
                //    val = val.Replace(">", "&gt;");
                //    sb.Append(val);
                //    sb.Append("</ph>");
                //}
                //else
                //{
                sb.Append("<ph>");
                writeTagText(sb, it);
                sb.Append("</ph>");
                //}
            }
            // Closing tag
            else if (it.Type == TagType.End)
            {
                // Index is -1: Unpaired, use <it>
                if (index == -1)
                {
                    sb.Append("<it pos='end'>");
                    writeTagText(sb, it);
                    sb.Append("</it>");
                }
                // Index is not -1: Paired tag, use <ept>
                else
                {
                    sb.Append("<ept i='");
                    sb.Append((index + 1).ToString());
                    sb.Append("'>");
                    writeTagText(sb, it);
                    sb.Append("</ept>");
                }
            }
            // Opening tag
            else if (it.Type == TagType.Start)
            {
                // Index is -1: Unpaired, use <it>
                if (index == -1)
                {
                    sb.Append("<it pos='begin'>");
                    writeTagText(sb, it);
                    sb.Append("</it>");
                }
                // Index is not -1: Paired tag, use <bpt>
                else
                {
                    sb.Append("<bpt i='");
                    sb.Append((index + 1).ToString());
                    sb.Append("'>");
                    writeTagText(sb, it);
                    sb.Append("</bpt>");
                }
            }
        }

        /// <summary>
        /// Write inline tag as XML (escaped)
        /// </summary>
        /// <param name="sb">String builder to append to (segment XML)</param>
        /// <param name="it">Inline tag to serialize</param>
        /// <param name="index">Index for use in bpt/ept tags</param>
        private void writeInlineTag(StringBuilder sb, InlineTag it)
        {
            // Check for invalid characters in tag name and attributes
            checkCharacterValidity(it);
            // Empty tag: easy
            if (it.TagType == TagType.Standalone || it.TagType == TagType.LockedContent || it.TagType == TagType.TextPlaceholder)
            {
                // placeholder

                if (it.Name == "php" || it.Name == "!--")
                {
                    sb.Append("<ph>");
                    Attribute attr = it.Attributes.Find(a => a.Name == "val");
                    if (attr != null)
                    {
                        string val = attr.Value;
                        val = val.Replace("&", "&amp;");
                        val = val.Replace("<", "&lt;");
                        val = val.Replace(">", "&gt;");
                        sb.Append(val);
                    }
                    sb.Append("</ph>");
                }
                else
                {
                    sb.Append("<ph>");
                    writeTagText(sb, it);
                    sb.Append("</ph>");
                }
            }
            // Closing tag
            else if (it.TagType == TagType.End)
            {
                // Paired tag, use <ept>
                sb.Append("<ept i='");
                sb.Append((it.Index + 1).ToString());
                sb.Append("'>");
                writeTagText(sb, it);
                sb.Append("</ept>");

            }
            // unpaired: use "it"
            else if (it.TagType == TagType.UnmatchedEnd)
            {
                sb.Append("<it pos='end'>");
                writeTagText(sb, it);
                sb.Append("</it>");
            }
            // Opening tag
            else if (it.TagType == TagType.Start)
            {

                // Index is not -1: Paired tag, use <bpt>              
                sb.Append("<bpt i='");
                sb.Append((it.Index + 1).ToString());
                sb.Append("'>");
                writeTagText(sb, it);
                sb.Append("</bpt>");
            }
            else if (it.TagType == TagType.UnmatchedStart)
            {
                // Index is -1: Unpaired, use <it>
                if (it.Index == -1)
                {
                    sb.Append("<it pos='begin'>");
                    writeTagText(sb, it);
                    sb.Append("</it>");
                }
            }
        }

        /// <summary>
        /// Writes a tag that has been collected from a Trados TranslationUnit.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="tag"></param>
        private void writeInlineTagSimple(StringBuilder sb, InlineTag tag)
        {
            // Check for invalid characters in tag name and attributes
            checkCharacterValidity(tag);
            // now I'll treat all the tags equally: they will just be tags with all the attributes

            // then TODO: somehow retrieve BIUSS formatting from at least doc and html, and create bpt/ept from them
            // TODO also treat text placeholders differently
            sb.Append("<");
            if (tag.TagType == TagType.End || tag.TagType == TagType.UnmatchedEnd) sb.Append("/");
            sb.Append(tag.Name);
            foreach (Attribute a in tag.Attributes)
            {
                sb.Append(" " + a.Name + "=\"" + a.Value);
            }
            if (tag.TagType == TagType.Standalone) sb.Append(" /");
            sb.Append(">");



        }


        /// <summary>
        /// Replace invalid Unicode/XML characters with spaces
        /// </summary>
        /// <param name="str">Input string</param>
        /// <returns>Sanitized strings</returns>
        private string sanitizeToSpace(string str)
        {
            char[] sanitizedStr = new char[str.Length];
            for (int i = 0; i < str.Length; ++i)
            {
                int ix = i;
                if (TMProvider.XmlSanitizingStreamReader.IsLegalXmlChar(false, str[ix]))
                    sanitizedStr[ix] = str[ix];
                else
                    sanitizedStr[ix] = ' ';
            }
            return new string(sanitizedStr);
        }

        /// <summary>
        /// Replace invalid Unicode/XML characters with spaces
        /// </summary>
        /// <param name="sb">StringBuilder object to sanitize</param>
        private void sanitizeToSpace(StringBuilder sb)
        {

            for (int i = 0; i < sb.Length; ++i)
            {
                int ix = i;
                if (!TMProvider.XmlSanitizingStreamReader.IsLegalXmlChar(false, sb[ix]))
                    sb[ix] = ' ';
            }
        }

        /// <summary>
        /// Parse XML and create segment. Throws exception of type Exception on parse error.
        /// </summary>
        /// <param name="xmlStr">XML as string</param>
        /// <returns>Segment</returns>
        public Segment Xml2Segment(string xmlStr, out string segmentParagraph)
        {
            currentMaxTagAnchor = 0;
            anchorStack.Clear();
            segmentParagraph = "";
            if (String.IsNullOrEmpty(xmlStr))
                return null;
            // TO DO: build the trados segment from the xml manually
            string sanitizedXmlStr = sanitizeToSpace(xmlStr);
            StringReader sr = new StringReader(sanitizedXmlStr);
            XmlTextReader reader = new XmlTextReader(sr);
            this.parseStack.Clear();
            this.charFormats.Clear();
            this.inlineIndices.Clear();
            // trados segment
            tradosSegment = new Segment();
            tradosParagraphBuilder.Clear();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        string name = reader.Name;
                        List<Attribute> attributes = new List<Attribute>();
                        if (reader.HasAttributes)
                        {
                            for (int i = 0; i < reader.AttributeCount; i++)
                            {
                                reader.MoveToAttribute(i);
                                attributes.Add(new Attribute(reader.Name, reader.Value));
                            }
                            // TO-DO: verify
                            // Discovered by DA
                            //reader.MoveToElement();
                        }
                        startElement(name, attributes);
                        if (reader.IsEmptyElement)
                        {
                            endElement(reader.Name);
                        }
                        break;
                    case XmlNodeType.EndElement:
                        endElement(reader.Name);
                        break;
                    case XmlNodeType.Text:
                        characters(reader.Value);
                        break;
                    case XmlNodeType.Whitespace:
                        characters(reader.Value);
                        break;
                }
            }
            segmentParagraph = tradosParagraphBuilder.ToString();
            // if there are any that are not closed, change them to unpaired start
            if (anchorStack.Count != 0)
            {
                foreach (int index in anchorStack)
                {
                    // find tag in segment
                    Tag t = tradosSegment.Elements.Find(el => el is Tag && (el as Tag).Anchor == index) as Tag;
                    if (t != null) t.Type = TagType.UnmatchedStart;
                }
            }
            tradosSegment.Tokens = Tokenizer.TokenizeSegment(tradosSegment);
            return tradosSegment;
        }

        /// <summary>
        /// Adds a starting tag to the parse stack.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attributes"></param>
        private void startElement(string name, List<Attribute> attributes)
        {
            if (name == "seg")
            {
                // NOP
            }
            else if (this.parseStack.Count == 0)
                throw new Exception("Invalid TMX data");

            this.inlineText.Length = 0;

            if (name == "bpt" || name == "ept" || name == "ph" || name == "it")
            {
                this.inlineText.Length = 0;
            }

            Dictionary<string, string> attrDict = attributes.ToDictionary(a => a.Name, a => a.Value);
            this.parseStack.Add(new XmlElement(name, attrDict));
        }

        private void endElement(string name)
        {
            if (this.parseStack.Count == 0)
                throw new Exception("Invalid TMX data");
            if (this.parseStack[this.parseStack.Count - 1].Name != name)
                throw new Exception("Invalid TMX data");
            //XmlElement myOpenElem = this.parseStack[this.parseStack.Count - 1];
            string paragraphText;
            // </seg>: do some final cleanup
            if (name == "seg")
            {

            }
            // </ph>: Inline tag, or {tag}
            else if (name == "ph" || name == "bpt" || name == "ept" || name == "it")
            {
                Tag it = ResolveInlineFromMemoQXML(this.inlineText.ToString(), out paragraphText);
                if (it != null && it.TagID == "mq:ch") tradosSegment.Add(new Text("\t"));
                else if (it != null) tradosSegment.Add(it);
                tradosParagraphBuilder.Append(paragraphText);
            }

            // All other: just ignore
            // .
            // Unwind parse stack
            this.parseStack.RemoveAt(this.parseStack.Count - 1);
        }

        private void characters(string text)
        {
            string stackTop = this.parseStack[this.parseStack.Count - 1].Name;
            if (stackTop == "seg")
            {
                tradosSegment.Add(new Text(text));
                tradosParagraphBuilder.Append(text);
            }
            else if (stackTop == "bpt" || stackTop == "ept" || stackTop == "ph" || stackTop == "it")
                this.inlineText.Append(text);
        }

        /// <summary>
        /// Inline tag resolution automaton state results
        /// </summary>
        private enum StateResult
        {
            Ready,
            EndSuccess,
            EndError
        }

        /// <summary>
        /// Non-immutable name-value pair, used during inline tag construction
        /// </summary>
        private class SoftNameValuePair
        {
            public string Name;
            public string Value;
        }

        /// <summary>
        /// State function delegate: where to go next
        /// </summary>
        /// <param name="c">Character to parse</param>
        /// <returns>State of automaton after eating character</returns>
        private delegate StateResult StateFunction(char c);

        /// <summary>
        /// The current state of the automaton
        /// </summary>
        private StateFunction currentState;

        /// <summary>
        /// Name of current inline tag under construction
        /// </summary>
        private string currentInlineTagName;

        /// <summary>
        /// Type of current inline tag under construction
        /// </summary>
        private TagType currentInlineTagType;

        /// <summary>
        /// Current inline tag under construction is a closing tag
        /// </summary>
        private bool currentTagIsClosing;

        /// <summary>
        /// Current string under construction (whatever it will become)
        /// </summary>
        private StringBuilder currentString = new StringBuilder();

        /// <summary>
        /// Attributes collected for current tag
        /// </summary>
        private List<SoftNameValuePair> currentAttributes = new List<SoftNameValuePair>();

        /// <summary>
        /// Character surrounding current attribute's value: quote or apostrophe
        /// </summary>
        private char currentAttrQuote;



        /// <summary>
        /// Parse string and attempt to resolve an inline tag
        /// </summary>
        /// <param name="inlineText">Text to parse</param>
        /// <returns>Inline tag, or null if failed</returns>
        public Tag ResolveInlineFromMemoQXML(string inlineText, out string paragraphText)
        {
            if (inlineText == "{}")
            {
                paragraphText = "";
                return null;
            }

            // Kick off with state 010, no inline tag
            currentState = state010;
            currentInlineTagName = null;
            currentAttributes.Clear();
            currentTagIsClosing = false;
            string newTagName;
            int newTagAnchor;

            // Go as long as input lasts, or error, or success
            int pos = 0;
            StateResult sr = StateResult.Ready;
            StringBuilder sbPar = new StringBuilder();
            while (pos < inlineText.Length && sr == StateResult.Ready)
            {
                sr = currentState(inlineText[pos]);
                ++pos;
            }
            // Resolved if we've eaten the complete text and state result is EndSuccess
            if (pos == inlineText.Length && sr == StateResult.EndSuccess)
            {
                // keep track of tag numbers
                if (currentInlineTagType != TagType.End && currentInlineTagType != TagType.UnmatchedEnd)
                {
                    currentMaxTagAnchor++;
                    anchorStack.Add(currentMaxTagAnchor);
                }
                if (anchorStack.Count == 0)
                {
                    // invalid tag order
                    // we can't have an end tag -> change type
                    currentInlineTagType = TagType.UnmatchedEnd;
                    currentMaxTagAnchor++;
                    newTagAnchor = currentMaxTagAnchor;
                }
                else
                {
                    newTagAnchor = anchorStack[anchorStack.Count - 1];
                }

                Attribute[] attrs = new Attribute[currentAttributes.Count];
                newTagName = currentInlineTagName;
                for (int i = 0; i != attrs.Length; ++i)
                {
                    attrs[i] = new Attribute(currentAttributes[i].Name, currentAttributes[i].Value);
                }
                Tag newTag = createTagWithMemoQAttributes(attrs, newTagName, currentInlineTagType, newTagAnchor, out paragraphText);
                // keep track of tag numbers
                if (anchorStack.Count > 0 && currentInlineTagType != TagType.Start && currentInlineTagType != TagType.UnmatchedStart)
                {
                    anchorStack.RemoveAt(anchorStack.Count - 1);
                }
                return newTag;
            }
            paragraphText = "";
            return null;
        }

        /// <summary>
        /// Checks character against the list of invalid characters
        /// </summary>
        /// <param name="c">Character to check</param>
        /// <returns>True if invalid, false otherwise</returns>
        private bool isInvalidNameChar(char c)
        {
            foreach (char ivc in invalidNameChars)
                if (ivc == c)
                    return true;
            return false;
        }

        // '<'
        private StateResult state010(char c)
        {
            if (c == '<')
            {
                currentState = state020;
                return StateResult.Ready;
            }
            return StateResult.EndError;

        }

        // '< '
        // '<i'
        // '</'
        private StateResult state020(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state020;
                return StateResult.Ready;
            }
            if (c == '/')
            {
                currentString.Length = 0;
                currentState = state030;
                currentTagIsClosing = true;
                return StateResult.Ready;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Length = 0;
            currentString.Append(c);
            currentState = state100;
            return StateResult.Ready;
        }

        // '</ '
        // '</i'
        private StateResult state030(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state030;
                return StateResult.Ready;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Append(c);
            currentState = state040;
            return StateResult.Ready;
        }

        // '</img>'
        // '</img '
        private StateResult state040(char c)
        {
            if (c == '>')
            {
                currentInlineTagName = currentString.ToString();
                currentInlineTagType = TagType.End;
                return StateResult.EndSuccess;
            }
            if (Char.IsWhiteSpace(c))
            {
                currentInlineTagName = currentString.ToString();
                currentInlineTagType = TagType.End;
                currentState = state050;
                return StateResult.Ready;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Append(c);
            currentState = state040;
            return StateResult.Ready;
        }

        // '</img >'
        // '</img a' -- closing tags can also have attributes for us; fold onto other part of machine
        private StateResult state050(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state050;
                return StateResult.Ready;
            }
            if (c == '>')
            {
                currentInlineTagName = currentString.ToString();
                currentInlineTagType = TagType.End;
                return StateResult.EndSuccess;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            // Attributes may come
            currentString.Length = 0;
            currentString.Append(c);
            currentState = state200;
            return StateResult.Ready;
        }

        // '<img '
        // '<img/'
        // '<img>'
        private StateResult state100(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentInlineTagName = currentString.ToString();
                currentInlineTagType = TagType.Start;
                currentState = state110;
                return StateResult.Ready;
            }
            if (c == '/')
            {
                currentInlineTagName = currentString.ToString();
                currentInlineTagType = TagType.Standalone;
                currentState = state120;
                return StateResult.Ready;
            }
            if (c == '>')
            {
                currentInlineTagName = currentString.ToString();
                currentInlineTagType = TagType.End;
                return StateResult.EndSuccess;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Append(c);
            currentState = state100;
            return StateResult.Ready;
        }

        // '<img >'
        // '<img a'
        // '<img /'
        private StateResult state110(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state110;
                return StateResult.Ready;
            }
            if (c == '>')
                return StateResult.EndSuccess;
            if (c == '/')
            {
                currentInlineTagType = TagType.Standalone;
                currentState = state120;
                return StateResult.Ready;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Length = 0;
            currentString.Append(c);
            currentState = state200;
            return StateResult.Ready;
        }

        // '<img />'
        // '<img / >'
        private StateResult state120(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state120;
                return StateResult.Ready;
            }
            if (c == '>')
                return StateResult.EndSuccess;
            return StateResult.EndError;
        }

        // '<img attr '
        // '<img attr='
        // '</img attr'
        private StateResult state200(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                SoftNameValuePair newAttr = new SoftNameValuePair();
                newAttr.Name = currentString.ToString();
                newAttr.Value = null;
                currentAttributes.Add(newAttr);
                currentState = state210;
                return StateResult.Ready;
            }
            if (c == '=')
            {
                SoftNameValuePair newAttr = new SoftNameValuePair();
                newAttr.Name = currentString.ToString();
                currentAttributes.Add(newAttr);
                currentState = state220;
                return StateResult.Ready;
            }
            // End of empty tag, NULL attrval
            if (c == '/')
            {
                // Must not be here if tag is a closing tag
                if (currentTagIsClosing) return StateResult.EndError;
                // Store attribute
                SoftNameValuePair newAttr = new SoftNameValuePair();
                newAttr.Name = currentString.ToString();
                newAttr.Value = null;
                currentAttributes.Add(newAttr);
                // Empty tag, last attribute has NULL value
                currentInlineTagType = TagType.Standalone;
                currentState = state250;
                return StateResult.Ready;
            }
            // End of non-empty tag, NULL attrval
            if (c == '>')
            {
                // Store attribute
                SoftNameValuePair newAttr = new SoftNameValuePair();
                newAttr.Name = currentString.ToString();
                newAttr.Value = null;
                currentAttributes.Add(newAttr);
                // Tag over
                return StateResult.EndSuccess;
            }
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Append(c);
            currentState = state200;
            return StateResult.Ready;
        }

        // '<img attr ='
        private StateResult state210(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state210;
                return StateResult.Ready;
            }
            if (c == '=')
            {
                currentState = state220;
                return StateResult.Ready;
            }
            // End of empty tag, NULL attrval
            if (c == '/')
            {
                // Must not be here if tag is a closing tag
                if (currentTagIsClosing) return StateResult.EndError;
                // Empty tag, last attribute has NULL value
                currentInlineTagType = TagType.Standalone;
                currentState = state250;
                return StateResult.Ready;
            }
            // End of non-empty tag, NULL attrval
            if (c == '>')
            {
                return StateResult.EndSuccess;
            }
            // Name of next attribute starts
            if (!isInvalidNameChar(c))
            {
                currentString.Length = 0;
                currentString.Append(c);
                currentState = state200;
                return StateResult.Ready;
            }
            return StateResult.EndError;
        }

        // '<img attr="'
        // '<img attr= "'
        private StateResult state220(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state220;
                return StateResult.Ready;
            }
            if (c == '\'' || c == '"')
            {
                currentAttrQuote = c;
                currentString.Length = 0;
                currentState = state230;
                return StateResult.Ready;
            }
            return StateResult.EndError;
        }

        // '<img attr="value"'
        private StateResult state230(char c)
        {
            if (c == currentAttrQuote)
            {
                // Unescape attribute value
                currentString.Replace("&lt;", "<");
                currentString.Replace("&gt;", ">");
                currentString.Replace("&apos;", "'");
                currentString.Replace("&quot;", "\"");
                currentString.Replace("&amp;", "&");
                currentAttributes[currentAttributes.Count - 1].Value = currentString.ToString();
                currentState = state240;
                return StateResult.Ready;
            }
            currentString.Append(c);
            currentState = state230;
            return StateResult.Ready;
        }

        // '<img attr="value" '
        // '<img attr="value"/'
        // '<img attr="value">'
        private StateResult state240(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state260;
                return StateResult.Ready;
            }
            if (c == '/')
            {
                // Must not be here if it was a close tag
                if (currentTagIsClosing)
                    return StateResult.EndError;
                // Empty tag
                currentInlineTagType = TagType.Standalone;
                currentState = state250;
                return StateResult.Ready;
            }
            if (c == '>')
                return StateResult.EndSuccess;
            return StateResult.EndError;
        }

        // '<img attr="value"/>'
        // '<img attr="value"/ '
        // '<img attr="value"/ >'
        private StateResult state250(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state250;
                return StateResult.Ready;
            }
            if (c == '>')
                return StateResult.EndSuccess;
            return StateResult.EndError;
        }

        // '<img attr="value" /'
        // '<img attr="value" >'
        // '<img attr="value" a'
        private StateResult state260(char c)
        {
            if (Char.IsWhiteSpace(c))
            {
                currentState = state260;
                return StateResult.Ready;
            }
            if (c == '/')
            {
                // Must not be here if it was a close tag
                if (currentTagIsClosing)
                    return StateResult.EndError;
                // Empty tag
                currentInlineTagType = TagType.Standalone;
                currentState = state250;
                return StateResult.Ready;
            }
            if (c == '>')
                return StateResult.EndSuccess;
            if (isInvalidNameChar(c))
                return StateResult.EndError;
            currentString.Length = 0;
            currentString.Append(c);
            currentState = state200;
            return StateResult.Ready;
        }

        private Tag createTagWithMemoQAttributes(Attribute[] attributes, string tagName, TagType tagType, int anchor, out string paragraphText)
        {
            paragraphText = "";
            //if (attributes == null || attributes.Length == 0) return null;
            // Tag has: TagID, Anchor, AlignmentAnchor, TextEquivalent
            // all the rest go to the paragraph text
            Tag newTag = new Tag(tagType, tagName, anchor);
            if (attributes == null) attributes = new Attribute[0];
            // paragraph text - we need the tag type
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            if (newTag.Type == TagType.End || newTag.Type == TagType.UnmatchedEnd) sb.Append("/");

            // name
            sb.Append(tagName);

            foreach (Attribute a in attributes)
            {
                sb.Append(" " + a.Name + "=\"" + a.Value + "\"");
            }
            if (newTag.Type == TagType.Standalone || newTag.Type == TagType.LockedContent || newTag.Type == TagType.TextPlaceholder)
                sb.Append(" /");
            sb.Append(">");
            paragraphText = sb.ToString();
            return newTag;
        }

        private Tag createTagFromTradosAttributes(Attribute[] attributes, out string paragraphText)
        {
            paragraphText = "";
            if (attributes == null || attributes.Length == 0) return null;
            // Tag has: TagID, Anchor, AlignmentAnchor, TextEquivalent
            // all the rest go to the paragraph text
            Tag newTag = new Tag();

            foreach (Attribute a in attributes)
            {
                if (a.Name == "TagID") newTag.TagID = a.Value.TrimStart('_');
                else if (a.Name == "Anchor") newTag.Anchor = Int32.Parse(a.Value);
                else if (a.Name == "AlignmentAnchor") newTag.AlignmentAnchor = Int32.Parse(a.Value);
                else if (a.Name == "TextEquivalent") newTag.TextEquivalent = a.Value;
                else if (a.Name == "TagType")
                {
                    TagType t;
                    if (Enum.TryParse(a.Value, out t))
                    {
                        newTag.Type = t;
                    }

                }
            }
            // paragraph text - we need the tag type
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            if (newTag.Type == TagType.End || newTag.Type == TagType.UnmatchedEnd) sb.Append("/");
            Attribute nameAttr = attributes.First(a => a.Name == "Name");
            // name
            if (nameAttr != null) sb.Append(nameAttr.Value);
            // if there's no name, use the ID (but it's probably a number)
            else sb.Append("_" + newTag.TagID);

            foreach (Attribute a in attributes)
            {
                if (a.Name == "TagID" || a.Name == "Anchor" || a.Name == "AlignmentAnchor" || a.Name == "TextEquivalent" || a.Name == "TagType")
                    continue;

                sb.Append(" " + a.Name + "=\"" + a.Value + "\"");
            }
            if (newTag.Type == TagType.Standalone || newTag.Type == TagType.LockedContent || newTag.Type == TagType.TextPlaceholder)
                sb.Append(" /");
            sb.Append(">");
            paragraphText = sb.ToString();
            return newTag;
        }

        private List<InlineTag> createTags(Sdl.LanguagePlatform.TranslationMemory.TranslationUnit tradosTU, bool source)
        {
            // the Trados segment doesn't contain information about tags, just an ID, anchor and type
            // the bilingual segment content contains the information that is used to display the tags on the UI
            // the way we collect tags:
            // collect the tags from the segment's segmentelements
            // parse this bilingual content, and add the attributes to the already collected tags in the order they are in
            // the 2 kinds of tags (segment element and in the bilingual content) are not connected by any information ->
            // we can only rely on their order
            Segment tradosSegment = source ? tradosTU.SourceSegment : tradosTU.TargetSegment;
            // collect tags from segment
            List<InlineTag> segmentTags = createSegmentTags(tradosSegment);
            // if there are no segment tags, we can't do much
            // TODO - think it over, is there anything we could do? still add these tags? but which ones?
            if (segmentTags.Count == 0) return segmentTags;
            if (tradosTU.DocumentSegmentPair == null) return segmentTags;

            // parse the content 
            // the text content might contain more (??? less) tags than the segment itself (eg. at the beginning, end)
            List<InlineTag> textTags = new List<InlineTag>();

            ISegment segText = tradosTU.DocumentSegmentPair.Source;
            string xml = segText.ToString();
            InlineTag newTag;
            Regex tagFinder = new Regex(rxTag);
            MatchCollection tagMatches = tagFinder.Matches(xml);
            foreach (System.Text.RegularExpressions.Match t in tagMatches)
            {
                if (t.Value.Length < 3) continue;
                TagType ttype;
                if (t.Value.StartsWith("</")) ttype = TagType.End;
                else if (t.Value.EndsWith("/>")) ttype = TagType.Standalone;
                else ttype = TagType.Start;
                // get name
                string name;
                int s = ttype == TagType.End ? 2 : 1, e;
                while (t.Value.Length > s && t.Value[s] == ' ') s++;
                e = s;
                while (t.Value.Length - 1 > e && t.Value[e] != ' ' && t.Value[e] != '/' && t.Value[e] != '>') e++;
                if (e == s) continue;
                name = t.Value.Substring(s, e - s);
                if (name.Contains('=')) continue;

                newTag = new InlineTag(name, ttype, new List<Attribute>());
                // end tags have no attributes
                if (ttype == TagType.End)
                {
                    textTags.Add(newTag);
                    continue;
                }
                // attributes
                Regex attrFinder = new Regex(rxAttr);
                MatchCollection attrMatches = attrFinder.Matches(t.Value);
                foreach (System.Text.RegularExpressions.Match a in attrMatches)
                {
                    string[] parts = a.Value.Split('=');
                    if (parts.Length != 2) continue;
                    newTag.Attributes.Add(new Attribute(parts[0], parts[1].Trim('"')));
                }
                textTags.Add(newTag);
            }

            // add text tag info to segment tags, if possible
            // try to normalize the number: remove extra ones from the beginning and end of text tags
            int diff = textTags.Count - segmentTags.Count;
            // if there are more segment tags, then I don't think we can do anything
            if (segmentTags.Count == 0 || diff < 0) return segmentTags;
            if (diff > 0)
            {
                // middle of the lists
                int mT = textTags.Count / 2;
                int mS = segmentTags.Count / 2;
                // index
                int tx, sx;
                // the tag types don't match
                // start and empty tags match
                if (textTags[mT].TagType == TagType.Start && segmentTags[mS].TagType != TagType.Start && segmentTags[mS].TagType != TagType.UnmatchedStart
                    && segmentTags[mS].TagType != TagType.LockedContent && segmentTags[mS].TagType != TagType.Standalone && segmentTags[mS].TagType != TagType.TextPlaceholder)
                    return segmentTags;
                if (textTags[mT].TagType == TagType.End && segmentTags[mS].TagType != TagType.End && segmentTags[mS].TagType != TagType.UnmatchedEnd)
                    return segmentTags;

                // now move from the middle to left and right and add the text attributes to the segment tags
                for (sx = mS, tx = mT; sx >= 0; sx--, tx--)
                {
                    segmentTags[sx].Attributes.AddRange(textTags[tx].Attributes);
                }
                for (sx = mS + 1, tx = mT + 1; sx < segmentTags.Count; sx++, tx++)
                {
                    segmentTags[sx].Attributes.AddRange(textTags[tx].Attributes);
                }
            }
            return segmentTags;

        }

        private List<InlineTag> createSegmentTags(Segment tradosSegment)
        {
            List<InlineTag> segmentTags = new List<InlineTag>();
            // collect tags from segment
            foreach (SegmentElement se in tradosSegment.Elements)
            {
                if (!(se is Tag)) continue;
                Tag t = se as Tag;
                List<Attribute> attrs = new List<Attribute>();
                attrs.Add(new Attribute("TagID", t.TagID));
                attrs.Add(new Attribute("Anchor", t.Anchor.ToString()));
                attrs.Add(new Attribute("AlignmentAnchor", t.AlignmentAnchor.ToString()));
                attrs.Add(new Attribute("TextEquivalent", t.TextEquivalent));
                segmentTags.Add(new InlineTag("_" + t.TagID, t.Type, attrs));
            }
            // if there are no segment tags, we can't do much
            // TODO - think it over, is there anything we could do? still add these tags? but which ones?
            return segmentTags;
        }

        #region Tag classes

        private class InlineTag
        {
            public string Name { get; private set; }
            public TagType TagType { get; private set; }
            public List<Attribute> Attributes { get; private set; }
            /// <summary>
            /// The index to mark the matching pairs. Only open/close tags have an index, for all other tags the index is -1. 
            /// </summary>
            public int Index { get; set; }

            public InlineTag(string name, TagType type, List<Attribute> attributes)
            {
                this.Name = name;
                this.TagType = type;
                this.Attributes = attributes;
                this.Index = -1;
            }
        }

        private class Attribute
        {
            public string Name { get; private set; }
            public string Value { get; private set; }

            public Attribute(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }

        #endregion
    }
}
