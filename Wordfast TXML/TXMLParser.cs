using System;
using System.Globalization;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileTypeSupport.TXML
{
    class TxmlParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware
    {
        private IFileProperties _fileProperties;
        private IDocumentProperties _documentProperties;
        private XmlDocument _document;
        public event EventHandler<ProgressEventArgs> Progress;

        private int _totalTagCount;
        private int _tmpTotalTagCount;
        private int _srcSegmentTagCount;


 

        public void SetFileProperties(IFileProperties properties)
        {
            _fileProperties = properties;
        }


        public void StartOfInput()
        {
            OnProgress(0);
            _document = new XmlDocument();
            _document.Load(_fileProperties.FileConversionProperties.OriginalFilePath);
        }
        

        public void EndOfInput()
        {
            OnProgress(100);
            _document = null;
        }

        protected virtual void OnProgress(byte percent)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressEventArgs(percent));
            }
        }

        public IDocumentProperties DocumentProperties
        {
            get
            {
                return _documentProperties;
            }
            set
            {
                _documentProperties = value;
            }
        }

        public IBilingualContentHandler Output
        {
            get;
            set;
        }

        public bool ParseNext()
        {
            if (_documentProperties == null)
            {
                _documentProperties = ItemFactory.CreateDocumentProperties();
            }
            Output.Initialize(_documentProperties);

            IFileProperties fileInfo = ItemFactory.CreateFileProperties();
            fileInfo.FileConversionProperties = _fileProperties.FileConversionProperties;
            Output.SetFileProperties(fileInfo);

            // variables for the progress report
            var xmlNodeList = _document.SelectNodes("//translatable");
            if (xmlNodeList != null)
            {
                int totalUnitCount = xmlNodeList.Count;
                int currentUnitCount = 0;
                foreach (XmlNode item in xmlNodeList)
                {
                    Output.ProcessParagraphUnit(CreateParagraphUnit(item));

                    // update the progress report   
                    currentUnitCount++;
                    OnProgress(Convert.ToByte(Math.Round(100 * ((decimal)currentUnitCount / totalUnitCount), 0)));
                }
            }

            Output.FileComplete();
            Output.Complete();

            return false;
        }

        // helper function for creating paragraph units
        private IParagraphUnit CreateParagraphUnit(XmlNode xmlUnit)
        {
            // create paragraph unit object
            IParagraphUnit paragraphUnit = ItemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            // create paragraph unit context
            var selectSingleNode = xmlUnit.SelectSingleNode("./@blockId");
            if (selectSingleNode != null)
            {
                string id = selectSingleNode.InnerText;
                paragraphUnit.Properties.Contexts = CreateContext(id);
            }


            var xmlNodeList = xmlUnit.SelectNodes("segment");
            if (xmlNodeList != null)
                foreach (XmlNode item in xmlNodeList)            
                {
                    // create segment pair object
                    ISegmentPairProperties segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
                    ITranslationOrigin tuOrg = ItemFactory.CreateTranslationOrigin();

                    // assign the appropriate confirmation level to the segment pair            
                    segmentPairProperties.ConfirmationLevel=CreateConfirmationLevel(item);
                    tuOrg.MatchPercent = CreateMatchValue(item);



                    // add source segment to paragraph unit
                    ISegment srcSegment = CreateSegment(item.SelectSingleNode("source"), segmentPairProperties, true);
                    paragraphUnit.Source.Add(srcSegment);
                    // add target segment to paragraph unit if available
                    if (item.SelectSingleNode("target") != null)
                    {
                        ISegment trgSegment = CreateSegment(item.SelectSingleNode("target"), segmentPairProperties, false);   
                        paragraphUnit.Target.Add(trgSegment);
                    }
                    else
                    {
                        var singleNode = item.SelectSingleNode("source");
                        if (singleNode != null) singleNode.InnerText = "";
                        ISegment trgSegment = CreateSegment(item.SelectSingleNode("source"), segmentPairProperties, false);
                        paragraphUnit.Target.Add(trgSegment);
                    }

                    if(tuOrg.MatchPercent >0)
                        tuOrg.OriginType = DefaultTranslationOrigin.TranslationMemory;


                    segmentPairProperties.TranslationOrigin = tuOrg;

                    //Add comments
                    if (item.SelectSingleNode("comments") != null)
                    {
                        paragraphUnit.Properties.Comments = CreateComment(item.SelectSingleNode("comments"));
                    }
                }

            return paragraphUnit;
        }

        private ConfirmationLevel CreateConfirmationLevel(XmlNode segmentXml)
        {
            ConfirmationLevel sdlxliffLevel;

            if (segmentXml.SelectSingleNode("target") != null)
            {
                var selectSingleNode = segmentXml.SelectSingleNode("target/@score");
                if (selectSingleNode != null && (selectSingleNode.InnerText == "100"))
                {
                    sdlxliffLevel = ConfirmationLevel.Translated;
                }
                else
                {
                    sdlxliffLevel = ConfirmationLevel.Draft;
                }
            }
            else
            {
                sdlxliffLevel = ConfirmationLevel.Unspecified;
            }

            return sdlxliffLevel;
        }

        private Byte CreateMatchValue(XmlNode segmentXml)
        {
            Byte matchValue = 0;

            if (segmentXml.SelectSingleNode("target/@score") == null) return matchValue;
            var selectSingleNode = segmentXml.SelectSingleNode("target/@score");
            if (selectSingleNode != null)
                matchValue = Convert.ToByte(selectSingleNode.InnerText);

            return matchValue;
        }

        // helper function for creating segment objects
        private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair, bool source)
        {
            int i = 1;

            ISegment segment = ItemFactory.CreateSegment(pair);

            if (source)
            {
                _srcSegmentTagCount = 0;
                if (_totalTagCount < _tmpTotalTagCount)
                {
                    _totalTagCount = _tmpTotalTagCount;
                }
            }
            else
            {
                _totalTagCount = _totalTagCount - _srcSegmentTagCount;
            }


            foreach (XmlNode item in segNode.ChildNodes)
            {
                if (item.NodeType == XmlNodeType.Text)
                {
                    segment.Add(CreateText(item.InnerText));
                }

                if (item.NodeType == XmlNodeType.Element)
                {
                    segment.Add(CreatePhTag(item.Name, item, i, source));
                    i++;
                }
            }
            return segment;
        }


        private IText CreateText(string segText)
        { 
            ITextProperties textProperties = PropertiesFactory.CreateTextProperties(segText);
            IText textContent = ItemFactory.CreateText(textProperties);

            return textContent;
        }

        private IPlaceholderTag CreatePhTag(string tagContent, XmlNode item, int tagNo, bool source)
        {
            IPlaceholderTagProperties phTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
            IPlaceholderTag phTag = ItemFactory.CreatePlaceholderTag(phTagProperties);

            
            phTagProperties.TagContent = item.OuterXml;
            phTagProperties.DisplayText = string.Format("{{{0}}}", tagNo);
            phTagProperties.CanHide = false;

            //determine tag id
            if (source)
            {
                var thisId =
                    new TagId(_totalTagCount.ToString(CultureInfo.InvariantCulture));

                phTagProperties.TagId = thisId;
                _totalTagCount += 1;
                _tmpTotalTagCount += 1;
                _srcSegmentTagCount += 1;
            }
            else
            {
                var thisId =
                    new TagId(_totalTagCount.ToString(CultureInfo.InvariantCulture));

                phTagProperties.TagId = thisId;
                _totalTagCount += 1;
            }

            return phTag;
        }

        private IContextProperties CreateContext(string unitId)
        {
            IContextProperties contextProperties = PropertiesFactory.CreateContextProperties();
            IContextInfo contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.Paragraph);
            contextInfo.Purpose = ContextPurpose.Information;

            // add unit id as metadata
            IContextInfo contextId = PropertiesFactory.CreateContextInfo("UnitId");
            contextId.SetMetaData("UnitID", unitId);
            contextId.Description="Original paragraph unit id";
            contextId.DisplayCode = "ID";

            contextProperties.Contexts.Add(contextInfo);
            contextProperties.Contexts.Add(contextId);

            return contextProperties;
        }



        private ICommentProperties CreateComment(XmlNode item)
        {
            ICommentProperties commentProperties = PropertiesFactory.CreateCommentProperties();

            var xmlNodeList = item.SelectNodes("comment");
            if (xmlNodeList != null)
                foreach (XmlNode commentNode in xmlNodeList)
                {
                    var commentSeverity=Severity.Undefined;

                    var selectSingleNode = commentNode.SelectSingleNode("./@type");
                    if (selectSingleNode != null)
                    {
                        if (selectSingleNode.InnerText == "text")
                            commentSeverity = Severity.Low;
                        if (selectSingleNode.InnerText == "question")
                            commentSeverity = Severity.Medium;
                        if (selectSingleNode.InnerText == "important")
                            commentSeverity = Severity.High;
                      
                    }

                    var singleNode = commentNode.SelectSingleNode("./@creationid");
                    if (singleNode != null)
                    {
                        IComment comment = PropertiesFactory.CreateComment(commentNode.InnerText,
                            singleNode.InnerText, commentSeverity);
                        commentProperties.Add(comment);
                    }
                }

            return commentProperties;
        }



        public void InitializeSettings(Core.Settings.ISettingsBundle settingsBundle, string configurationId)
        {
            //loading of filter settings
        }



        public void Dispose()
        {
            _document = null;
        }


    }
}