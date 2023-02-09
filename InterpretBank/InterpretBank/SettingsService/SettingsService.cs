using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.DAL;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.TerminologyService;
using InterpretBank.Wrappers.Interface;
using Sdl.MultiSelectComboBox.EventArgs;

namespace InterpretBank.SettingsService;

public class SettingsService : ISettingsService, INotifyPropertyChanged
{
	private ICommand _chooseFilePathCommand;
	private ICommand _clearCommand;
	private ICommand _deleteTagCommand;
	private ICommand _enterGlossaryCommand;
	private ICommand _enterTagCommand;
	private string _filepath;
	private List<GlossaryModel> _glossaries;

	private List<GlossaryModel> _glossariesTaggedWithSelected;
	private List<LanguageModel> _languages;
	private ICommand _saveCommand;

	private TagModel _selectedTag;
	private List<TagModel> _tags;
	private ICommand _selectedItemsChangedCommand;

	public SettingsService(IOpenFileDialog openFileDialog, IInterpretBankDataContext interpretBankDataContext)
	{
		OpenFileDialog = openFileDialog;
		InterpretBankDataContext = interpretBankDataContext;

		var tagGroup = new TagsGroup(1, "All tags");

		Tags = InterpretBankDataContext.GetTags();

		Tags.ForEach(t => t.Group = tagGroup);

		Glossaries = InterpretBankDataContext.GetGlossaries();



		TagLinks = InterpretBankDataContext.GetLinks();
		Languages = InterpretBankDataContext.GetLanguages().Select(l => new LanguageModel { Name = l.Name }).ToList();

		PropertyChanged += SettingsService_PropertyChanged;
	}

	public event PropertyChangedEventHandler PropertyChanged;

	public ICommand ChooseFilePathCommand => _chooseFilePathCommand ??= new RelayCommand(ChooseFilePath);

	public ICommand ClearCommand => _clearCommand ??= new RelayCommand(Clear);

	public ICommand DeleteTagCommand => _deleteTagCommand ??= new RelayCommand(DeleteTag);

	public ICommand EnterGlossaryCommand => _enterGlossaryCommand ??= new RelayCommand(EnterGlossary);

	public ICommand EnterTagCommand => _enterTagCommand ??= new RelayCommand(EnterTag);

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

	public List<GlossaryModel> Glossaries
	{
		get => _glossaries;
		set
		{
			if (Equals(value, _glossaries))
				return;

			foreach (var glossaryModel in value)
			{
				var glossaryTags = glossaryModel.Tags.Select(t => t.TagName);
				glossaryModel.Tags = new ObservableCollection<TagModel>(Tags.Where(t => glossaryTags.Contains(t.TagName)).ToList());

				glossaryModel.Tags.CollectionChanged += GlossarySetup_TagCollectionChanged;
			}

			_glossaries = value;
			OnPropertyChanged();
		}
	}

	private void GlossarySetup_TagCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
	{
		switch (e.Action)
		{
			case NotifyCollectionChangedAction.Remove:
				{
					if (e.OldItems[0] is TagModel oldItems)
						InterpretBankDataContext.RemoveTagFromGlossary(oldItems.TagName, GlossarySetupSelectedGlossary.GlossaryName);
					break;
				}
			case NotifyCollectionChangedAction.Add:
				{
					var newItems = e.NewItems;
					break;
				}
		}
	}

	public List<GlossaryModel> GlossariesTaggedWithSelected
	{
		get => _glossariesTaggedWithSelected;
		set
		{
			if (Equals(value, _glossariesTaggedWithSelected))
				return;
			_glossariesTaggedWithSelected = value;
			OnPropertyChanged();
		}
	}

	public List<string> GlossaryNames { get; set; }

	public List<LanguageModel> Languages
	{
		get => _languages;
		set
		{
			if (Equals(value, _languages))
				return;
			_languages = value;
			OnPropertyChanged();
		}
	}

	public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save);

	public TagModel SelectedTag
	{
		get => _selectedTag;
		set
		{
			if (Equals(value, _selectedTag))
				return;
			_selectedTag = value;
			OnPropertyChanged();
		}
	}

	public List<DbTag> SelectedTags { get; set; }

	public List<TagLinkModel> TagLinks { get; set; }

	public List<TagModel> Tags
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

	private IInterpretBankDataContext InterpretBankDataContext { get; }

	private IOpenFileDialog OpenFileDialog { get; }

	public ICommand SelectedItemsChangedCommand =>
		_selectedItemsChangedCommand ??= new RelayCommand(SelectedItemsChanged);

	public GlossaryModel GlossarySetupSelectedGlossary { get; set; }

	private void SelectedItemsChanged(object parameter)
	{
		//InterpretBankDataContext.TagGlossary()
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

	private void DeleteTag(object parameter)
	{
		if (parameter is string tagName)
		{
			Tags.Remove(Tags.Single(t => t.TagName == tagName));
			InterpretBankDataContext.RemoveTag(tagName);
		}
	}

	private void EnterGlossary(object parameter)
	{
		if (string.IsNullOrEmpty((string)parameter))
			return;

		var newGlossary = new GlossaryModel { GlossaryName = (string)parameter };
		Glossaries.Add(newGlossary);

		InterpretBankDataContext.InsertGlossary(newGlossary);
	}

	private void EnterTag(object parameter)
	{
		if (string.IsNullOrEmpty((string)parameter))
			return;

		var newTag = new TagModel { TagName = (string)parameter };
		Tags.Add(newTag);

		InterpretBankDataContext.InsertTag(newTag);
	}

	private void Save(object parameter)
	{
		InterpretBankDataContext.SubmitData();
	}

	private void SettingsService_PropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(SelectedTag) && SelectedTag is not null)
		{
			var glossaryIdsOfTagged = TagLinks.Where(tl => tl.TagName == SelectedTag.TagName).Select(tl => tl.GlossaryId);
			GlossariesTaggedWithSelected = Glossaries.Where(gl => glossaryIdsOfTagged.Contains(gl.Id)).ToList();
		}
	}
}