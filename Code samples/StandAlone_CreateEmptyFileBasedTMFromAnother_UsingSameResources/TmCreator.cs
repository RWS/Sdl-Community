using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;

namespace StandAlone_CreateEmptyFileBasedTMFromAnother_UsingSameResources
{
    public class TmCreator
    {

        public FileBasedTranslationMemory CreateFileBasedTm(string tmPath)
        {
            FileBasedTranslationMemory tm = new FileBasedTranslationMemory(
                tmPath,
                "This is a sample TM",
                CultureInfo.GetCultureInfo("en-US"),
                CultureInfo.GetCultureInfo("de-DE"),
                this.GetFuzzyIndexes(),
                this.GetRecognizers(),
                TokenizerFlags.BreakOnDash | TokenizerFlags.BreakOnHyphen | TokenizerFlags.BreakOnApostrophe,
                WordCountFlags.BreakOnTag | WordCountFlags.BreakOnHyphen | WordCountFlags.BreakOnApostrophe | WordCountFlags.BreakOnDash
            );

            tm.LanguageResourceBundles.Clear();
            tm.Save();
            return tm;
        }

        private FuzzyIndexes GetFuzzyIndexes()
        {
            return FuzzyIndexes.SourceCharacterBased |
                   FuzzyIndexes.SourceWordBased |
                   FuzzyIndexes.TargetCharacterBased |
                   FuzzyIndexes.TargetWordBased;
        }

        private BuiltinRecognizers GetRecognizers()
        {
            return BuiltinRecognizers.RecognizeAcronyms |
                   BuiltinRecognizers.RecognizeDates |
                   BuiltinRecognizers.RecognizeNumbers |
                   BuiltinRecognizers.RecognizeTimes |
                   BuiltinRecognizers.RecognizeVariables |
                   BuiltinRecognizers.RecognizeMeasurements |
                   BuiltinRecognizers.RecognizeAlphaNumeric;
        }

    }
}
