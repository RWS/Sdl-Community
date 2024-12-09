using System.Globalization;
using System.IO;
using System.Xml;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileType.TMX
{
	class TMXSniffer : INativeFileSniffer
    {
        static string _BilingualDocument = "tmx";

        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, 
            Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, 
            ISettingsGroup settingsGroup)
        {
            SniffInfo info = new SniffInfo();
            info.IsSupported = false;
            if (File.Exists(nativeFilePath))
            {
                info.IsSupported = IsFileSupported(nativeFilePath);
                GetFileLanguages(ref info, nativeFilePath);
            }

            return info;
        }

        // determine whether a given file is supported based on the
        // root element
        private bool IsFileSupported(string nativeFilePath)
        {
            bool result = false;
            bool isTmx = false;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.XmlResolver = null;
            settings.DtdProcessing = DtdProcessing.Ignore;
            XmlReader xmlReader = XmlTextReader.Create(nativeFilePath, settings);          
           

            XmlDocument doc = new XmlDocument();
            
            doc.Load(xmlReader);
            if (doc.DocumentElement.Name == _BilingualDocument)
                isTmx = true;

            if (isTmx)
                result = true;


            return result;
        }

        // retrieve the source and target language
        // from the file header
        private void GetFileLanguages(ref SniffInfo info, string nativeFilePath)
        {
            XmlDocument doc = new XmlDocument();

			doc.Load(nativeFilePath);
            string tmxSource = doc.SelectSingleNode("tmx/header/@srclang").InnerText;
            var sourceLanguage = new Language(tmxSource);

			info.DetectedSourceLanguage =
                new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(sourceLanguage, DetectionLevel.Certain);

            if (doc.SelectSingleNode("tmx/body/tu[1]/tuv[2]").Attributes[0] != null &&
                doc.SelectSingleNode("tmx/body/tu[1]/tuv[2]").Attributes[0].InnerText.Length == 5)
            {
                string tmxTarget = doc.SelectSingleNode("tmx/body/tu[1]/tuv[2]").Attributes[0].InnerText;
                var targetLanguage = new Language(tmxTarget);

                info.DetectedTargetLanguage = new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(targetLanguage,
                    DetectionLevel.Certain);
            }
        }

    }
}
