using System.Xml;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.FileTypeSupport.TXML
{
    class TxmlSniffer : INativeFileSniffer
    {
        private const string BilingualDocument = "txml";
        private const string SourceLanguage = "locale";
        private const string TargetLanguage = "targetlocale";


        public SniffInfo Sniff(string nativeFilePath, Language suggestedSourceLanguage, 
            Codepage suggestedCodepage, INativeTextLocationMessageReporter messageReporter, 
            ISettingsGroup settingsGroup)
        {
            var info = new SniffInfo();

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
            var doc = new XmlDocument();
            doc.Load(nativeFilePath);
            if (doc.DocumentElement != null && doc.DocumentElement.Name == BilingualDocument)
            {
                result = true;
            }

            return result;
        }



        // retrieve the source and target language
        // from the file header
        private void GetFileLanguages(ref SniffInfo info, string nativeFilePath)
        {
            var doc = new XmlDocument();
            
            doc.Load(nativeFilePath);
            if (doc.DocumentElement != null && doc.DocumentElement.HasAttributes)
            {
                XmlAttribute source = doc.DocumentElement.Attributes[SourceLanguage];
                if (source != null && source.Value.Length ==5)
                {
                    info.DetectedSourceLanguage =
                        new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(new Language(source.Value),
                            DetectionLevel.Certain);
                }

                XmlAttribute target = doc.DocumentElement.Attributes[TargetLanguage];
                if (target != null && target.Value.Length ==5)
                {
                    info.DetectedTargetLanguage =
                        new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(new Language(target.Value),
                            DetectionLevel.Certain);
                }
            }
        }

    }
}
