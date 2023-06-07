using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	private List<GlossaryModel> _glossaries;
	private ICommand _saveCommand;
	private List<TagModel> _tags;
	private List<GlossaryModel> _selectedGlossaries = new();
	private IEnumerable<TagModel> _selectedTags;

	public SettingsService(IOpenFileDialog openFileDialog, IInterpretBankDataContext interpretBankDataContext)
	{
		InterpretBankDataContext = interpretBankDataContext;
		OpenFileDialog = openFileDialog;

		PropertyChanged += SettingsService_PropertyChanged;
	}

	public ICommand ChooseFilePathCommand => _chooseFilePathCommand ??= new RelayCommand(ChooseFilePath);

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

	public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save, o => !string.IsNullOrWhiteSpace(Filepath));

	public List<GlossaryModel> SelectedGlossaries
	{
		get => _selectedGlossaries;
		set
		{
			if (SetField(ref _selectedGlossaries, value))
			{
				OnPropertyChanged(nameof(Settings));
			}
		}
	}

	public IEnumerable<TagModel> SelectedTags
	{
		get => _selectedTags;
		set
		{
			if (SetField(ref _selectedTags, value))
			{
				OnPropertyChanged(nameof(Settings));
			}
		}
	}

	public Settings Settings
	{
		set
		{
			SettingsId = value.SettingsId;
			Filepath = value.DatabaseFilepath;
			if (value.Glossaries is not null)
				SelectedGlossaries = Glossaries?.Where(g => value.Glossaries.Contains(g.GlossaryName)).ToList();
			
			if (value.Tags is not null)
				SelectedTags = Tags?.Where(t => value.Tags.Contains(t.TagName)).ToList();
		}
		get =>
			new()
			{
				SettingsId = SettingsId,
				DatabaseFilepath = Filepath,
				Glossaries = SelectedGlossaries?.Select(g => g.GlossaryName).ToList(),
				Tags = SelectedTags?.Select(t => t.TagName).ToList()
			};
	}

	public string SettingsId { get; set; }

	public List<TagModel> Tags
	{
		get => _tags;
		set => SetField(ref _tags, value);
	}

	private IInterpretBankDataContext InterpretBankDataContext { get; }

	private IOpenFileDialog OpenFileDialog { get; }

	public void Dispose()
	{
		InterpretBankDataContext?.Dispose();
	}

	private void ChooseFilePath(object obj)
	{
		var filePath = OpenFileDialog.GetFilePath();

		if (string.IsNullOrWhiteSpace(filePath))
			return;
		Filepath = filePath;
	}

	private void Save(object parameter)
	{
		InterpretBankDataContext.SubmitData();
	}

	private void SettingsService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName != nameof(Filepath))
			return;

		Setup();

		//GlossarySetupViewModel.SetDataContext(InterpretBankDataContext);
		//GlossarySetupViewModel.Setup(Glossaries, Tags);
	}

	private void Setup()
	{
		InterpretBankDataContext?.Dispose();
		if (!string.IsNullOrWhiteSpace(_filepath))
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
}