using Sdl.Community.XmlReader.Helpers;
using Sdl.Community.XmlReader.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.XmlReader.Data
{
    public class LocalDatabase
    {
        private static List<TargetLanguageCode> _languageCodes;

        public LocalDatabase()
        {
            _languageCodes = new List<TargetLanguageCode>();
        }

        public static void MockLanguageCodes()
        {
            _languageCodes = new List<TargetLanguageCode>();

            AddFile(@"C:\Users\lparaschivescu\Downloads\Analyze Files de-DE_en-GB.xml");
            AddFile(@"C:\Users\lparaschivescu\Downloads\Analyze Files en-US_en-GB.xml");
            AddFile(@"C:\Users\lparaschivescu\Downloads\Analyze Files en-GB_it-IT.xml");
            AddFile(@"C:\Users\lparaschivescu\Downloads\Analyze Files en-US_az-Cyrl-AZ - Copy.xml");
        }

        public static void AddFile(string analyzeFilePath)
        {
            var languageCode = Helper.GetTargetLanguageCode(Helper.GetFileName(analyzeFilePath));
            var targetLanguageCode = _languageCodes.FirstOrDefault(x => x.LanguageCode.Equals(languageCode));

            if (targetLanguageCode == null)
            {
                targetLanguageCode = new TargetLanguageCode
                {
                    LanguageCode = languageCode,
                    AnalyzeFiles = new List<AnalyzeFile>()
                };

                _languageCodes.Add(targetLanguageCode);
            }

            targetLanguageCode.AnalyzeFiles.Add(new AnalyzeFile
            {
                Name = Helper.GetXMLFileName(analyzeFilePath) + " " + (targetLanguageCode.AnalyzeFiles.Count + 1),
                Path = analyzeFilePath
            });

        }

        public void DeleteFile(string languageCode, string fileName)
        {
            var targetLanguageCode = _languageCodes.FirstOrDefault(x => x.LanguageCode.Equals(languageCode));
            if (targetLanguageCode == null) return;

            var analyzeFile = targetLanguageCode.AnalyzeFiles.FirstOrDefault(x => x.Name.Equals(fileName));
            if (analyzeFile == null) return;

            targetLanguageCode.AnalyzeFiles.Remove(analyzeFile);
        }

        public void ResetLanguageCodes() { _languageCodes.Clear(); }

        public static List<TargetLanguageCode> GetLanguageCodes() { return _languageCodes; }

        public static List<AnalyzeFile> GetAnalyzeFilesByLanguageCode(string languageCode)
        {
            var targetLanguageCode = _languageCodes.FirstOrDefault(x => x.LanguageCode.Equals(languageCode));
            if (targetLanguageCode == null) return null;

            return targetLanguageCode.AnalyzeFiles;
        }
    }
}
