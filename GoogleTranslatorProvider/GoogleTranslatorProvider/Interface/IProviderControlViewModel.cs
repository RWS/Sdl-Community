using GoogleTranslatorProvider.Model;
using GoogleTranslatorProvider.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GoogleTranslatorProvider.Interfaces
{
	public delegate void ClearMessageEventRaiser();

	public interface IProviderControlViewModel
	{
		BaseModel ViewModel { get; set; }

		ICommand ShowSettingsCommand { get; set; }

		List<TranslationOption> TranslationOptions { get; set; }

		List<GoogleApiVersion> GoogleApiVersions { get; set; }

		GoogleApiVersion SelectedGoogleApiVersion { get; set; }

		TranslationOption SelectedTranslationOption { get; set; }

		bool IsV2Checked { get; set; }

		bool PersistGoogleKey { get; set; }


		bool IsTellMeAction { get; set; }

		bool BasicCsvGlossary { get; set; }


		string ApiKey { get; set; } //Google

		string JsonFilePath { get; set; }

		string ProjectName { get; set; }

		string GoogleEngineModel { get; set; }

		string ProjectLocation { get; set; }

		string GlossaryId { get; set; }

		string GlossaryPath { get; set; }

		event ClearMessageEventRaiser ClearMessageRaised;
	}
}
