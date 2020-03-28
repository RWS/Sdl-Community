using System.Collections.ObjectModel;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public class LanguageMappingModel : BaseViewModel
	{
		private string _projectLanguagePair;
		private LangMappingMTCode _selectedMTCodeSource;
		private LangMappingMTCode _selectedMTCodeTarget;
		private TranslationModel _selectedModelOption;
		private ObservableCollection<TranslationModel> _engines;
		private ObservableCollection<LangMappingMTCode> _mtCodesSource;
		private ObservableCollection<LangMappingMTCode> _mtCodesTarget;
		private ObservableCollection<MTCloudDictionary> _mtCloudDictionaries;
		private MTCloudDictionary _selectedMTCloudDictionary;

		public string ProjectLanguagePair
		{
			get => _projectLanguagePair;
			set
			{
				_projectLanguagePair = value;
				OnPropertyChanged(nameof(ProjectLanguagePair));
			}
		}

		public ObservableCollection<TranslationModel> Engines
		{
			get => _engines;
			set
			{
				_engines = value;
				OnPropertyChanged(nameof(Engines));
			}
		}

		public TranslationModel SelectedModelOption
		{
			get => _selectedModelOption;
			set
			{
				_selectedModelOption = value;
				OnPropertyChanged(nameof(SelectedModelOption));
			}
		}

		public ObservableCollection<LangMappingMTCode> MTCodesSource
		{
			get => _mtCodesSource;
			set
			{
				_mtCodesSource = value;
				OnPropertyChanged(nameof(MTCodesSource));
			}
		}

		public ObservableCollection<LangMappingMTCode> MTCodesTarget
		{
			get => _mtCodesTarget;
			set
			{
				_mtCodesTarget = value;
				OnPropertyChanged(nameof(MTCodesTarget));
			}
		}

		public LangMappingMTCode SelectedMTCodeTarget
		{
			get => _selectedMTCodeTarget;
			set
			{
				_selectedMTCodeTarget = value;
				OnPropertyChanged(nameof(SelectedMTCodeTarget));
			}
		}

		public LangMappingMTCode SelectedMTCodeSource
		{
			get => _selectedMTCodeSource;
			set
			{
				_selectedMTCodeSource = value;
				OnPropertyChanged(nameof(SelectedMTCodeSource));
			}
		}

		public ObservableCollection<MTCloudDictionary> MTCloudDictionaries
		{
			get => _mtCloudDictionaries;
			set
			{
				_mtCloudDictionaries = value;
				OnPropertyChanged(nameof(MTCloudDictionaries));
			}
		}

		public MTCloudDictionary SelectedMTCloudDictionary
		{
			get => _selectedMTCloudDictionary;
			set
			{
				_selectedMTCloudDictionary = value;
				OnPropertyChanged(nameof(SelectedMTCloudDictionary));
			}
		}

		public string SourceTradosCode { get; set; }

		public string TargetTradosCode { get; set; }
	}
}
