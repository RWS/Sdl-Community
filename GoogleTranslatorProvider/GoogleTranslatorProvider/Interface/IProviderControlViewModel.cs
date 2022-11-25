using System.Collections.Generic;
using GoogleTranslatorProvider.Models;

namespace GoogleTranslatorProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		BaseModel ViewModel { get; set; }

		List<TranslationOption> TranslationOptions { get; set; }

		TranslationOption SelectedTranslationOption { get; }

		List<GoogleApiVersion> GoogleApiVersions { get; set; }

		GoogleApiVersion SelectedGoogleApiVersion { get; }

		bool BasicCsvGlossary { get; set; }

		bool PersistGoogleKey { get; set; }

		bool IsTellMeAction { get; set; }

		bool IsV2Checked { get; }

		string GoogleEngineModel { get; set; }

		string VisibleJsonPath { get; set; }

		string JsonFilePath { get; set; }

		string ProjectName { get; set; }

		string ApiKey { get; set; }

		string ProjectLocation { get; set; }

		ProjectLocation SelectedLocation { get; set; }

		string GlossaryPath { get; set; }

		string GlossaryId { get; set; }

		event ClearMessageEventRaiser ClearMessageRaised;
	}
}