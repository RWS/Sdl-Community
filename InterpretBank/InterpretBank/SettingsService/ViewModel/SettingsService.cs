using System.Collections.Generic;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.SettingsService.ViewModel.Interface;
using InterpretBank.TerminologyService;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.SettingsService.ViewModel;

public class SettingsService : ViewModel, ISettingsService
{
	private RelayCommand _chooseFilePathCommand;
	private string _filepath;
	private ICommand _saveCommand;

	public SettingsService(IOpenFileDialog openFileDialog, IInterpretBankDataContext interpretBankDataContext)
	{
		InterpretBankDataContext = interpretBankDataContext;
		OpenFileDialog = openFileDialog;

		ViewModels = new List<ISubViewModel>
		{
			new SettingsViewModel(),
			new SetupGlossariesViewModel(interpretBankDataContext)
		};

		PropertyChanged += SettingsService_PropertyChanged;
	}

	private void Setup()
	{
		InterpretBankDataContext?.Dispose();
		if (_filepath != null)
		{
			InterpretBankDataContext.Setup(Filepath);
			Tags = InterpretBankDataContext.GetTags();
			Glossaries = InterpretBankDataContext.GetGlossaries();
		}
		else
		{
			Tags = null;
			Glossaries = null;
		}
	}

	private void SettingsService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Filepath))
		{
			Setup();
			ViewModels.ForEach(vm => vm.Setup(Glossaries, Tags));
		}
	}

	public ICommand ChooseFilePathCommand => _chooseFilePathCommand ??= new RelayCommand(ChooseFilePath);
	private IOpenFileDialog OpenFileDialog { get; }

	public string Filepath
	{
		get => _filepath;
		set => SetField(ref _filepath, value);
	}



	public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);
	public List<ISubViewModel> ViewModels { get; set; }
	private List<GlossaryModel> Glossaries { get; set; }

	private IInterpretBankDataContext InterpretBankDataContext { get; }
	private List<TagModel> Tags { get; set; }

	public void Dispose()
	{
		InterpretBankDataContext?.Dispose();
	}

	private void ChooseFilePath(object obj)
	{
		var filePath = OpenFileDialog.GetFilePath();

		if (string.IsNullOrWhiteSpace(filePath)) return;
		Filepath = filePath;
	}

	private void Save(object parameter)
	{
		InterpretBankDataContext.SubmitData();
	}
}