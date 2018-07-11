using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.XliffReadWrite.SDLXLIFF;

namespace Sdl.Community.XliffToLegacyConverter.Core
{
    public class Processor
    {

        public delegate void ChangedEventHandler(int maximum, int current, int percent, string message);

        public event ChangedEventHandler Progress;


        public static string GetVisualSegmentStatus(string segmentStatusId)
        {
            switch (segmentStatusId)
            {
                case "Unspecified": return "Not Translated";
                case "Draft": return "Draft";
                case "Translated": return "Translated";
                case "RejectedTranslation": return "Translation Rejected";
                case "ApprovedTranslation": return "Translation Approved";
                case "RejectedSignOff": return "Sign-off Rejected";
                case "ApprovedSignOff": return "Signed Off";
                default: return "Unknown";
            }
        }
        public static string GetSegmentStatusFromVisual(string segmentStatusId)
        {
            switch (segmentStatusId)
            {
                case "Not Translated": return "Unspecified";
                case "Draft": return "Draft";
                case "Translated": return "Translated";
                case "Translation Rejected": return "RejectedTranslation";
                case "Translation Approved": return "ApprovedTranslation";
                case "Sign-off Rejected": return "RejectedSignOff";
                case "Signed Off": return "ApprovedSignOff";
                default: return "Unknown";
            }
        }


        public Dictionary<string, ParagraphUnit> ReadRtf(string rtfFile)
        {
            var rtf = new RTF.Processor();

            Dictionary<string, ParagraphUnit> paragraphUnits;
            try
            {
                rtf.Progress += counter_Progress;

                paragraphUnits = rtf.ReadRtf(rtfFile);
            }
            finally
            {
                rtf.Progress -= counter_Progress;
            }

            return paragraphUnits;
        }

        public Dictionary<string, ParagraphUnit> ReadWord(string wrdFile)
        {
            SegmentsImported = 0;
            SegmentsNotImported = 0;

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            Dictionary<string, ParagraphUnit> paragraphUnits;
            try
            {
                var word = new Word.Processor();                
                try
                {

                    word.Progress += counter_Progress;

                    paragraphUnits = word.GetSegments(wrdFile);

                    SegmentsImported = word.SegmentsImported;
                    SegmentsNotImported = word.SegmentsNotImported;
                }
                finally
                {
                    word.Progress -= counter_Progress;                
                }
            }
            finally
            {   
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            }

            return paragraphUnits;
        }

        public Dictionary<string, ParagraphUnit> ReadTtx(string ttxFile)
        {
            SegmentsImported = 0;
            SegmentsNotImported = 0;

            var ttx = new TTX.Processor();

            Dictionary<string, ParagraphUnit> paragraphUnits;
            try
            {
                ttx.Progress += counter_Progress;

                paragraphUnits = ttx.ReadTtx(ttxFile);

                SegmentsImported = ttx.SegmentsImported;
                SegmentsNotImported = ttx.SegmentsNotImported;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ttx.Progress -= counter_Progress;
            }

            return paragraphUnits;
        }


        public int SegmentsExported;
        public int SegmentsNotExported;
        public int SegmentsImported;
        public int SegmentsNotImported;

        public void WriteTtx(string fileName, string sdlxliffFilePath, Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits, CultureInfo sourceCulture, CultureInfo targetCulture)
        {
            SegmentsExported = 0;
            SegmentsNotExported = 0;

            var ttx = new TTX.Processor();
            ttx.WriteTtx(fileName, sdlxliffFilePath, fileParagraphUnits, sourceCulture, targetCulture);

            SegmentsExported = ttx.SegmentsExported;
            SegmentsNotExported = ttx.SegmentsNotExported;

        }

        public void WriteRtf(string fileName, string sdlxliffFilePath, Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits
            , CultureInfo sourceCulture, CultureInfo targetCulture, bool saveAsDocX
            , bool includeLegacyStructure)
        {
            SegmentsExported = 0;
            SegmentsNotExported = 0;

            var rtf = new RTF.Processor();
            rtf.WriteRtf(fileName, sdlxliffFilePath, fileParagraphUnits, sourceCulture, targetCulture, includeLegacyStructure);

            SegmentsExported = rtf.SegmentsExported;
            SegmentsNotExported = rtf.SegmentsNotExported;

            var currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            try
            {

                System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                var word = new Word.Processor();
                word.SaveAsDocument(fileName, saveAsDocX);
                File.Delete(fileName);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
            }
            
           
        }

        public void WriteTmx(string fileName, string sdlxliffFilePath, Dictionary<string, Dictionary<string, ParagraphUnit>> fileParagraphUnits, CultureInfo sourceCulture, CultureInfo targetCulture, bool excludeTags, bool reverseLanguageDirection)
        {

            SegmentsExported = 0;
            SegmentsNotExported = 0;

            var tmx = new TMX.Processor
            {
                ExcludeTags = excludeTags,
                ReverseLanguageDirection = reverseLanguageDirection
            };

            tmx.WriteTmx(fileName, sdlxliffFilePath, fileParagraphUnits, sourceCulture, targetCulture);

            SegmentsExported = tmx.SegmentsExported;
            SegmentsNotExported = tmx.SegmentsNotExported;


        }



        void counter_Progress(int maximum, int current, int percent, string message)
        {
            if (Progress != null)
            {
                Progress(maximum, current, percent, message);
            }
        }


        public static List<TagUnit> SeperateTags(string text,string tagType)
        {
            //simple function to recover the tag information
            var tags = new List<TagUnit>();

            //[1:TEXT][1:TEXT]
            //<cf italic="true">che</cf>
            var sections = new List<TagUnit>();

            var rLockedContent = new Regex(@"\<ILockedContent\>(?<xContentText>.*?|)\<\/ILockedContent\>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var mcRLockedContent = rLockedContent.Matches(text);            
            var icurrent = 0;
            foreach (Match mRLockedContent in mcRLockedContent)
            {
                var indexStart = mRLockedContent.Index;
                var indexEnd = mRLockedContent.Index + mRLockedContent.Length;

                if (icurrent < indexStart)
                {
                    var substring = text.Substring(icurrent, indexStart - icurrent);
                    var xTagUnit1 = new TagUnit(string.Empty, string.Empty, substring, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsPlaceholder);
                    sections.Add(xTagUnit1);
                }

               
                var content = mRLockedContent.Groups["xContentText"].Value;
                var xTagUnit2 = new TagUnit(string.Empty, string.Empty, content, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsLockedContent);

                sections.Add(xTagUnit2);

                icurrent = indexEnd;
            }

            if (icurrent < text.Length)
            {
                var substring = text.Substring(icurrent);
                var xTagUnit1 = new TagUnit(string.Empty, string.Empty, substring, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsPlaceholder);
                sections.Add(xTagUnit1);
            }


            var rCustomType01 = new Regex(@"[XREF[^\]]*\][^\]]*\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);          
            var rEmptyTag = new Regex(@"\/\s*\>$", RegexOptions.Singleline);            
            var rClosingTag = new Regex(@"^\<\s*\/", RegexOptions.Singleline);

         
            try
            {
                foreach (var section in sections)
                {

                    if (section.Type == TagUnit.TagUnitType.IsLockedContent)
                    {
                        tags.Add(new TagUnit(string.Empty, string.Empty, section.Content, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsLockedContent));
                    }
                    else
                    {
                        var str = section.Content;
                        while ((str.IndexOf("<", StringComparison.Ordinal) >= 0) && (str.IndexOf(">", str.IndexOf("<", StringComparison.Ordinal), StringComparison.Ordinal) >= 0))
                        {
                            var posS = str.IndexOf("<", StringComparison.Ordinal);//absolute opening position of tag

                            var posO = posS;//starting position for the opening tag
                            var posC = str.IndexOf(">", posO, StringComparison.Ordinal) + 1;//closing position of tag that starts after the opening tag

                            var posOi = str.IndexOf("<", posO + 1, posC - (posO + 1), StringComparison.Ordinal);//position of a possible opening tag within the text
                            var posCi = str.Length > posC ? str.IndexOf(">", posC, StringComparison.Ordinal) : -1;//position of the next posible closing tag

                            while ((posOi > -1) && (posCi > -1))
                            {
                                posO = posOi;	//inherit the value from the internal opening tag
                                posC = posCi; //inherit the value from the internal closing tag

                                posOi = str.IndexOf("<", posO + 1, posC - (posO + 1), StringComparison.Ordinal);//position of a possible opening tag within the text
                                posCi = str.Length > posC ? str.IndexOf(">", posC + 1, StringComparison.Ordinal) : -1;//position of the next posible closing tag

                                if ((posOi < 0) || (posCi < 0)) posC++; //retrive posible closing tag position before loop escapes
                            }

                            var btag = str.Substring(0, posS); //before tag                    
                            if (btag != string.Empty)
                            {
                                if (btag.StartsWith("]"))
                                {
                                    //for tags that have back to back
                                    //[1:TEXT][1:TEXT]
                                    var btagA = btag.Substring(0, 1);
                                    tags.Add(new TagUnit(string.Empty, string.Empty, btagA, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));

                                    if (btag.Length > 1)
                                    {
                                        var btagB = btag.Substring(1).Trim();
                                        tags.Add(btagB.StartsWith("[")
                                            ? new TagUnit(string.Empty, string.Empty, btagB,
                                                TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag)
                                            : new TagUnit(string.Empty, string.Empty, btagB,
                                                TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
                                    }
                                }
                                else if (rCustomType01.Match(btag).Success)
                                {
                                    tags.Add(new TagUnit(string.Empty, string.Empty, btag, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
                                }
                                else if (btag.StartsWith("["))
                                {
                                    tags.Add(new TagUnit(string.Empty, string.Empty, btag, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
                                }
                                else if (btag.EndsWith("]"))
                                {
                                    tags.Add(new TagUnit(string.Empty, string.Empty, btag, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));
                                }
                                else
                                {
                                    tags.Add(new TagUnit(string.Empty, string.Empty, btag, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
                                }
                            }

                            var ttag = str.Substring(posS, posC - posS); //the tag
                            var atag = str.Substring(posC); //after tag


                            if (ttag != string.Empty)
                            {
								if (tagType.Equals("end"))
								{
									tags.Add(new TagUnit(string.Empty, GetEndTagName(ttag), ttag, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));
								}
								if (tagType.Equals("empty"))
								{
									var tagId = string.Empty;
									var tagName = GetStartTagName(ttag, ref tagId);
									tags.Add(new TagUnit(tagId, tagName, ttag, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
								}
								if (tagType.Equals("start"))
								{
									var tagId = string.Empty;
									var tagName = GetStartTagName(ttag, ref tagId);
									tags.Add(new TagUnit(tagId, tagName, ttag, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
								}

								//if (rClosingTag.Match(ttag).Success)
								//{
								//	tags.Add(new TagUnit(string.Empty, GetEndTagName(ttag), ttag, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));
								//}
								//else if (rEmptyTag.Match(ttag).Success)
								//{
								//	var tagId = string.Empty;
								//	var tagName = GetStartTagName(ttag, ref tagId);
								//	tags.Add(new TagUnit(tagId, tagName, ttag, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
								//}
								//else
								//{
								//	var tagId = string.Empty;
								//	var tagName = GetStartTagName(ttag, ref tagId);
								//	tags.Add(new TagUnit(tagId, tagName, ttag, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
								//}
							}

                            str = atag;
                        }

                        if (str == string.Empty) continue;
                        {
                            if (str.StartsWith("]"))
                            {
                                //for tags that have back to back
                                //[1:TEXT][1:TEXT]
                                var btagA = str.Substring(0, 1);
                                tags.Add(new TagUnit(string.Empty, string.Empty, btagA, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));

                                if (str.Length <= 1) 
                                    continue;

                                var btagB = str.Substring(1).Trim();
                                tags.Add(btagB.StartsWith("[")
                                    ? new TagUnit(string.Empty, string.Empty, btagB, TagUnit.TagUnitState.IsOpening,
                                        TagUnit.TagUnitType.IsTag)
                                    : new TagUnit(string.Empty, string.Empty, btagB, TagUnit.TagUnitState.IsEmpty,
                                        TagUnit.TagUnitType.IsTag));
                            }
                            else if (rCustomType01.Match(str).Success)
                            {
                                tags.Add(new TagUnit(string.Empty, string.Empty, str, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
                            }
                            else if (str.StartsWith("["))
                            {
                                tags.Add(new TagUnit(string.Empty, string.Empty, str, TagUnit.TagUnitState.IsOpening, TagUnit.TagUnitType.IsTag));
                            }
                            else if (str.EndsWith("]"))
                            {
                                tags.Add(new TagUnit(string.Empty, string.Empty, str, TagUnit.TagUnitState.IsClosing, TagUnit.TagUnitType.IsTag));
                            }
                            else
                            {
                                tags.Add(new TagUnit(string.Empty, string.Empty, str, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
                            }
                        }
                    }
                }
            }
            catch
            {
                var tagId = string.Empty;
                var tagName = GetStartTagName(text, ref tagId);
                tags.Add(new TagUnit(tagId, tagName, text, TagUnit.TagUnitState.IsEmpty, TagUnit.TagUnitType.IsTag));
            }

            return tags;

        }


        private static readonly Regex RegexTagName = new Regex(@"\<(?<xName>[^\s""\>]*)"
          , RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex RegexTagId = new Regex(@"\<[^\s""]*\s+(?<xAttName>[^\s""]+)\=""(?<xID>[^""]*)"""
           , RegexOptions.Singleline | RegexOptions.IgnoreCase);

        private static readonly Regex RegexTagEndName = new Regex(@"\<\s*\/\s*(?<xName>[^\s\>]*)"
                , RegexOptions.Singleline | RegexOptions.IgnoreCase);

        internal static string GetStartTagName(string text, ref string refId)
        {
            var sTagName = "";         

            var m = RegexTagName.Match(text);
            if (m.Success)
                sTagName = m.Groups["xName"].Value;

            m = RegexTagId.Match(text);
            if (!m.Success) 
                return sTagName;

            var id = m.Groups["xID"].Value;
            var attName = m.Groups["xAttName"].Value;

            if (string.Compare(attName, "id", StringComparison.OrdinalIgnoreCase) == 0)
            {
                refId = id;
            }

            return sTagName;
        }

     

        internal static string GetEndTagName(string text)
        {
            var sTagName = "";
            
            var m = RegexTagEndName.Match(text);
            if (m.Success)
                sTagName = m.Groups["xName"].Value;

            return sTagName;
        }

        internal static string GetSectionsToText(List<SegmentSection> sections)
        {
            return sections.Aggregate(string.Empty, (current, section) => current + section.Content);
        }

        internal static string GetContentTypeToMarkup(SegmentSection.ContentType contentType, string contentText, string contentId)
        {
            var rs = string.Empty;

            switch (contentType)
            {
                case SegmentSection.ContentType.Text:
                    {
                        rs = contentText;
                    } break;
                case SegmentSection.ContentType.Tag:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.TagClosing:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.Placeholder:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;
                case SegmentSection.ContentType.LockedContent:
                    {
                        rs = "<xProtected_ type=\"" + contentType + "\" id=\"" + contentId + "\">" + contentText + "</xProtected_>";
                    } break;           
            }

            return rs;
        }
    }
}
