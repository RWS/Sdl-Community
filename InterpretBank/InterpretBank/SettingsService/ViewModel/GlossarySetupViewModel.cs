using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.Wrappers.Interface;

namespace InterpretBank.SettingsService.ViewModel
{
	public class GlossarySetupViewModel : ViewModelBase.ViewModel
	{
		private ICommand _deleteTagCommand;
		private ICommand _enterGlossaryCommand;
		private ICommand _enterTagCommand;
		private string _filepath;
		private List<GlossaryModel> _glossaries;
		private List<GlossaryModel> _glossariesTaggedWithSelected;
		private List<LanguageModel> _languages;
		private ICommand _saveCommand;
		private GlossaryModel _selectedGlossary;
		private List<LanguageModelsListBoxItem> _selectedLanguages;
		private TagModel _selectedTag;
		private List<TagLinkModel> _tagLinks;
		private List<TagModel> _tags;

		public GlossarySetupViewModel(IDialog openFileDialog)
		{
			OpenFileDialog = openFileDialog;
		}

		public ICommand DeleteTagCommand => _deleteTagCommand ??= new RelayCommand(DeleteTag);
		public ICommand EnterGlossaryCommand => _enterGlossaryCommand ??= new RelayCommand(EnterGlossary);
		public ICommand EnterTagCommand => _enterTagCommand ??= new RelayCommand(EnterTag);

		public string Filepath
		{
			get => _filepath;
			set
			{
				SetField(ref _filepath, value);

				InterpretBankDataContext.Setup(Filepath);
				Setup();
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
					glossaryModel.Tags.CollectionChanged += Glossary_TagCollectionChanged;
					glossaryModel.Languages.CollectionChanged += Glossary_LanguageCollectionChanged;
				}

				_glossaries = value;
				OnPropertyChanged();
			}
		}

		public List<GlossaryModel> GlossariesTaggedWithSelected
		{
			get
			{
				var idsOfTaggedGlossaries = TagLinks?.Where(tl => tl.TagName == SelectedTag.TagName).Select(tl => tl.GlossaryId);

				if (idsOfTaggedGlossaries is null) return null;

				_glossariesTaggedWithSelected = Glossaries?.Where(gl => idsOfTaggedGlossaries.Contains(gl.Id)).ToList();
				return _glossariesTaggedWithSelected;
			}
			set => SetField(ref _glossariesTaggedWithSelected, value);
		}

		public IInterpretBankDataContext InterpretBankDataContext { get; set; }

		public List<LanguageModel> Languages
		{
			get => _languages;
			set => SetField(ref _languages, value);
		}

		public string Name => "Tag/set up glossaries";

		public IDialog OpenFileDialog { get; }
		public ICommand SaveCommand => _saveCommand ??= new RelayCommand(Save, o => !string.IsNullOrWhiteSpace(Filepath));

		public GlossaryModel SelectedGlossary
		{
			get => _selectedGlossary;
			set => SetField(ref _selectedGlossary, value);
		}

		public List<LanguageModelsListBoxItem> SelectedLanguages
		{
			get => _selectedLanguages;
			set => SetField(ref _selectedLanguages, value);
		}

		public TagModel SelectedTag
		{
			get => _selectedTag;
			set
			{
				SetField(ref _selectedTag, value);
				OnPropertyChanged(nameof(GlossariesTaggedWithSelected));
			}
		}

		public List<TagLinkModel> TagLinks
		{
			get => _tagLinks;
			set => SetField(ref _tagLinks, value);
		}

		public List<TagModel> Tags
		{
			get => _tags;
			set => SetField(ref _tags, value);
		}

		private List<LanguageModelsListBoxItem> CheckForDuplicates(string selectedIndexName)
		{
			var duplicates = SelectedLanguages.GroupBy(sl => sl.SelectedIndex).Where(g => g.Count() > 1 && g.Key != 0)
				.SelectMany(gr => gr).ToList();

			var infiniteLoopFlag = true;
			duplicates.ForEach(d =>
			{
				if (d[selectedIndexName] != "Duplicate value")
					infiniteLoopFlag = false;
				d[selectedIndexName] = "Duplicate value";
			});

			var nonDuplicates = SelectedLanguages.Except(duplicates).ToList();
			nonDuplicates.ForEach(nd =>
			{
				if (nd[selectedIndexName] != null)
					infiniteLoopFlag = false;
				nd[selectedIndexName] = null;
			});

			if (!infiniteLoopFlag)
				duplicates.Concat(nonDuplicates).ToList().ForEach(l => l.OnPropertyChanged(selectedIndexName));
			return nonDuplicates;
		}

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

		private void Glossary_LanguageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					if (e.NewItems[0] is LanguageModel newLanguage)
					{
						InterpretBankDataContext.AddLanguageToGlossary(newLanguage, SelectedGlossary.GlossaryName);
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void Glossary_TagCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
					{
						if (e.OldItems[0] is TagModel removedTag)
						{
							InterpretBankDataContext.RemoveTagFromGlossary(removedTag.TagName,
								SelectedGlossary.GlossaryName);
							TagLinks.RemoveAll(tl =>
								tl.TagName == removedTag.TagName && tl.GlossaryId == SelectedGlossary.Id);
						}
						break;
					}
				case NotifyCollectionChangedAction.Add:
					{
						var newItem = e.NewItems[0];
						if (newItem is TagModel newTag)
						{
							InterpretBankDataContext.TagGlossary(newTag, SelectedGlossary.GlossaryName);
							TagLinks.Add(new TagLinkModel
							{
								GlossaryId = SelectedGlossary.Id,
								TagName = newTag.TagName
							});
						}
						break;
					}
			}
			OnPropertyChanged(nameof(GlossariesTaggedWithSelected));
		}

		private void LanguageListBoxItem_SelectedLanguagesChanged(object sender, PropertyChangedEventArgs e)
		{
			var selectedIndexName = nameof(LanguageModelsListBoxItem.SelectedIndex);

			if (e.PropertyName != selectedIndexName ||
				sender is not LanguageModelsListBoxItem languageModelsListBoxItem)
				return;

			var newLanguage = new LanguageModel
			{
				Index = SelectedLanguages.IndexOf(languageModelsListBoxItem) + 1,
				Name = Constants.Languages.LanguagesList[languageModelsListBoxItem.SelectedIndex]
			};

			var nonDuplicates = CheckForDuplicates(selectedIndexName);

			if (nonDuplicates.Contains(languageModelsListBoxItem))
			{
				Languages.Add(newLanguage);
				InterpretBankDataContext.InsertLanguage(newLanguage);
			}
		}

		private void Save(object parameter)
		{
			InterpretBankDataContext.SubmitData();
		}

		private void Setup()
		{
			Tags = InterpretBankDataContext.GetTags().Distinct().ToList();
			Languages = InterpretBankDataContext.GetDbLanguages();
			Glossaries = InterpretBankDataContext.GetGlossaries();
			TagLinks = InterpretBankDataContext.GetLinks();

			var languageGroup = new LanguageGroup(1, "All languages");
			Languages.ForEach(l => l.Group = languageGroup);

			var tagGroup = new TagsGroup(1, "All tags");
			Tags.ForEach(t => t.Group = tagGroup);

			SetupSelectedLanguages();
		}

		private void SetupSelectedLanguages()
		{
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
	}
}