using System.Windows.Forms;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileType.TMX
{
    class TMXWriter : AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
    {

        private IPersistentFileConversionProperties _originalFileProperties;
        private INativeOutputFileProperties _nativeFileProperties;
        private XmlDocument _targetFile;
        private TMXTextExtractor _textExtractor;

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
            XmlNode xmlUnit = _targetFile.SelectSingleNode("//tu[" + unitId + "]");
            MessageBox.Show(_targetFile.SelectSingleNode("//tu[" + unitId + "]").OuterXml);

            CreateParagraphUnit(paragraphUnit, xmlUnit);
        }



        private void CreateParagraphUnit(IParagraphUnit paragraphUnit, XmlNode xmlUnit)
        {
            foreach (ISegmentPair segmentPair in paragraphUnit.SegmentPairs)
            {
                XmlNode source = xmlUnit.SelectSingleNode("./tuv[1]/seg");
                source.InnerXml = _textExtractor.GetPlainText(segmentPair.Source);

                XmlNode target = xmlUnit.SelectSingleNode("./tuv[2]/seg");
                target.InnerXml = _textExtractor.GetPlainText(segmentPair.Target);

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
            //don't need to dispose of anthing
        }

    }
}
