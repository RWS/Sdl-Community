using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using Microsoft.VisualBasic;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.CleanUpTasks
{
	public class ConversionCleanupHandler : SegmentHandlerBase, ISegmentHandler
    {
        private readonly List<ConversionItemList> conversionItemLists = null;
        private readonly List<Placeholder> placeholderList = new List<Placeholder>();
        private readonly IXmlReportGenerator reportGenerator = null;
        private readonly ISettings settings = null;
        private readonly List<ITagPair> tagPairList = new List<ITagPair>();
        private List<Tuple<string, IText>> textList = new List<Tuple<string, IText>>();
        private readonly BatchTaskMode taskMode;

        public ConversionCleanupHandler(ISettings settings,
                                    List<ConversionItemList> conversionItems,
                                    IDocumentItemFactory itemFactory,
                                    ICleanUpMessageReporter reporter,
                                    IXmlReportGenerator reportGenerator,
                                    BatchTaskMode taskMode)
            : base(itemFactory, reporter)
        {
            this.settings = settings;
            conversionItemLists = conversionItems;
            this.reportGenerator = reportGenerator;
            this.taskMode = taskMode;
        }

        public List<Placeholder> PlaceholderList { get { return placeholderList; } }

        public void VisitSegment(ISegment segment)
        {
            if (ShouldSkip(segment)) { return; }

            VisitChildren(segment);

            CleanUpSource(segment);

            ResetFields();
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            tagPairList.Add(tagPair);
            VisitChildren(tagPair);
        }

        public void VisitText(IText text)
        {
            var txt = text.Properties.Text;

            textList.Add(new Tuple<string, IText>(txt, text));
        }

        private static VbStrConv ConvertToEnum(SearchText search)
        {
            VbStrConv vbStrConv = VbStrConv.None;
            foreach (var v in search.VbStrConv)
            {
                vbStrConv += (int)v;
            }

            return vbStrConv;
        }

        private bool AttributesChanged(string updatedText, string fullText)
        {
            bool result = false;

            try
            {
                var tagPair1 = XElement.Parse(updatedText);
                var tagPair2 = XElement.Parse(fullText);

                if (tagPair1.HasAttributes && tagPair2.HasAttributes)
                {
                    var attrib1 = tagPair1.Attributes();
                    var attrib2 = tagPair2.Attributes();

                    if (attrib1.Count() == attrib2.Count())
                    {
                        foreach (var attrib in attrib1)
                        {
                            result = !attrib2.Any(a => a.Name == attrib.Name && a.Value == attrib.Value);
                        }
                    }
                }
            }
            catch (System.Xml.XmlException)
            {
                // Tag may contain invalid xml, in that case we ignore it
            }

            return result;
        }

        private void CleanUpSource(ISegment segment)
        {
            if (textList.Count == 0 && tagPairList.Count == 0)
            {
                return;
            }

            foreach (var itemList in conversionItemLists)
            {
                foreach (var item in itemList.Items)
                {
                    ProcessText(item, segment);
                }
            }
        }

        private bool ContainsTags(string updatedText)
        {
            return Regex.IsMatch(updatedText, "<.+?>");
        }

        private bool ContentChanged(string updatedText, string fullText)
        {
            var match1 = Regex.Match(updatedText, "(<.+?>)(.+?)(</.+?>)");
            var match2 = Regex.Match(fullText, "(<.+?>)(.+?)(</.+?>)");

            if (match1.Success && match2.Success)
            {
                return match1.Groups[2].Value != match2.Groups[2].Value;
            }

            return false;
        }

        private List<IAbstractMarkupData> CreateMarkupData(string input, HtmlTagTable tagTable, HtmlEntitizer entitizer)
        {
            var markupList = new List<IAbstractMarkupData>();
            var parser = new HtmlHelper(entitizer.Entitize(input), tagTable);

            if (parser.ParseErrors.Count() > 0)
            {
                return ParseTagsFallback(input, markupList);
            }

            foreach (var node in parser.Descendants())
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    if (!tagTable.Table[node.OriginalName].HasEndTag)
                    {
                        var ph = parser.GetRawStartTag(node);
                        var phTag = CreatePlaceHolderTag(ph);
                        markupList.Add(phTag);
                    }
                    else if (tagTable.Table[node.OriginalName].IsEndGhostTag)
                    {
                        var eTag = parser.GetRawEndTag(node);
                        markupList.Add(CreatePlaceHolderTag(eTag));
                    }
                    else
                    {
                        var stTag = parser.GetRawStartTag(node);

                        if (node.Closed)
                        {
                            var eTag = parser.GetRawEndTag(node);
                            var startTag = CreateStartTag(stTag);
                            var endTag = CreateEndTag(eTag);
                            var tagPair = CreateTagPair(startTag, endTag);

                            if (!ContainsTags(node.InnerHtml))
                            {
                                if (!string.IsNullOrEmpty(node.InnerHtml))
                                {
                                    var itext = CreateIText(entitizer.DeEntitize(node.InnerHtml));
                                    tagPair.Add(itext);
                                }

                                // Experimental:
                                // Creation of new formatting
                                CreateNewFormatting(node, tagPair);
                            }
                            else
                            {
                                var list = CreateMarkupData(node.InnerHtml, tagTable, entitizer);

                                foreach (var item in list)
                                {
                                    tagPair.Add(item);
                                }
                            }

                            markupList.Add(tagPair);
                        }
                        else
                        {
                            var phTag = CreatePlaceHolderTag(stTag);
                            markupList.Add(phTag);

                            var list = CreateMarkupData(node.InnerHtml, tagTable, entitizer);

                            foreach (var item in list)
                            {
                                markupList.Add(item);
                            }
                        }

                        node.RemoveAllChildren();
                    }
                }
                else if (node.NodeType == HtmlNodeType.Text)
                {
                    markupList.Add(CreateIText(entitizer.DeEntitize(node.InnerText)));
                }
                else
                {
                    markupList.Add(CreateIText(entitizer.DeEntitize(node.InnerHtml)));
                }
            }

            return markupList;
        }

        private List<IAbstractMarkupData> ParseTagsFallback(string input, List<IAbstractMarkupData> markupList)
        {
            // Fall back on regex parsing
            foreach (var chunk in Regex.Split(input, "(<.+?>)"))
            {
                if (string.IsNullOrEmpty(chunk))
                {
                    continue;
                }

                if (Regex.IsMatch(chunk, "<.+?>"))
                {
                    var phTag = CreatePlaceHolderTag(chunk);
                    markupList.Add(phTag);
                }
                else
                {
                    markupList.Add(CreateIText(chunk));
                }
            }

            return markupList;
        }

        private void CreateNewFormatting(HtmlNode node, ITagPair tagPair)
        {
            if (node.Name == "cf" && node.HasAttributes)
            {
                var formattingGroup = CreateFormattingGroup();

                foreach (var attrib in node.Attributes)
                {
                    formattingGroup.Add(CreateFormattingItem(attrib.OriginalName, attrib.Value));
                }

                tagPair.StartTagProperties.Formatting = formattingGroup;
            }
        }

        private void CreatePlaceHolder(string updatedText, IText itext, string replacementText)
        {
            if (string.IsNullOrEmpty(replacementText))
            {
                Reporter.Report(this, ErrorLevel.Warning, "Replacement text was empty", updatedText);
                return;
            }

            var m = Regex.Match(replacementText, @"<(\w+)\b\s*[^<]*?/>");
            var parent = itext.Parent;

            if (m.Success)
            {
                var matches = Regex.Matches(updatedText, $@"<{m.Groups[1].Value}\b\s*[^<]*?/>");

                if (matches.Count > 0)
                {
                    var index = itext.IndexInParent;

                    var splitString = "";
                    foreach (Match match in matches)
                    {
                        if (string.IsNullOrEmpty(splitString))
                        {
                            splitString += $"({Regex.Escape(match.Value)})";
                        }
                        else
                        {
                            splitString += $"|({Regex.Escape(match.Value)})";
                        }
                    }

                    foreach (var item in Regex.Split(updatedText, splitString))
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            continue;
                        }

                        var phMatch = Regex.Match(item, $@"<{m.Groups[1].Value}\b\s*[^<]*?/>");

                        if (phMatch.Success)
                        {
                            var phTag = CreatePlaceHolderTag(phMatch.Value);

                            if (phTag != null)
                            {
                                StorePlaceholder(phMatch.Value);

                                if (index >= 0)
                                {
                                    parent.Insert(index++, phTag);
                                }
                            }
                        }
                        else
                        {
                            parent.Insert(index++, CreateIText(item));
                        }
                    }

                    itext.RemoveFromParent();
                }
            }
            else
            {
                var matchTagPair = Regex.Match(updatedText, "(<.+?>)(.+?)(</.+?>)");

                if (matchTagPair.Success)
                {
                    CreateTagPair(updatedText, itext, parent);
                }
                else
                {
                    Reporter.Report(this, ErrorLevel.Warning, "Placeholder not found", $"{updatedText}");
                }
            }
        }

        private void CreateTagPair(string updatedText, IText itext, IAbstractMarkupDataContainer parent)
        {
            var entitizer = new HtmlEntitizer();
            var markupData = CreateMarkupData(updatedText, new HtmlTagTable(entitizer.Entitize(updatedText)), entitizer);

            var index = itext.IndexInParent;

            if (markupData.Count > 0)
            {
                foreach (var item in markupData)
                {
                    parent.Insert(index++, item);
                }

                itext.RemoveFromParent();
            }
        }

        private void GetFullText(ITagPair tagPair, StringBuilder stringBuilder)
        {
            var startTag = tagPair.StartTagProperties.TagContent;
            stringBuilder.Append(startTag);

            foreach (var item in tagPair.AllSubItems)
            {
                // Make sure we do not add content of container nodes
                if (object.ReferenceEquals(item.Parent, tagPair))
                {
                    if (item is IText)
                    {
                        var txt = ((IText)item).Properties.Text;
                        stringBuilder.Append(txt);
                    }
                    else if (item is IPlaceholderTag)
                    {
                        var tag = ((IPlaceholderTag)item).TagProperties.TagContent;
                        stringBuilder.Append(tag);
                    }
                    else if (item is ITagPair)
                    {
                        GetFullText((ITagPair)item, stringBuilder);
                    }
                }
            }

            var endTag = tagPair.EndTagProperties.TagContent;
            stringBuilder.Append(endTag);
        }

        private void Log(string original, IText itext, ConversionItem item, string convertedText)
        {
            if (itext.Parent is ISegment)
            {
                var segment = (ISegment)itext.Parent;
                reportGenerator.AddConversionItem(segment.Properties.Id.Id, original, convertedText, item.Search.Text, item.Replacement.Text);
            }
            else if (itext.Parent is ITagPair)
            {
                var tagPair = (ITagPair)itext.Parent;
                if (tagPair.Parent is ISegment)
                {
                    var segment = (ISegment)tagPair.Parent;
                    reportGenerator.AddConversionItem(segment.Properties.Id.Id, original, convertedText, item.Search.Text, item.Replacement.Text);
                }
            }
        }

        private void ProcessSingleString(string original, IText itext, ConversionItem item)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            if (search.EmbeddedTags && taskMode == BatchTaskMode.Source)
            {
                bool result = false;

                if (search.UseRegex)
                {
                    result = original.RegexCompare(search.Text, search.CaseSensitive);
                }
                else if (search.WholeWord)
                {
                    var searchText = "\\b" + Regex.Escape(search.Text) + "\\b";
                    result = original.RegexCompare(searchText, search.CaseSensitive);
                }
                else
                {
                    result = original.NormalStringCompare(search.Text, search.CaseSensitive, settings.SourceCulture);
                }

                if (result)
                {
                    Log(original, itext, item, original);

                    CreateTagPair(original, itext, itext.Parent);

                    StoreTagPair(original);
                }
            }
            else if (replacement.Placeholder && taskMode == BatchTaskMode.Source)
            {
                string updatedText;
                bool result = false;

                if (search.UseRegex)
                {
                    result = TryRegexUpdate(original, item, out updatedText);
                }
                else if (search.WholeWord)
                {
                    result = TryRegexUpdate(original, item, out updatedText, true);
                }
                else
                {
                    result = TryNormalStringUpdate(original, item, out updatedText);
                }

                if (result)
                {
                    Log(original, itext, item, updatedText);

                    CreatePlaceHolder(updatedText, itext, replacement.Text);
                }
            }
            else if (!search.TagPair && !search.EmbeddedTags && !replacement.Placeholder)
            {
                string convertedText;
                var replaceSuccessful = TryReplaceString(original, item, search, out convertedText);

                if (replaceSuccessful)
                {
                    Log(original, itext, item, convertedText);

                    UpdateIText(convertedText, itext);
                }
            }
        }

        private void ProcessText(ConversionItem item, ISegment segment)
        {
            if (item.Search.TagPair && taskMode == BatchTaskMode.Source)
            {
                TagPairUpdate(item);
                return;
            }

            foreach (var pair in textList)
            {
                if (string.IsNullOrEmpty(pair.Item1))
                {
                    Reporter.Report(this, ErrorLevel.Warning, "Source was empty", new TextLocation(pair.Item2, 0), new TextLocation(pair.Item2, pair.Item1.Length));
                }
                else
                {
                    ProcessSingleString(pair.Item1, pair.Item2, item);
                }
            }

            TextListUpdater updater = new TextListUpdater();
            updater.VisitSegment(segment);

            // Replace list with current version
            textList = updater.TextList;
        }

        private void ReplaceTagPair(string updatedText, ITagPair tagPair, IAbstractMarkupDataContainer parent)
        {
            var entitizer = new HtmlEntitizer();
            var markupData = CreateMarkupData(updatedText, new HtmlTagTable(entitizer.Entitize(updatedText)), entitizer);

            var index = tagPair.IndexInParent;

            if (markupData.Count > 0)
            {
                foreach (var item in markupData)
                {
                    parent.Insert(index++, item);
                }

                tagPair.RemoveFromParent();
            }
        }

        private void ResetFields()
        {
            textList.Clear();
            tagPairList.Clear();
        }

        private bool SearchTextValid(SearchText search)
        {
            if (string.IsNullOrWhiteSpace(search.Text))
            {
                Reporter.Report(this, ErrorLevel.Warning, "Search text is empty", $"{search.Text}");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void StorePlaceholder(string value, bool isTagPair = false)
        {
            if (!placeholderList.Any(p => p.Content.Contains(value)))
            {
                placeholderList.Add(new Placeholder() { Content = value, IsTagPair = isTagPair });
            }
        }

        private void StoreTagPair(string original)
        {
            var matches = Regex.Matches(original, "<.+?>");
            foreach (Match m in matches)
            {
                var tag = m.Value;
                var index = m.Value.LastIndexOf("/>");
                if (index > -1 && m.Value[index - 1] != ' ')
                {
                    tag = m.Value.Insert(index, " ");
                }

                StorePlaceholder(tag, true);
            }
        }

        private void TagPairUpdate(ConversionItem item)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            if (SearchTextValid(search))
            {
                foreach (var tagpair in tagPairList)
                {
                    var stringBuilder = new StringBuilder();
                    GetFullText(tagpair, stringBuilder);
                    var fullText = stringBuilder.ToString();
                    string updatedText;
                    var result = TryRegexUpdateTagPair(fullText, item, out updatedText);

                    if (result)
                    {
                        UpdateTagPair(updatedText, fullText, tagpair);

                        if (tagpair.Parent is ISegment)
                        {
                            var segment = (ISegment)tagpair.Parent;
                            reportGenerator.AddConversionItem(segment.Properties.Id.Id, fullText, updatedText, item.Search.Text, item.Replacement.Text);
                        }
                    }
                }
            }
        }

        private bool TagsChanged(string updatedText, string fullText)
        {
            var match1 = Regex.Match(updatedText, "(<.+?>)(.+?)(</.+?>)");
            var match2 = Regex.Match(fullText, "(<.+?>)(.+?)(</.+?>)");

            if (match1.Success && match2.Success)
            {
                var startTag1 = Regex.Match(match1.Groups[1].Value, "<(.+?)\\b.*>");
                var startTag2 = Regex.Match(match2.Groups[1].Value, "<(.+?)\\b.*>");

                // TagPair may contain invalid xml, such as non-quoted attributes
                //var startTag1 = XElement.Parse(updatedText).Name;
                //var startTag2 = XElement.Parse(fullText).Name;

                if (startTag1.Success && startTag2.Success)
                {
                    return startTag1.Groups[1].Value != startTag2.Groups[1].Value;
                }
            }

            return true;
        }

        private bool TryNormalStringUpdate(string original, ConversionItem item, out string updatedText)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            updatedText = null;

            if (SearchTextValid(search))
            {
                if (original.NormalStringCompare(search.Text, search.CaseSensitive, settings.SourceCulture))
                {
                    updatedText = original.NormalStringReplace(search.Text, replacement.Text, search.CaseSensitive);
                }
            }

            return updatedText != null;
        }

        private bool TryRegexUpdate(string original, ConversionItem item, out string updatedText, bool wholeWord = false)
        {
            var search = item.Search;
            var replacement = item.Replacement;

            string searchText;
            if (wholeWord)
            {
                searchText = "\\b" + Regex.Escape(search.Text) + "\\b";
            }
            else
            {
                searchText = search.Text;
            }

            updatedText = null;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                if (original.RegexCompare(searchText, search.CaseSensitive))
                {
                    updatedText = original.RegexReplace(searchText, replacement, search.CaseSensitive);
                }
            }

            // Because the method returns input unchanged if there is no match, you can use
            // the Object.ReferenceEquals method to determine whether the method has made any
            // replacements to the input string.
            return (updatedText != null) && !object.ReferenceEquals(original, updatedText);
        }

        private bool TryRegexUpdateTagPair(string original, ConversionItem item, out string updatedText, bool wholeWord = false)
        {
            var result = false;
            var search = item.Search;
            var replacement = item.Replacement;
            updatedText = null;

            if (search.StrConv == false &&
                replacement.ToLower == false &&
                replacement.ToUpper == false)
            {
                return TryRegexUpdate(original, item, out updatedText, wholeWord);
            }

            // Create a dummy ConversionItem has ToUpper, ToLower and StrConv set to false
            var dummy = new ConversionItem()
            {
                Search = new SearchText()
                {
                    CaseSensitive = search.CaseSensitive,
                    Text = search.Text,
                    UseRegex = search.UseRegex,
                    WholeWord = search.WholeWord
                },
                Replacement = new ReplacementText()
                {
                    Placeholder = replacement.Placeholder,
                    Text = replacement.Text
                }
            };

            string updatedTextTemp1;
            string updatedTextTemp2;
            result = TryRegexUpdate(original, dummy, out updatedTextTemp1, wholeWord);
            result = TryRegexUpdate(original, item, out updatedTextTemp2, wholeWord);

            if (result)
            {
                if (ContainsTags(updatedTextTemp2))
                {
                    var match1 = Regex.Match(updatedTextTemp1, "(<.+?>)(.+?)(</.+?>)");
                    var match2 = Regex.Match(updatedTextTemp2, "(<.+?>)(.+?)(</.+?>)");

                    if (match1.Success && match2.Success)
                    {
                        updatedText = match1.Groups[1].Value +
                                      match2.Groups[2].Value +
                                      match1.Groups[3].Value;
                    }
                    else
                    {
                        // By setting this to updatedTextTemp1 we ignore ToUpper, ToLower and Strconv
                        //
                        updatedText = updatedTextTemp1;
                    }
                }
                else
                {
                    // If it doesn't contain tags, we just use the normal version
                    updatedText = updatedTextTemp2;
                }
            }

            return result;
        }

        private bool TryReplaceString(string original, ConversionItem item, SearchText search, out string updatedText)
        {
            var result = false;

            if (search.StrConv)
            {
                result = TryStrConvUpdate(original, item, out updatedText);
            }
            else if (search.UseRegex)
            {
                result = TryRegexUpdate(original, item, out updatedText);
            }
            else if (search.WholeWord)
            {
                result = TryRegexUpdate(original, item, out updatedText, true);
            }
            else
            {
                result = TryNormalStringUpdate(original, item, out updatedText);
            }

            return result;
        }

        private bool TryStrConvUpdate(string original, ConversionItem item, out string updatedText)
        {
            var search = item.Search;
            var replacement = item.Replacement;
            updatedText = null;

            if (SearchTextValid(search))
            {
                VbStrConv vbStrConv = ConvertToEnum(search);

                try
                {
                    if (settings.SourceCulture != null)
                    {
                        updatedText = original.RegexReplace(search.Text, replacement, search.CaseSensitive,
                                                            m => Strings.StrConv(m.Value, vbStrConv, settings.SourceCulture.LCID));
                    }
                    else if (vbStrConv.HasFlag(VbStrConv.Hiragana) || vbStrConv.HasFlag(VbStrConv.Katakana))
                    {
                        updatedText = original.RegexReplace(search.Text, replacement, search.CaseSensitive,
                                                            m => Strings.StrConv(m.Value, vbStrConv, new CultureInfo("ja-JP").LCID));
                    }
                    else
                    {
                        updatedText = original.RegexReplace(search.Text, replacement, search.CaseSensitive,
                                                            m => Strings.StrConv(m.Value, vbStrConv));
                    }
                }
                catch (ArgumentException e)
                {
                    Reporter.Report(this, ErrorLevel.Warning, $"Error {search.Text}: {e.Message}", "Exception happened");
                }
            }

            // String compare has to be always case-sensitive here
            return (updatedText != null) && !original.NormalStringCompare(updatedText, true, settings.SourceCulture);
        }

        private void UpdateIText(string updatedText, IText itext)
        {
            // updatedText could be empty if string was removed
            if (updatedText != string.Empty)
            {
                itext.Properties.Text = updatedText;
            }
            else
            {
                // If string is empty, remove the IText from the container
                itext.RemoveFromParent();
            }
        }

        private void UpdateTagContent(string updatedText, ITagPair tagPair)
        {
            var match = Regex.Match(updatedText, "(<.+?>)(.+?)(</.+?>)");

            if (match.Success)
            {
                var entitizer = new HtmlEntitizer();
                var markupData = CreateMarkupData(match.Groups[2].Value, new HtmlTagTable(entitizer.Entitize(match.Groups[2].Value)), entitizer);

                if (markupData.Count > 0)
                {
                    tagPair.Clear();

                    foreach (var item in markupData)
                    {
                        tagPair.Add(item);
                    }
                }
            }
            else
            {
                Reporter.Report(this, ErrorLevel.Error, "Could not update tag content", $"Error: {updatedText}");
            }
        }

        private void UpdateTagPair(string updatedText, string fullText, ITagPair tagPair)
        {
            var parent = tagPair.Parent;

            if (ContainsTags(updatedText))
            {
                try
                {
                    // Check if element name changed
                    if (TagsChanged(updatedText, fullText))
                    {
                        ReplaceTagPair(updatedText, tagPair, parent);
                    }
                    else
                    {
                        // Check if the attributes changed
                        if (AttributesChanged(updatedText, fullText))
                        {
                            var attributes = XElement.Parse(updatedText).Attributes();
                            var formatting = tagPair.StartTagProperties.Formatting;

                            var match = Regex.Match(updatedText, "(<.+?>)(.+?)(</.+?>)");
                            var oldTagContent = tagPair.StartTagProperties.TagContent.Split(new[] { ' ', '>' });

                            tagPair.StartTagProperties.DisplayText = match.Groups[1].Value;
                            tagPair.StartTagProperties.TagContent = match.Groups[1].Value;

                            foreach (var attrib in attributes)
                            {
                                var formattingItem = FormattingFactory.CreateFormatting(attrib.Name.LocalName, attrib.Value);
                                formatting.Add(formattingItem);

                                var metaDataList = new List<KeyValuePair<string, string>>();
                                foreach (var data in tagPair.TagProperties.MetaData)
                                {
                                    foreach (var piece in oldTagContent)
                                    {
                                        var m = Regex.Match(piece, "(.+?)=\"(.+?)\"");
                                        if (m.Success)
                                        {
                                            var updated = data.Value.Replace($"\"{m.Groups[2].Value}\"", $"\"{attrib.Value}\"");

                                            if (!ReferenceEquals(data.Value, updated))
                                            {
                                                metaDataList.Add(new KeyValuePair<string, string>(data.Key, updated));
                                            }
                                        }
                                    }
                                }

                                foreach (var pair in metaDataList)
                                {
                                    tagPair.TagProperties.SetMetaData(pair.Key, pair.Value);
                                }
                            }
                        }

                        if (ContentChanged(updatedText, fullText))
                        {
                            UpdateTagContent(updatedText, tagPair);
                        }
                    }
                }
                catch (ArgumentException)
                {
                    Reporter.Report(this, ErrorLevel.Error, "Error parsing updated text. Xml invalid.", $"Error: {updatedText}");
                }
            }
            else
            {
                var itext = CreateIText(updatedText);
                parent.Add(itext);

                tagPair.RemoveFromParent();
            }
        }

        private void VisitChildren(IAbstractMarkupDataContainer container)
        {
            foreach (var item in container)
            {
                item.AcceptVisitor(this);
            }
        }

        private string WrapInTags(string input)
        {
            return string.Concat("<root>", input, "</root>");
        }

        #region Not Used

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
        }

        /// <summary>
        /// Placeholder tags are ignored, as the translatable content
        /// is assumed to be fixed
        /// </summary>
        /// <param name="tag"><see cref="IPlaceholderTag"/></param>
        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
        }

        #endregion Not Used
    }
}