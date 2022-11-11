using System.Collections.Generic;
using System.Windows.Input;
using GoogleTranslatorProvider.Models;

namespace GoogleTranslatorProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		BaseModel ViewModel { get; set; }

		TranslationOption SelectedTranslationOption { get; }

		List<TranslationOption> TranslationOptions { get; set; }

		List<GoogleApiVersion> GoogleApiVersions { get; set; }

		GoogleApiVersion SelectedGoogleApiVersion { get; set; }

		bool IsV2Checked { get; }

		bool PersistGoogleKey { get; set; }

		bool IsTellMeAction { get; set; }

		string ApiKey { get; set; }

		string JsonFilePath { get; set; }

		string ProjectName { get; set; }

		string GoogleEngineModel { get; set; }

		string ProjectLocation { get; set; }

		string GlossaryId { get; set; }

		string GlossaryPath { get; set; }

		bool BasicCsvGlossary { get; set; }

		ICommand ShowSettingsCommand { get; set; }

		event ClearMessageEventRaiser ClearMessageRaised;
	}
}