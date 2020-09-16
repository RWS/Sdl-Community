using System.Globalization;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.MtEnhancedProvider.ViewModel.Interface
{
	public interface IGoogleV3Connecter
	{
		void SetGoogleAvailableLanguages();
		void TryToAuthenticateUser();
		bool IsSupportedLanguage(CultureInfo sourceLanguage, CultureInfo targetLanguage);
		string ProjectName { get; set; }
		string JsonFilePath { get; set; }
		string EngineModel { get; set; }
		string Location { get; set; }
		string GlossaryPath { get; set; }
		string GlossaryId { get; set; }
		string TranslateText(CultureInfo sourceLanguage, CultureInfo targetLanguage, string sorceText);
		void CreateGoogleGlossary(LanguagePair[] languagePairs);
	}
}
