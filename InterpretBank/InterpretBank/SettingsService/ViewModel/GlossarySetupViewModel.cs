using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using DocumentFormat.OpenXml.Wordprocessing;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.SettingsService.ViewModel.Interface;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.SettingsService.ViewModel
{
	public class GlossarySetupViewModel : ViewModel
	{
		public IOpenFileDialog OpenFileDialog { get; }
		private ICommand _deleteTagCommand;
		private ICommand _enterGlossaryCommand;
		private ICommand _enterTagCommand;
		private List<GlossaryModel> _glossaries;
		private List<GlossaryModel> _glossariesTaggedWithSelected;
		private List<Language> _languages;
		private TagModel _selectedTag;
		private List<TagModel> _tags;
		private ICommand _chooseFilepathCommand;
		private string _filepath;
		private ICommand _saveCommand;
		private List<LanguageModelsListBoxItem> _selectedLanguages;

		public GlossarySetupViewModel(IOpenFileDialog openFileDialog)
		{
			OpenFileDialog = openFileDialog;
			PropertyChanged += SettingsService_PropertyChanged;


		}

		public List<LanguageModelsListBoxItem> SelectedLanguages
		{
			get => _selectedLanguages;
			set => SetField(ref _selectedLanguages, value);
		}

		public ICommand DeleteTagCommand => _deleteTagCommand ??= new RelayCommand(DeleteTag);
		public ICommand ChooseFilePathCommand => _chooseFilepathCommand ??= new RelayCommand(ChooseFilePath);

		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save, o => !string.IsNullOrWhiteSpace(Filepath));

		private void ChooseFilePath(object obj)
		{
			var filePath = OpenFileDialog.GetFilePath();

			if (string.IsNullOrWhiteSpace(filePath))
				return;
			Filepath = filePath;
		}

		public string Filepath
		{
			get => _filepath;
			set => SetField(ref _filepath, value);
		}

		public ICommand EnterGlossaryCommand => _enterGlossaryCommand ??= new RelayCommand(EnterGlossary);

		public ICommand EnterTagCommand => _enterTagCommand ??= new RelayCommand(EnterTag);

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

		public List<GlossaryModel> GlossariesTaggedWithSelected
		{
			get => _glossariesTaggedWithSelected;
			set => SetField(ref _glossariesTaggedWithSelected, value);
		}

		public GlossaryModel GlossarySetupSelectedGlossary { get; set; }

		public List<Language> Languages
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

		public string Name => "Tag/set up glossaries";
		private void Setup()
		{
			Tags = InterpretBankDataContext.GetTags();
			Glossaries = InterpretBankDataContext.GetGlossaries();

			Languages = InterpretBankDataContext.GetLanguages();
			TagLinks = InterpretBankDataContext.GetLinks();

			var tagGroup = new TagsGroup(1, "All tags");
			Tags.ForEach(t => t.Group = tagGroup);

			var allLanguages = Constants.Languages.LanguagesList;

			SelectedLanguages = new List<LanguageModelsListBoxItem>(10);
			for (var i = 0; i < 10; i++)
			{
				SelectedLanguages.Add(new LanguageModelsListBoxItem
				{
					LanguageModels = allLanguages.Select(l => new LanguageModel { Name = l }).ToList()
				});
			}
			SelectedLanguages.ForEach(si => si.SelectedIndex = 0);

			foreach (var language in Languages)
			{
				var selectedIndex = allLanguages.IndexOf(language.Name);
				SelectedLanguages[language.Index - 1].SelectedIndex = selectedIndex;
				SelectedLanguages[language.Index - 1].IsEditable = false;
			}

			SelectedLanguages.ForEach(sl => sl.PropertyChanged += LanguageListBoxItem_SelectedLanguagesChanged);
		}

		private void LanguageListBoxItem_SelectedLanguagesChanged(object sender, PropertyChangedEventArgs e)
		{
			var selectedIndexName = nameof(LanguageModelsListBoxItem.SelectedIndex);

			if (e.PropertyName != selectedIndexName ||
			    sender is not LanguageModelsListBoxItem languageModelsListBoxItem)
				return;

			var newLanguage = new Language
			{
				Index = SelectedLanguages.IndexOf(languageModelsListBoxItem) + 1,
				Name = Constants.Languages.LanguagesList[languageModelsListBoxItem.SelectedIndex]
			};

			var duplicates = SelectedLanguages.GroupBy(sl => sl.SelectedIndex).Where(g => g.Count() > 1 && g.Key != 0).SelectMany(gr => gr).ToList();


			var infiniteLoopFlag = true;
			duplicates.ForEach(d =>
			{
				if (d[selectedIndexName] != "Duplicate value") infiniteLoopFlag = false;
				d[selectedIndexName] = "Duplicate value";
			});

			var nonDuplicates = SelectedLanguages.Except(duplicates).ToList();
			nonDuplicates.ForEach(nd =>
			{
				if (nd[selectedIndexName] != null) infiniteLoopFlag = false;
				nd[selectedIndexName] = null;
			});

			if (!infiniteLoopFlag)
				duplicates.Concat(nonDuplicates).ToList().ForEach(l => l.OnPropertyChanged(selectedIndexName));

			//var duplicateIndexes = GetSelectedLanguagesDuplicates(languageModelsListBoxItem);
			if (nonDuplicates.Contains(languageModelsListBoxItem))
			{
				Languages.Add(newLanguage);
				InterpretBankDataContext.InsertLanguage(newLanguage);
			}
			//else
			//{
			//	duplicateIndexes.ForEach(di =>
			//	{
			//		SelectedLanguages[di][nameof(languageModelsListBoxItem.SelectedIndex)] = "Duplicate value";
			//	});
			//}
		}

		private List<int> GetSelectedLanguagesDuplicates(LanguageModelsListBoxItem languageModelsListBoxItem)
		{
			var indexes = new List<int>();
			SelectedLanguages.Select((item, index) => (item, index)).ToList().ForEach(sl =>
			{
				if (sl.item.SelectedIndex == languageModelsListBoxItem.SelectedIndex &&
					sl.item.LanguageModels[sl.item.SelectedIndex].Name != "undef")
					indexes.Add(sl.index);
			});

			return indexes.Count > 1 ? indexes : null;
		}

		public TagModel SelectedTag
		{
			get => _selectedTag;
			set => SetField(ref _selectedTag, value);
		}

		public List<TagLinkModel> TagLinks { get; set; }

		public List<TagModel> Tags
		{
			get => _tags;
			set => SetField(ref _tags, value);
		}

		public IInterpretBankDataContext InterpretBankDataContext { get; set; }

		private void DeleteTag(object parameter)
		{
			if (parameter is not string tagName)
				return;

			Tags.Remove(Tags.Single(t => t.TagName == tagName));
			InterpretBankDataContext.RemoveTag(tagName);
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

		//private void EnterLanguage(object parameter)
		//{
		//	if (string.IsNullOrEmpty((string)parameter))
		//		return;

		//	var newTag = new LanguageModel() { TagName = (string)parameter };
		//	Tags.Add(newTag);

		//	InterpretBankDataContext.InsertTag(newTag);
		//}

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

			if (e.PropertyName == nameof(Filepath) && !string.IsNullOrWhiteSpace(Filepath))
			{
				InterpretBankDataContext.Setup(Filepath);
				Setup();
			}
		}
	}
}