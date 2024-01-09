using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.TerminologyService;

namespace InterpretBank.SettingsService.ViewModel;

public class SettingsService(IInterpretBankDataContext interpretBankDataContext)
    : ViewModelBase.ViewModel, IDataErrorInfo
{
	private string _filepath;
	private List<GlossaryModel> _glossaries;
	private ICommand _saveCommand;
	private ObservableCollection<GlossaryModel> _selectedGlossaries = new();
	private ObservableCollection<TagModel> _selectedTags = new();
	private List<TagModel> _tags;

    public string Filepath
	{
		get => _filepath;
		set
		{
			var result = SetField(ref _filepath, value);
			Setup(result);
		}
	}

	public List<GlossaryModel> Glossaries
	{
		get => _glossaries;
		set => SetField(ref _glossaries, value);
	}

	public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save, o => !string.IsNullOrWhiteSpace(Filepath) && this["SelectedTags"] == "" && this["SelectedGlossaries"] == "");

	public ObservableCollection<GlossaryModel> SelectedGlossaries
	{
		get => _selectedGlossaries;
		set
		{
			SetField(ref _selectedGlossaries, value);
			_selectedGlossaries.CollectionChanged += (s, e) =>
			{
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectedTags));
			};
		}
	}

	public ObservableCollection<TagModel> SelectedTags
	{
		get => _selectedTags;
		set
		{
			SetField(ref _selectedTags, value);
            //Add tagged glossaries to the SelectedGlossaries
			_selectedTags.CollectionChanged += (s, e) =>
			{
				OnPropertyChanged();
				OnPropertyChanged(nameof(SelectedGlossaries));
			};
		}
	}

	public Settings Settings
	{
		set
		{
			SettingsId = value.SettingsId;
			Filepath = value.DatabaseFilepath;
			if (value.Glossaries is not null)
				SelectedGlossaries = new ObservableCollection<GlossaryModel>(Glossaries?.Where(g => value.Glossaries.Contains(g.GlossaryName)));

			if (value.Tags is not null)
				SelectedTags = new ObservableCollection<TagModel>(Tags?.Where(t => value.Tags.Contains(t.TagName)));
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

	private IInterpretBankDataContext InterpretBankDataContext { get; set; } = interpretBankDataContext;

    public void Dispose()
	{
		InterpretBankDataContext?.Dispose();
	}

	private void Save(object parameter)
	{
		if (Error != "") return;
		InterpretBankDataContext.SubmitData();
	}

	private void Setup(bool reset = false)
	{
		InterpretBankDataContext?.Dispose();
		if (!string.IsNullOrWhiteSpace(_filepath))
		{
			InterpretBankDataContext.Setup(Filepath);
			Tags = InterpretBankDataContext.GetTags().Distinct().ToList();
			Glossaries = InterpretBankDataContext.GetGlossaries();
		}
		else
		{
			Tags = null;
			Glossaries = null;
		}

		if (!reset)
			return;

		SelectedGlossaries.Clear();
		SelectedTags.Clear();
	}

	public string this[string columnName] {
		get
		{
			if (columnName is nameof(SelectedTags) or nameof(SelectedGlossaries))
			{
				if (!SelectedTags.Any() && !SelectedGlossaries.Any())
				{
					return "Please select some tags or glossaries";
				}

				
			}

			return string.Empty;
		}
	}

	public string Error { get; set; }

	public void Validate()
	{
		Error = !SelectedTags.Any() && !SelectedGlossaries.Any() ? "Please select some tags or glossaries" : "";
		OnPropertyChanged(Error);
	}

    public void Setup(string dbFilepath)
    {
        _filepath = dbFilepath;
    }
}