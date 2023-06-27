using System.Collections.Generic;
using System.Linq;
using GoogleCloudTranslationProvider.ViewModel;
using Sdl.LanguagePlatform.Core;

namespace GoogleCloudTranslationProvider.Models
{
	public class PairMapping : BaseViewModel
	{
		private List<RetrievedGlossary> _availableGlossaries;
		private RetrievedGlossary _selectedGlossary;
		private List<RetrievedCustomModel> _availableModels;
		private RetrievedCustomModel _selectedModel;

		public string DisplayName { get; set; }

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
			get => _selectedGlossary ??= AvailableGlossaries.FirstOrDefault();
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
			get => _selectedModel ??= AvailableModels.FirstOrDefault();
			set
			{
				if (_selectedModel == value) return;
				_selectedModel = value;
				OnPropertyChanged();
			}
		}
	}
}