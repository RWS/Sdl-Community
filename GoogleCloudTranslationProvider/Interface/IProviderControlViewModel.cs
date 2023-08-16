using System;
using System.Collections.Generic;
using System.Windows.Input;
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

		List<LanguagePairResources> LanguageMappingPairs { get; set; }

		List<GoogleApiVersion> GoogleApiVersions { get; set; }

		GoogleApiVersion SelectedGoogleApiVersion { get; }

		bool ProjectResourcesLoaded { get; set; }

		bool PersistGoogleKey { get; set; }

		bool IsV2Checked { get; }

		bool IsV3Checked { get; }

		string VisibleJsonPath { get; set; }

		string JsonFilePath { get; set; }

		string ProjectId { get; set; }

		string ApiKey { get; set; }

		string ProjectLocation { get; set; }

		ICommand SwitchViewExternal { get; set; }

		event EventHandler LanguageMappingLoaded;

		bool CanConnectToGoogleV2(HtmlUtil htmlUtil);

		bool CanConnectToGoogleV3(LanguagePair[] languagePairs);

		void UpdateLanguageMapping();
	}
}