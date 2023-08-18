using System.Collections.Generic;
using System.Linq;
using GoogleCloudTranslationProvider.ViewModel;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.Models
{
	public class LanguagePairResources : BaseViewModel
	{
		private string _sourceLanguageCode;
		private string _targetLanguageCode;
		private List<RetrievedGlossary> _availableGlossaries;
		private RetrievedGlossary _selectedGlossary;
		private List<RetrievedCustomModel> _availableModels;
		private RetrievedCustomModel _selectedModel;

		public string DisplayName { get; set; }

		public string SourceLanguageCode
		{
			get => _sourceLanguageCode;
			set
			{
				if (_sourceLanguageCode == value) return;
				_sourceLanguageCode = value;
				OnPropertyChanged();
			}
		}

		public string TargetLanguageCode
		{
			get => _targetLanguageCode;
			set
			{
				if (_targetLanguageCode == value) return;
				_targetLanguageCode = value;
				OnPropertyChanged();
			}
		}

		public LanguagePair LanguagePair { get; set; }

		public List<RetrievedGlossary> AvailableGlossaries
		{
			get => _availableGlossaries;
			set
			{
				if (_availableGlossaries == value) return;
				_availableGlossaries = value;
				OnPropertyChanged();
			}
		}

		public RetrievedGlossary SelectedGlossary
		{
			get => _selectedGlossary;
			set
			{
				if (_selectedGlossary == value) return;
				_selectedGlossary = value;
				OnPropertyChanged();
			}
		}

		public List<RetrievedCustomModel> AvailableModels
		{
			get => _availableModels;
			set
			{
				if (_availableModels == value) return;
				_availableModels = value;
				OnPropertyChanged();
			}
		}

		public RetrievedCustomModel SelectedModel
		{
			get => _selectedModel;
			set
			{
				if (_selectedModel == value) return;
				_selectedModel = value;
				OnPropertyChanged();
			}
		}

		[JsonIgnore]
		public bool GlossariesAreAvailable => AvailableGlossaries.Any() && AvailableGlossaries[0].DisplayName != PluginResources.RetrievedResources_NotAvailable;

		[JsonIgnore]
		public bool ModelsAreAvailable => AvailableModels.Any() && AvailableModels[0].DisplayName != PluginResources.RetrievedResources_NotAvailable;
	}
}