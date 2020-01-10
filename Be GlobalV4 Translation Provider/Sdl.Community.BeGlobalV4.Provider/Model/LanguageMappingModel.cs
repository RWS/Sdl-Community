using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	public class LanguageMappingModel : BaseViewModel
	{
		private string _projectLanguagePair;
		private string _selectedMTCodeSource;
		private string _selectedMTCodeTarget;
		private ObservableCollection<string> _engines;
		private ObservableCollection<string> _mtCodeSource;
		private ObservableCollection<string> _mtCodeTarget;

		public string ProjectLanguagePair
		{
			get => _projectLanguagePair;
			set
			{
				_projectLanguagePair = value;
				OnPropertyChanged(nameof(ProjectLanguagePair));
			}
		}

		public ObservableCollection<string> Engines
		{
			get => _engines;
			set
			{
				_engines = value;
				OnPropertyChanged(nameof(Engines));
			}
		}

		public ObservableCollection<string> MTCodeSource
		{
			get => _mtCodeSource;
			set
			{
				_mtCodeSource = value;
				OnPropertyChanged(nameof(MTCodeSource));
			}
		}

		public ObservableCollection<string> MTCodeTarget
		{
			get => _mtCodeTarget;
			set
			{
				_mtCodeTarget = value;
				OnPropertyChanged(nameof(MTCodeTarget));
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
