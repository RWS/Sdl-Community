using Sdl.LanguagePlatform.Core;
using System.Globalization;

namespace GoogleTranslatorProvider.Interfaces
{
	public interface IGoogleV3Connecter
	{
		string GlossaryId { get; set; }
		string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sorceText, string format);
		void CreateGoogleGlossary(LanguagePair[] languagePairs);
		void SetGoogleAvailableLanguages();
		void TryToAuthenticateUser();
		bool IsSupportedLanguage(CultureInfo sourceLanguage, CultureInfo targetLanguage);
	}
}