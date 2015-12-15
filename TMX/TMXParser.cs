using System;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileType.TMX
{
    class TMXParser : AbstractBilingualFileTypeComponent, IBilingualParser, INativeContentCycleAware, ISettingsAware
    {
        private IFileProperties _fileProperties;
        private IDocumentProperties _documentProperties;
        private XmlDocument _document;
        public event EventHandler<ProgressEventArgs> Progress;
        int segId;


 

        public void SetFileProperties(IFileProperties properties)
        {
            _fileProperties = properties;
        }


        public void StartOfInput()
        {
            OnProgress(0);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader xmlReader = XmlTextReader.Create(_fileProperties.FileConversionProperties.OriginalFilePath, settings);


            _document = new XmlDocument();

            _document.Load(xmlReader);

  
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
            int totalUnitCount = _document.SelectNodes("//tu").Count;
            int currentUnitCount = 0;
            foreach (XmlNode item in _document.SelectNodes("//tu"))
            {
                Output.ProcessParagraphUnit(CreateParagraphUnit(item));

                // update the progress report   
                currentUnitCount++;
                OnProgress(Convert.ToByte(Math.Round(100 * ((decimal)currentUnitCount / totalUnitCount), 0)));
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

            segId++;
            string id = Convert.ToString(segId);
            paragraphUnit.Properties.Contexts = CreateContext("Paragraph", id);



           // create segment pair object
           ISegmentPairProperties segmentPairProperties = ItemFactory.CreateSegmentPairProperties();
           ITranslationOrigin tuOrg = ItemFactory.CreateTranslationOrigin();

           // assign the appropriate confirmation level to the segment pair            
           segmentPairProperties.ConfirmationLevel = CreateConfirmationLevel(xmlUnit);
           tuOrg.MatchPercent = this.CreateMatchValue();

            
           // add source segment to paragraph unit
           ISegment srcSegment = CreateSegment(xmlUnit.SelectSingleNode("tuv[1]/seg"), segmentPairProperties);
           paragraphUnit.Source.Add(srcSegment);

           // add target segment to paragraph unit
           ISegment trgSegment = CreateSegment(xmlUnit.SelectSingleNode("tuv[2]/seg"), segmentPairProperties);
           paragraphUnit.Target.Add(trgSegment);


            if(tuOrg.MatchPercent >0)
                    tuOrg.OriginType = DefaultTranslationOrigin.TranslationMemory;

            segmentPairProperties.TranslationOrigin = tuOrg;

            
            return paragraphUnit;
        }

        private ConfirmationLevel CreateConfirmationLevel(XmlNode segmentXml)
        {
            ConfirmationLevel sdlxliffLevel = ConfirmationLevel.Translated;

            string confirmationValue = "Translated";

            if (segmentXml.SelectSingleNode("prop[@type='x-ConfirmationLevel']") != null)
            {
                confirmationValue = segmentXml.SelectSingleNode("prop[@type='x-ConfirmationLevel']").InnerText;
            }


            switch (confirmationValue)
            {
               case "Translated":
                    sdlxliffLevel = ConfirmationLevel.Translated;
                    break;
               case "Draft":
                    sdlxliffLevel = ConfirmationLevel.Draft;
                    break;
               case "ApprovedTranslation":
                    sdlxliffLevel = ConfirmationLevel.ApprovedTranslation;
                    break;
               case "ApprovedSignOff":
                    sdlxliffLevel = ConfirmationLevel.ApprovedSignOff;
                    break;
               case "RejectedTranslation":
                    sdlxliffLevel = ConfirmationLevel.RejectedTranslation;
                    break;
               case "RejectedSignOff":
                    sdlxliffLevel = ConfirmationLevel.RejectedSignOff;
                    break;
               default:
                    sdlxliffLevel = ConfirmationLevel.Translated;
                    break;
            }


 

            return sdlxliffLevel;
        }

        private Byte CreateMatchValue()
        {
            Byte matchValue;

            matchValue = 100;

            return matchValue;
        }

        // helper function for creating segment objects
        private ISegment CreateSegment(XmlNode segNode, ISegmentPairProperties pair)
        {
            int i = 1;

            ISegment segment = ItemFactory.CreateSegment(pair);

            foreach (XmlNode item in segNode.ChildNodes)
            {
                if (item.NodeType == XmlNodeType.Text)
                {
                    segment.Add(CreateText(item.InnerText));
                }

                if (item.NodeType == XmlNodeType.Element)
                {
                    segment.Add(CreatePhTag(item.Name, item, i));
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

        private IPlaceholderTag CreatePhTag(string tagContent, XmlNode item, int tagNo)
        {
            IPlaceholderTagProperties phTagProperties = PropertiesFactory.CreatePlaceholderTagProperties(tagContent);
            IPlaceholderTag phTag = ItemFactory.CreatePlaceholderTag(phTagProperties);

            string cont;
            if (item.NextSibling == null)
                cont = "";
            else
                cont = item.NextSibling.Value;

            phTagProperties.TagContent = item.OuterXml;
            phTagProperties.DisplayText = item.Name;
            phTagProperties.CanHide = false;

            return phTag;
        }

        private IContextProperties CreateContext(string spec, string unitID)
        {
            IContextProperties contextProperties = PropertiesFactory.CreateContextProperties();
            IContextInfo contextInfo = PropertiesFactory.CreateContextInfo(StandardContextTypes.TranslatableContent);
            contextInfo.Purpose = ContextPurpose.Information;

            // add unit id as metadata
            IContextInfo contextId = PropertiesFactory.CreateContextInfo("UnitId");
            contextId.SetMetaData("UnitID", unitID);
            contextId.Description="Original paragraph unit id";
            contextId.DisplayCode = "ID";

            contextProperties.Contexts.Add(contextInfo);
            contextProperties.Contexts.Add(contextId);

            return contextProperties;
        }







        public void InitializeSettings(Sdl.Core.Settings.ISettingsBundle settingsBundle, string configurationId)
        {
            //loading of filter settings
        }



        public void Dispose()
        {
            _document = null;
        }


    }
}