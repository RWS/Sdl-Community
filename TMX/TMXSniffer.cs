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

            if (System.IO.File.Exists(nativeFilePath))
            {
                // call method to check if file is supported
                info.IsSupported = IsFileSupported(nativeFilePath);
                // call method to determine the file language pair
                GetFileLanguages(ref info, nativeFilePath);
            }
            else
            {
                info.IsSupported = false;
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

            info.DetectedSourceLanguage =
                new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(new Language(tmxSource), DetectionLevel.Certain); 

            if (doc.SelectSingleNode("tmx/body/tu[1]/tuv[2]").Attributes[0] != null && 
                doc.SelectSingleNode("tmx/body/tu[1]/tuv[2]").Attributes[0].InnerText.Length==5)
            {
                string tmxTarget = doc.SelectSingleNode("tmx/body/tu[1]/tuv[2]").Attributes[0].InnerText;
                info.DetectedTargetLanguage = new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(new Language(tmxTarget), 
                    DetectionLevel.Certain);
            }
        }

    }
}
