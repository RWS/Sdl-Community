namespace Sdl.Community.FileTypeSupport.MXLIFF
{
    using Sdl.Core.Globalization;
    using Sdl.Core.Settings;
    using Sdl.FileTypeSupport.Framework.BilingualApi;
    using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
    using Sdl.FileTypeSupport.Framework.IntegrationApi;
    using Sdl.FileTypeSupport.Framework.NativeApi;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal class MXLIFFParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware
    {
        private readonly Queue<int> _tagIds = new Queue<int>();
        private readonly Dictionary<int, IPlaceholderTagProperties> _tags = new Dictionary<int, IPlaceholderTagProperties>();
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>();
        private XmlDocument _document;
        private IFileProperties _fileProperties;
        private XmlNamespaceManager _nsmgr;
        private int _totalTagCount;
        private int _workflowLevel;

        public event EventHandler<ProgressEventArgs> Progress;

        public IDocumentProperties DocumentProperties { get; set; }

        public IBilingualContentHandler Output
        {
            get; set;
        }

        public void Dispose()
        {
            _document = null;
        }

        public void EndOfInput()
        {
            OnProgress(100);
            _document = null;
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
            // Loading of filter settings
        }

        public bool ParseNext()
        {
            if (DocumentProperties == null)
            {
                DocumentProperties = ItemFactory.CreateDocumentProperties();
            }

            Output.Initialize(DocumentProperties);

            var fileInfo = ItemFactory.CreateFileProperties();
            fileInfo.FileConversionProperties = _fileProperties.FileConversionProperties;
            Output.SetFileProperties(fileInfo);

            // Variables for the progress report
            var xmlNodeList = _document.SelectNodes("//x:group", _nsmgr);
            if (xmlNodeList != null)
            {
                var totalUnitCount = xmlNodeList.Count;
                var currentUnitCount = 0;
                foreach (XmlNode item in xmlNodeList)
                {
                    foreach(var unit in CreateParagraphUnit(item))
                    {
                        Output.ProcessParagraphUnit(unit);
                    }

                    // Update the progress report
                    currentUnitCount++;
                    OnProgress(Convert.ToByte(Math.Round(100 * ((decimal)currentUnitCount / totalUnitCount), 0)));
                }
            }

            Output.FileComplete();
            Output.Complete();

            return false;
        }

        public void SetFileProperties(IFileProperties properties)
        {
            _fileProperties = properties;
        }

        public void StartOfInput()
        {
            OnProgress(0);
            _document = new XmlDocument();
            _document.Load(_fileProperties.FileConversionProperties.OriginalFilePath);
            _nsmgr = new XmlNamespaceManager(_document.NameTable);
            _nsmgr.AddNamespace("x", "urn:oasis:names:tc:xliff:document:1.2");
            _nsmgr.AddNamespace("m", "http://www.memsource.com/mxlf/2.0");

            // Acquire workflow level
            var level = _document.DocumentElement.Attributes["m:level"];

            if (level != null)
            {
                _workflowLevel = int.TryParse(level.Value, out _) ? int.Parse(level.Value) : 0;
            }

            // Acquire users
            var memsourceUsers = _document.SelectNodes("//m:user", _nsmgr);

            if (memsourceUsers != null)
            {
                foreach (XmlElement user in memsourceUsers)
                {
                    var id = user.Attributes["id"]?.Value;
                    var username = user.Attributes["username"]?.Value;

                    _users.Add(id ?? "_UNKNOWN_", username ?? "_UNKNOWN_");
                }
            }
        }

        protected virtual void OnProgress(byte percent)
        {
            Progress?.Invoke(this, new ProgressEventArgs(percent));
        }

        private ICommentProperties CreateComment(XmlNode commentNode)
        {
	        var commentProperties = PropertiesFactory.CreateCommentProperties();

            var createdBy = commentNode?.Attributes?["created-by"]?.Value;
            var resolved = commentNode?.Attributes?["resolved"]?.Value;

            var author = string.Empty;
            if (createdBy != null)
            {
                author = _users[createdBy];
            }

            var severity = Severity.Low;

            if (resolved != null && (resolved == "false"))
            {
                severity = Severity.Medium;
            }

            var comment = PropertiesFactory.CreateComment(commentNode.InnerText, author, severity);

            commentProperties.Add(comment);

            return commentProperties;
        }

        private ConfirmationLevel CreateConfirmationLevel(XmlNode transUnit)
        {
            ConfirmationLevel sdlxliffLevel;

            if (transUnit != null)
            {
                var confirmed = transUnit.Attributes?["m:confirmed"];
                var levelEdited = transUnit.Attributes?["m:level-edited"];

                if (confirmed != null && (confirmed.Value == "1"))
                {
                    if (_workflowLevel > 1 && levelEdited != null && (levelEdited.Value == "true"))
                    {
                        sdlxliffLevel = ConfirmationLevel.ApprovedTranslation;
                    }
                    else
                    {
                        sdlxliffLevel = ConfirmationLevel.Translated;
                    }
                }
                else
                {
                    if (_workflowLevel > 1 && levelEdited != null && (levelEdited.Value == "true"))
                    {
                        sdlxliffLevel = ConfirmationLevel.RejectedTranslation;
                    }
                    else
                    {
                        var target = transUnit.SelectSingleNode("x:target", _nsmgr);
                        if (target != null && !string.IsNullOrWhiteSpace(target.InnerText))
                        {
                            sdlxliffLevel = ConfirmationLevel.Draft;
                        }
                        else
                        {
                            sdlxliffLevel = ConfirmationLevel.Unspecified;
                        }
                    }
                }
            }
            else
            {
                sdlxliffLevel = ConfirmationLevel.Unspecified;
            }

            return sdlxliffLevel;
        }

        private IContextProperties CreateContext(string id, string paraId)
        {
	        var contextProperties = PropertiesFactory.CreateContextProperties();

            var contextId = PropertiesFactory.CreateContextInfo("id");
            contextId.SetMetaData("ID", id);
            contextId.Description = "Trans-unit id";
            contextId.DisplayCode = "ID";

            var contextParaId = PropertiesFactory.CreateContextInfo("para-id");
            contextParaId.SetMetaData("PARA ID", paraId);
            contextParaId.Description = "Para-id";
            contextParaId.DisplayCode = "PARA ID";

            contextProperties.Contexts.Add(contextId);
            contextProperties.Contexts.Add(contextParaId);

            return contextProperties;
        }

        private byte CreateMatchValue(XmlNode transUnit)
        {
            byte matchValue = 0;

            // "m:gross-score" and "m:score" are both percentage values of TM matches from which the given segment is pre-translated.
            // "m:gross-score" is the percentage prior to the application of TM penalization, if any, while "m:score" is the percentage after penalization.
            var score = transUnit?.Attributes?["m:score"];

            if (score != null)
            {
	            matchValue = Convert.ToByte(Convert.ToDouble(score.Value, CultureInfo.InvariantCulture.NumberFormat) * 100);
            }

            return matchValue;
        }

        private List<IParagraphUnit> CreateParagraphUnit(XmlNode xmlUnit)
        {
	        var units = new List<IParagraphUnit>();
	        var xmlNodes = xmlUnit.SelectNodes("x:trans-unit", _nsmgr);

            foreach (XmlNode xmlNode in xmlNodes)
            {
				// Create paragraph unit object
				var paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

                if (xmlNode != null)
                {
                    var id = xmlNode.Attributes["id"];
                    var paraId = xmlNode.Attributes["m:para-id"];

                    if (id != null && paraId != null)
                    {
                        paragraphUnit.Properties.Contexts = CreateContext(id.Value, paraId.Value);
                    }

					// Create segment pair object
					var segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
                    var tuOrg = ItemFactory.CreateTranslationOrigin();

                    // Assign the appropriate confirmation level to the segment pair
                    segmentPairProperties.ConfirmationLevel = CreateConfirmationLevel(xmlNode);
                    tuOrg.MatchPercent = CreateMatchValue(xmlNode);

                    // Add source segment to paragraph unit
                    var srcSegment = CreateSegment(xmlNode.SelectSingleNode("x:source", _nsmgr), segmentPairProperties, true);

                    paragraphUnit.Source.Add(srcSegment);

                    // Add target segment to paragraph unit if available
                    if (xmlNode.SelectSingleNode("x:target", _nsmgr) != null)
                    {
                        var trgSegment = CreateSegment(xmlNode.SelectSingleNode("x:target", _nsmgr), segmentPairProperties, false);

                        // Check if locked
                        var locked = xmlNode.Attributes["m:locked"];
                        if (locked != null && locked.Value == "true")
                        {
                            trgSegment.Properties.IsLocked = true;
                        }

                        // Check if target empty and look for alt-trans
                        if (trgSegment.Count == 0)
                        {
                            var alttrans = xmlNode.SelectSingleNode("x:alt-trans/x:target", _nsmgr);
                            if (alttrans != null && !string.IsNullOrWhiteSpace(alttrans.InnerText))
                            {
                                PopulateSegment(trgSegment, alttrans, false);

                                var alttransOrigin = xmlNode.SelectSingleNode("x:alt-trans", _nsmgr).Attributes["origin"];
                                if (alttransOrigin != null)
                                {
                                    var origin = alttransOrigin.Value;
                                    if (origin.Contains("machine") || origin.Contains("mt"))
                                    {
                                        tuOrg.OriginType = DefaultTranslationOrigin.MachineTranslation;
                                    }
                                    else if (origin.Contains("tm"))
                                    {
                                        tuOrg.OriginType = DefaultTranslationOrigin.TranslationMemory;
                                    }

                                    tuOrg.OriginSystem = origin;
                                }
                            }
                        }

                        paragraphUnit.Target.Add(trgSegment);
                    }
                    else
                    {
                        var singleNode = xmlNode.SelectSingleNode("x:source", _nsmgr);
                        if (singleNode != null) singleNode.InnerText = "";
                        var trgSegment = CreateSegment(xmlNode.SelectSingleNode("x:source", _nsmgr), segmentPairProperties, false);
                        paragraphUnit.Target.Add(trgSegment);
                    }

                    var transOrigin = xmlNode.Attributes["m:trans-origin"];
                    if (transOrigin.Value != null && transOrigin.Value != "null")
                    {
                        tuOrg.OriginType = transOrigin.Value;
                    }

                    segmentPairProperties.TranslationOrigin = tuOrg;

                    // Add comments
                    if (xmlNode.SelectSingleNode("m:comment", _nsmgr) != null)
                    {
                        paragraphUnit.Properties.Comments = CreateComment(xmlNode.SelectSingleNode("m:comment", _nsmgr));
                    }
                }

                // Clear any ids on the queue
                _tagIds.Clear();

                units.Add(paragraphUnit);
            }

            return units;
        }

        private IPlaceholderTag CreatePhTag(string tagContent, int tagNo, bool source)
        {
	        var phTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
	        var phTag = ItemFactory.CreatePlaceholderTag(phTagProperties);

            phTagProperties.TagContent = tagContent;
            phTagProperties.DisplayText = $@"{{{tagNo}}}";
            phTagProperties.CanHide = false;

            if (source)
            {
                var thisId = new TagId(_totalTagCount.ToString(CultureInfo.InvariantCulture));

                phTagProperties.TagId = thisId;

                if (_tags.ContainsKey(int.Parse(thisId.Id)) && !_tags.ContainsValue(phTagProperties))
                {
                    _totalTagCount = CreatePlaceholderTagHelper(thisId, phTagProperties);
                }

                if (!(_tags.ContainsKey(int.Parse(thisId.Id)) && _tags.ContainsValue(phTagProperties)))
                {
                    _tags.Add(_totalTagCount, phTagProperties);
                }

                _tagIds.Enqueue(_totalTagCount);

                _totalTagCount++;
            }
            else
            {
                int id;
                if (_tagIds.Count != 0)
                {
                    id = _tagIds.Dequeue();
                }
                else
                {
                    id = _totalTagCount++;
                }


                var thisId = new TagId(id.ToString(CultureInfo.InvariantCulture));

                phTagProperties.TagId = thisId;

                if (_tags.ContainsKey(int.Parse(thisId.Id)) && !_tags.ContainsValue(phTagProperties))
                {
                    _totalTagCount = CreatePlaceholderTagHelper(thisId, phTagProperties);
                }
            }

            return phTag;
        }

        private int CreatePlaceholderTagHelper(TagId placeholderId, IPlaceholderTagProperties phTagProperties)
        {
            var max = _tags.Keys.Max();
            var id = max + 1;
            placeholderId.Id = id.ToString();
            phTagProperties.TagId = placeholderId;

            _tags.Add(id, phTagProperties);

            return id;
        }

        private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair, bool source)
        {
            var segment = ItemFactory.CreateSegment(pair);

            PopulateSegment(segment, segNode, source);

            return segment;
        }

        private IText CreateText(string segText)
        {
            var textProperties = PropertiesFactory.CreateTextProperties(segText);
            var textContent = ItemFactory.CreateText(textProperties);

            return textContent;
        }

        private void PopulateSegment(ISegment segment, XmlNode node, bool source)
        {
            int i = 1;
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.NodeType == XmlNodeType.Text)
                {
                    foreach (var chunk in Regex.Split(item.InnerText, @"((?:{|<)\^*[a-z0-9]{0,2}(?:>|}))"))
                    {
                        if (Regex.IsMatch(chunk, @"((?:{|<)\^*[a-z0-9]{0,2}(?:>|}))"))
                        {
                            segment.Add(CreatePhTag(chunk, i, source));
                            i++;
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                segment.Add(CreateText(chunk));
                            }
                        }
                    }
                }

                if (item.NodeType == XmlNodeType.Element)
                {
                    segment.Add(CreatePhTag(item.OuterXml, i, source));
                    i++;
                }
            }
        }
    }
}