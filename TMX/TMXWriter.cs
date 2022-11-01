using System.Windows.Forms;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections;
using Sdl.Community.FileType.TMX.Settings;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.FileType.TMX
{
    class TMXWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware, ISettingsAware
    {

        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private XmlDocument _targetFile;
        private TMXTextExtractor _textExtractor;
        private XmlNodeList nodeList;
        private string lastTargetFile;
        private WriterSettings _writerSettings;

        public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
        {
            _originalFileProperties = fileProperties;
        }

        public void SetOutputProperties(INativeOutputFileProperties properties)
        {
            _nativeFileProperties = properties;
        }


        public void SetFileProperties(IFileProperties fileInfo)
        {
            _targetFile = new XmlDocument();
            _targetFile.PreserveWhitespace = true;
            _targetFile.Load(_originalFileProperties.OriginalFilePath);
        }


        public void Initialize(IDocumentProperties documentInfo)
        {
            _textExtractor = new TMXTextExtractor();
        }



        public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            
            string unitId = paragraphUnit.Properties.Contexts.Contexts[1].GetMetaData("UnitID");
            //XmlNode xmlUnit = _targetFile.SelectSingleNode("//tu[" + unitId + "]");
            //MessageBox.Show(_targetFile.SelectSingleNode("//tu[" + unitId + "]").OuterXml);
            if (nodeList == null || lastTargetFile != _targetFile.BaseURI)
            {
                nodeList = _targetFile.SelectNodes("tmx/body/tu");
                lastTargetFile = _targetFile.BaseURI;
            }
            XmlNode xmlUnit = nodeList[int.Parse(unitId) - 1];

            CreateParagraphUnit(paragraphUnit, xmlUnit);
        }

        private void UpdateSegmentAttributes(XmlNode node, ISegment segment)
        {
	        var author = _textExtractor.TryGetAuthor(segment);
	        var modifiedDate = _textExtractor.TryGetModifiedDate(segment);
	        if (author != null && (_writerSettings?.WriteUserID ?? false))
	        {
		        (node as XmlElement).SetAttribute("changeid", author);
	        }

	        if (modifiedDate != null && (_writerSettings?.WriteChangeDate ?? false))
	        {
				var dateIso8601 = modifiedDate.Value.ToString("yyyyMMddTHHmmssZ");
		        (node as XmlElement).SetAttribute("changedate", dateIso8601);
	        }
		}

        private void CreateParagraphUnit(IParagraphUnit paragraphUnit, XmlNode xmlUnit)
        {
            foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
            {
                XmlNode source = xmlUnit.SelectSingleNode("./tuv[1]/seg");
                source.InnerXml = _textExtractor.GetPlainText(segmentPair.Source);

                XmlNode target = xmlUnit.SelectSingleNode("./tuv[2]/seg");
                target.InnerXml = _textExtractor.GetPlainText(segmentPair.Target);
                UpdateSegmentAttributes(xmlUnit, segmentPair.Source);

				if (xmlUnit.SelectNodes("prop[@type='x-ConfirmationLevel']").Count > 0)
                {
                    xmlUnit.SelectSingleNode("prop[@type='x-ConfirmationLevel']").InnerText = UpdateEditedStatus(segmentPair.Properties.ConfirmationLevel);
                }
            }
        }



        private string UpdateEditedStatus(ConfirmationLevel unitLevel)
        {
            string status = "";

            switch (unitLevel)
            {
                case ConfirmationLevel.Translated:
                    status = "Translated";
                    break;
                case ConfirmationLevel.Draft:
                    status = "Draft";
                    break;
                case ConfirmationLevel.Unspecified:
                    status = "Draft";
                    break;
                case ConfirmationLevel.ApprovedTranslation:
                    status = "ApprovedTranslation";
                    break;
                case ConfirmationLevel.ApprovedSignOff:
                    status = "ApprovedSignOff";
                    break;
                case ConfirmationLevel.RejectedSignOff:
                    status = "RejectedSignOff";
                    break;
                case ConfirmationLevel.RejectedTranslation:
                    status = "RejectedTranslation";
                    break;
                default:
                    status = "Translated";
                    break;
            }
            return status;
        }



        public void FileComplete()
        {
            _targetFile.Save(_nativeFileProperties.OutputFilePath);
            _targetFile = null;
        }

        public void Complete()
        {

        }



        public void Dispose()
        {
            //don't need to dispose of anything
        }

        public void InitializeSettings(ISettingsBundle settingsBundle, string configurationId)
        {
	        _writerSettings = new WriterSettings();
			_writerSettings.PopulateFromSettingsBundle(settingsBundle, configurationId);
        }
    }
}
