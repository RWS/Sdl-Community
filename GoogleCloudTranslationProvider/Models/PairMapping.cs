using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Documents;
using GoogleCloudTranslationProvider.ViewModel;
using Newtonsoft.Json;

namespace GoogleCloudTranslationProvider.Models
{
	public class PairMapping : BaseViewModel
	{
		private List<RetrievedGlossary> _availableGlossaries;
		private List<RetrievedCustomModel> _availableModels;

		private RetrievedGlossary _selectedGlossary;
		private RetrievedCustomModel _selectedModel;

		[JsonIgnore]
		public string DisplayName => $"{SourceDisplayName} - {TargetDisplayName}";

		public string SourceDisplayName { get; set; }

		public string SourceLanguageCode { get; set; }

		public CultureInfo SourceCulture { get; set; }

		public string TargetDisplayName { get; set; }

		public string TargetLanguageCode { get; set; }

		public CultureInfo TargetCulture { get; set; }

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
			get => _selectedGlossary ??= AvailableGlossaries.First();
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
			get => _selectedModel ??= AvailableModels.First();
			set
			{
				if (_selectedModel == value) return;
				_selectedModel = value;
				OnPropertyChanged();
			}
		}
	}
}