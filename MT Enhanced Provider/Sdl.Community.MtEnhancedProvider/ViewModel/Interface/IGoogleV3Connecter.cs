using System.Globalization;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface IGoogleV3Connecter
	{
		string GlossaryId { get; set; }
		string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sorceText);
		void CreateGoogleGlossary(LanguagePair[] languagePairs);
		void SetGoogleAvailableLanguages();
		void TryToAuthenticateUser();
		bool IsSupportedLanguage(CultureInfo sourceLanguage, CultureInfo targetLanguage);
	}
}
