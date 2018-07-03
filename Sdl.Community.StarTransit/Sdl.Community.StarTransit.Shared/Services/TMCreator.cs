using System.Globalization;
using System.IO;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public static class TMCreator
	{
		#region Public Methods
		public static FileBasedTranslationMemory CreateTM(string projectPath, CultureInfo sourceLanguage, CultureInfo targetLanguage, string fileName)
		{		
			var tm = new FileBasedTranslationMemory(
			Path.Combine(projectPath, string.Concat(fileName, ".sdltm")),
			string.Concat(fileName, " description"),
			sourceLanguage,
			targetLanguage,
			GetFuzzyIndexes(),
			GetRecognizers(),
			TokenizerFlags.DefaultFlags,
			WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);

			tm.LanguageResourceBundles.Clear();
			tm.Save();

			return tm;
		}
		#endregion

		#region Private Methods
		private static FuzzyIndexes GetFuzzyIndexes()
		{
			return FuzzyIndexes.SourceCharacterBased |
				FuzzyIndexes.SourceWordBased |
				FuzzyIndexes.TargetCharacterBased |
				FuzzyIndexes.TargetWordBased;
		}

		private static BuiltinRecognizers GetRecognizers()
		{
			return BuiltinRecognizers.RecognizeNone;
		}
		#endregion
	}
}
