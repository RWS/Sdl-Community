using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.Model;
using InterpretBank.SettingsService.ViewModel.Interface;

namespace InterpretBank.SettingsService.ViewModel
{
	public class SetupGlossariesViewModel : ViewModel, ISubViewModel
	{
		private ICommand _deleteTagCommand;
		private ICommand _enterGlossaryCommand;
		private ICommand _enterTagCommand;
		private List<GlossaryModel> _glossaries;
		private List<GlossaryModel> _glossariesTaggedWithSelected;
		private List<LanguageModel> _languages;
		private TagModel _selectedTag;
		private List<TagModel> _tags;

		public SetupGlossariesViewModel(IInterpretBankDataContext interpretBankDataContext, List<GlossaryModel> glossaries, List<TagModel> tags)
		{
			InterpretBankDataContext = interpretBankDataContext;
			Languages = InterpretBankDataContext.GetLanguages().Select(l => new LanguageModel { Name = l.Name }).ToList();

			var tagGroup = new TagsGroup(1, "All tags");
			Tags = tags;
			Tags.ForEach(t => t.Group = tagGroup);

			Glossaries = glossaries;
			TagLinks = InterpretBankDataContext.GetLinks();

			PropertyChanged += SettingsService_PropertyChanged;
		}

		public ICommand DeleteTagCommand => _deleteTagCommand ??= new RelayCommand(DeleteTag);

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

		public string Name => "Tag/set up glossaries";

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

		private IInterpretBankDataContext InterpretBankDataContext { get; }

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

		private void SettingsService_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(SelectedTag) && SelectedTag is not null)
			{
				var glossaryIdsOfTagged = TagLinks.Where(tl => tl.TagName == SelectedTag.TagName).Select(tl => tl.GlossaryId);
				GlossariesTaggedWithSelected = Glossaries.Where(gl => glossaryIdsOfTagged.Contains(gl.Id)).ToList();
			}
		}
	}
}