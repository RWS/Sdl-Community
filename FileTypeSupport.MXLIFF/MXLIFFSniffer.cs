using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Xml;

namespace Sdl.Community.FileTypeSupport.MXLIFF
{
    internal class MXLIFFSniffer : INativeFileSniffer
    {
        private const string BilingualDocument = "xliff";
        private const string File = "file";
        private const string SourceLanguage = "source-language";
        private const string TargetLanguage = "target-language";

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
                var memsource = doc.DocumentElement.Attributes["xmlns:m"];

                if (memsource != null && memsource.Value == "http://www.memsource.com/mxlf/2.0")
                {
                    result = true;
                }
            }

            return result;
        }

        // retrieve the source and target language
        // from the file header
        private void GetFileLanguages(ref SniffInfo info, string nativeFilePath)
        {
            var doc = new XmlDocument();

            doc.Load(nativeFilePath);

            if (doc.DocumentElement != null)
            {
                var fileElements = doc.GetElementsByTagName(File);

                foreach (XmlElement file in fileElements)
                {
                    if (file.HasAttributes)
                    {
                        XmlAttribute source = file.Attributes[SourceLanguage];
                        if (source != null && source.Value.Length == 5)
                        {
                            info.DetectedSourceLanguage =
                                new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(new Language(source.Value),
                                    DetectionLevel.Certain);
                        }

                        XmlAttribute target = file.Attributes[TargetLanguage];
                        if (target != null && target.Value.Length == 5)
                        {
                            info.DetectedTargetLanguage =
                                new Sdl.FileTypeSupport.Framework.Pair<Language, DetectionLevel>(new Language(target.Value),
                                    DetectionLevel.Certain);
                        }
                    }
                }
            }
        }
    }
}