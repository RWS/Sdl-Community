using Sdl.Community.XmlReader.WPF.Helpers;
using Sdl.Community.XmlReader.WPF.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.XmlReader.WPF.Repository
{
    public static class XmlFilesRepository
    {
        private static List<TargetLanguageCode> _languageCodes = new List<TargetLanguageCode>();

        public static TargetLanguageCode AddFile(string analyzeFilePath)
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

            return targetLanguageCode;
        }

        public static void DeleteFile(string languageCode, string fileName)
        {
            var targetLanguageCode = _languageCodes.FirstOrDefault(x => x.LanguageCode.Equals(languageCode));
            if (targetLanguageCode == null) return;

            var analyzeFile = targetLanguageCode.AnalyzeFiles.FirstOrDefault(x => x.Name.Equals(fileName));
            if (analyzeFile == null) return;

            targetLanguageCode.AnalyzeFiles.Remove(analyzeFile);
        }

        public static void ResetLanguageCodes() { _languageCodes.Clear(); }

        public static List<TargetLanguageCode> GetLanguageCodes() { return _languageCodes; }

        public static List<AnalyzeFile> GetAnalyzeFilesByLanguageCode(string languageCode)
        {
            var targetLanguageCode = _languageCodes.FirstOrDefault(x => x.LanguageCode.Equals(languageCode));
            if (targetLanguageCode == null) return null;

            return targetLanguageCode.AnalyzeFiles;
        }
    }
}
