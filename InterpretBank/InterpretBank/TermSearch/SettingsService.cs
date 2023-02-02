using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.DAL;

namespace InterpretBank.TermSearch;

public class SettingsService : ISettingsService, INotifyPropertyChanged
{
	private ICommand _chooseFilePathCommand;
	private ICommand _clearCommand;
	private string _filepath;
	private List<DbGlossary> _glossaries;
	private List<DbTag> _tags;

	public SettingsService(IOpenFileDialog openFileDialog, IInterpretBankDataContext interpretBankDataContext)
	{
		OpenFileDialog = openFileDialog;
		InterpretBankDataContext = interpretBankDataContext;

		Tags = InterpretBankDataContext.GetRows<DbTag>().ToList();
		_glossaries = InterpretBankDataContext.GetRows<DbGlossary>().ToList();
	}

	public ICommand ChooseFilePathCommand => _chooseFilePathCommand ??= new RelayCommand(ChooseFilePath);
	public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

	public string Filepath
	{
		get => _filepath;
		set
		{
			if (value == _filepath)
				return;

			_filepath = value;
			OnPropertyChanged();
		}
	}

	public List<DbTag> SelectedTags { get; set; }

	private IInterpretBankDataContext InterpretBankDataContext { get; }
	private IOpenFileDialog OpenFileDialog { get; }

	public List<DbGlossary> Glossaries
	{
		get => _glossaries;
		set
		{
			if (Equals(value, _glossaries)) return;
			_glossaries = value;

			OnPropertyChanged();
		}
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public List<string> GlossaryNames { get; set; }
	public List<int> LanguageIndices { get; set; }

	public List<DbTag> Tags
	{
		get => _tags;
		set
		{
			if (Equals(value, _tags))
				return;

			_tags = value;
			OnPropertyChanged();
		}
	}

	public void Clear(object parameter)
	{
		Filepath = null;
	}

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	private void ChooseFilePath(object obj)
	{
		Filepath = OpenFileDialog.GetFilePath();
	}
}