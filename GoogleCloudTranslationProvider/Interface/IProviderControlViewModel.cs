using System.Collections.Generic;
using GoogleCloudTranslationProvider.Models;
using GoogleCloudTranslationProvider.Service;
using GoogleCloudTranslationProvider.ViewModel;
using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		BaseViewModel ViewModel { get; set; }

		List<PairMapping> Mappings { get; set; }

		List<TranslationOption> TranslationOptions { get; set; }

		TranslationOption SelectedTranslationOption { get; }

		List<GoogleApiVersion> GoogleApiVersions { get; set; }

		GoogleApiVersion SelectedGoogleApiVersion { get; }

		RetrievedGlossary SelectedGlossary { get; }

		bool BasicCsvGlossary { get; set; }

		bool PersistGoogleKey { get; set; }

		bool IsTellMeAction { get; set; }

		bool IsV2Checked { get; }

		string GoogleEngineModel { get; set; }

		string VisibleJsonPath { get; set; }

		string JsonFilePath { get; set; }

		string ProjectId { get; set; }

		string ApiKey { get; set; }

		string ProjectLocation { get; set; }

		string GlossaryPath { get; set; }

		string GlossaryId { get; set; }

		event ClearMessageEventRaiser ClearMessageRaised;

		bool CanConnectToGoogleV2(HtmlUtil htmlUtil);
		bool CanConnectToGoogleV3(LanguagePair[] languagePairs);
	}
}