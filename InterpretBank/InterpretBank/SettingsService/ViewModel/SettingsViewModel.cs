using System.Collections.Generic;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.SettingsService.Model;
using InterpretBank.SettingsService.ViewModel.Interface;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.SettingsService.ViewModel
{
	public class SettingsViewModel : ViewModel, ISubViewModel
	{
		private ICommand _chooseFilePathCommand;
		private ICommand _clearCommand;
		private string _filepath;
		private List<GlossaryModel> _glossaries;

		public SettingsViewModel(List<GlossaryModel> glossaries, List<TagModel> tags, IOpenFileDialog openFileDialog)
		{
			OpenFileDialog = openFileDialog;
			Tags = tags;
			Glossaries = glossaries;
		}

		public ICommand ChooseFilePathCommand => _chooseFilePathCommand ??= new RelayCommand(ChooseFilePath);

		public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

		public string Filepath
		{
			get => _filepath;
			set => SetField(ref _filepath, value);
		}

		public List<GlossaryModel> Glossaries
		{
			get => _glossaries;
			set => SetField(ref _glossaries, value);
		}

		public string Name => "Settings";
		public List<DbTag> SelectedTags { get; set; }
		public List<TagModel> Tags { get; set; }
		private IOpenFileDialog OpenFileDialog { get; }

		public void Clear(object parameter)
		{
			Filepath = null;
		}

		private void ChooseFilePath(object obj)
		{
			Filepath = OpenFileDialog.GetFilePath();
		}
	}
}