using System.IO;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.PostEdit.Compare.Core.TM
{
    public class Tm
    {        
        public static void SetTm(string tmPath, string source, string target)
        {
            if (File.Exists(tmPath))
            {
                FileTm = new FileBasedTranslationMemory(tmPath);
            }
            else
            {
                const BuiltinRecognizers recognizers = BuiltinRecognizers.RecognizeDates
                                | BuiltinRecognizers.RecognizeMeasurements
                                | BuiltinRecognizers.RecognizeNumbers
                                | BuiltinRecognizers.RecognizeTimes;

                const TokenizerFlags tokenizerFlags = TokenizerFlags.DefaultFlags;

                FileTm = new FileBasedTranslationMemory(tmPath,
                    string.Empty,
                    new System.Globalization.CultureInfo(source),
                    new System.Globalization.CultureInfo(target),
                    new FuzzyIndexes(),
                    new BuiltinRecognizers(),
                    new TokenizerFlags(),
                    new WordCountFlags()) 
                    { 
                        Recognizers = recognizers, 
                        TokenizerFlags = tokenizerFlags 
                    };
            }           
        }
      
        public static FileBasedTranslationMemory FileTm
        {
            get;
            set;
        }


        public static SearchResults GetSearchResults(Segment segment)
        {
            return FileTm.LanguageDirection.SearchSegment(GetSearchSettings(), segment);
        }

        private static SearchSettings GetSearchSettings()
        {
            var settings = new SearchSettings
            {
                MaxResults = 1,
                MinScore = 100,
                Mode = SearchMode.ExactSearch
            };


            return settings;
        }
    }
}
