using System.Collections.ObjectModel;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class LanguageMappingModel : BaseViewModel
	{
		private string _projectLanguagePair;
		private string _selectedMTCodeSource;
		private string _selectedMTCodeTarget;
		private TranslationModel _selectedModelOption;
		private ObservableCollection<TranslationModel> _engines;
		private ObservableCollection<string> _mtCodesSource;
		private ObservableCollection<string> _mtCodesTarget;
				
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

		public ObservableCollection<string> MTCodesSource
		{
			get => _mtCodesSource;
			set
			{
				_mtCodesSource = value;
				OnPropertyChanged(nameof(MTCodesSource));
			}
		}

		public ObservableCollection<string> MTCodesTarget
		{
			get => _mtCodesTarget;
			set
			{
				_mtCodesTarget = value;
				OnPropertyChanged(nameof(MTCodesTarget));
			}
		}

		public string SelectedMTCodeTarget
		{
			get => _selectedMTCodeTarget;
			set
			{
				_selectedMTCodeTarget = value;
				OnPropertyChanged(nameof(SelectedMTCodeTarget));
			}
		}

		public string SelectedMTCodeSource
		{
			get => _selectedMTCodeSource;
			set
			{
				_selectedMTCodeSource = value;
				OnPropertyChanged(nameof(SelectedMTCodeSource));
			}
		}
	}
}
