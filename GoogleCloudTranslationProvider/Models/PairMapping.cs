using System.Collections.Generic;
using System.Windows.Documents;
using GoogleCloudTranslationProvider.ViewModel;

namespace GoogleCloudTranslationProvider.Models
{
	public class PairMapping : BaseViewModel
	{
		private string _sourceDisplayName;
		private string _sourceLanguageCode;
		
		private string _targetDisplayName;
		private string _targetLanguageCode;

		private List<RetrievedGlossary> _availableGlossaries;
		private RetrievedGlossary _selectedGlossary;

		private List<RetrievedCustomModel> _availableModels;
		private RetrievedCustomModel _selectedModel;

		public string SourceDisplayName
		{
			get => _sourceDisplayName;
			set
			{
				if (_sourceDisplayName == value) return;
				_sourceDisplayName = value;
				OnPropertyChanged();
			}
		}

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

		public string TargetDisplayName
		{
			get => _targetDisplayName;
			set
			{
				if (_targetDisplayName == value) return;
				_targetDisplayName = value;
				OnPropertyChanged();
			}
		}

		public string TargetLanguageCode
		{
			get => _targetLanguageCode;
			set
			{
				if (value == _targetLanguageCode) return;
				_targetLanguageCode = value;
				OnPropertyChanged();
			}
		}

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
	}
}