using InterpretBank.Commands;
using InterpretBank.Interface;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ExchangeService = InterpretBank.GlossaryExchangeService.GlossaryExchangeService;

namespace InterpretBank.SettingsService.ViewModel;

public class GlossarySetupViewModel(
    IUserInteractionService userInteractionService,
    ExchangeService exchangeService,
    IInterpretBankDataContext interpretBankDataContext)
    : NotifyChangeModel
{
    private ICommand _deleteTagCommand;
    private ICommand _enterGlossaryCommand;
    private ICommand _enterTagCommand;
    private string _filepath;
    private ObservableCollection<object> _glossaries;
    private ObservableCollection<object> _languages;
    private object _selectedGlossary;
    private ObservableCollection<object> _selectedGlossaryTags;
    private List<LanguageModelsListBoxItem> _selectedLanguages;
    private TagModel _selectedTag;
    private ObservableCollection<object> _selectedTagGlossaries;
    private ObservableCollection<TagLinkModel> _tagLinks;
    private ObservableCollection<object> _tags;
    public ICommand DeleteGlossaryCommand => new RelayCommand(DeleteGlossary);
    public ICommand DeleteTagCommand => _deleteTagCommand ??= new RelayCommand(DeleteTag);
    public ICommand EnterGlossaryCommand => _enterGlossaryCommand ??= new RelayCommand(EnterGlossary);
    public ICommand EnterTagCommand => _enterTagCommand ??= new RelayCommand(EnterTag);

    public string Filepath
    {
        get => _filepath;
        set
        {
            SetField(ref _filepath, value);

            ClearControl();
            InterpretBankDataContext.Setup(Filepath);

            if (InterpretBankDataContext.IsValid) Setup();

            OnPropertyChanged(nameof(IsDbValid));
        }
    }

    public ObservableCollection<object> Glossaries
    {
        get => _glossaries;
        set
        {
            if (value == null)
            {
                foreach (var glossaryModel in _glossaries)
                    DetachFromEventsOfGlossaryModel((GlossaryModel)glossaryModel);
                return;
            }

            foreach (var glossaryModel in value) AttachToEventsOfGlossaryModel((GlossaryModel)glossaryModel);

            SetField(ref _glossaries, value);
        }
    }

    public ICommand ImportIntoSelectedGlossaryCommand =>
        new RelayCommand(ImportIntoSelectedGlossary, o => SelectedGlossary != null);

    public IInterpretBankDataContext InterpretBankDataContext { get; set; } = interpretBankDataContext;

    public bool IsDbValid => InterpretBankDataContext.IsValid;

    public ObservableCollection<object> Languages
    {
        get => _languages;
        set => SetField(ref _languages, value);
    }

    public string Name => "Tag/set up glossaries";

    public GlossaryModel SelectedGlossary
    {
        get => (GlossaryModel)_selectedGlossary;
        set
        {
            SetField(ref _selectedGlossary, value);
            OnPropertyChanged(nameof(SelectedGlossaryTags));
        }
    }

    public ObservableCollection<object> SelectedGlossaryTags
    {
        get
        {
            if (SelectedGlossary is null) return _selectedGlossaryTags;

            if (_selectedGlossaryTags is not null)
                _selectedGlossaryTags.CollectionChanged -= TagLinks_CollectionChanged;

            var tagNamesSelectedGlossary =
                TagLinks?.Where(tl => tl.GlossaryId == SelectedGlossary.Id).Select(tl => tl.TagName);

            _selectedGlossaryTags =
                new ObservableCollection<object>(
                    ((tagNamesSelectedGlossary is not null && tagNamesSelectedGlossary.Any())
                        ? Tags.Cast<TagModel>().Where(t => tagNamesSelectedGlossary.Contains(t.TagName)).ToList()
                        : []).OrderBy(t=>t.TagName));

            _selectedGlossaryTags.CollectionChanged += TagLinks_CollectionChanged;
            return _selectedGlossaryTags;
        }
        set => SetField(ref _selectedGlossaryTags, value);
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
            OnPropertyChanged(nameof(SelectedTagGlossaries));
        }
    }

    public ObservableCollection<object> SelectedTagGlossaries
    {
        get
        {
            if (_selectedTagGlossaries is not null) _selectedTagGlossaries.CollectionChanged -= TagLinks_CollectionChanged;

            var idsOfTaggedGlossaries = TagLinks?.Where(tl => tl.TagName == SelectedTag?.TagName).Select(tl => tl.GlossaryId);
            if (idsOfTaggedGlossaries is null) return null;

            _selectedTagGlossaries = new ObservableCollection<object>(Glossaries?.Cast<GlossaryModel>()
                .Where(gl => idsOfTaggedGlossaries.Contains(gl.Id)).OrderBy(g => g.GlossaryName)
                .ThenBy(g => g.SubGlossaryName).ToList());
            _selectedTagGlossaries.CollectionChanged += TagLinks_CollectionChanged;

            return _selectedTagGlossaries;
        }
        set => SetField(ref _selectedTagGlossaries, value);
    }

    public ObservableCollection<TagLinkModel> TagLinks
    {
        get => _tagLinks;
        set => SetField(ref _tagLinks, value);
    }

    public ObservableCollection<object> Tags
    {
        get => _tags;
        set => SetField(ref _tags, value);
    }

    private ExchangeService ExchangeService { get; } = exchangeService;

    private IUserInteractionService UserInteractionService { get; } = userInteractionService;

    public void Setup()
    {
        SelectedGlossary = null;

        TagLinks = new ObservableCollection<TagLinkModel>(InterpretBankDataContext.GetLinks());
        Glossaries = new ObservableCollection<object>(InterpretBankDataContext.GetGlossaries().OrderBy(g=>g.GlossaryName).ThenBy(g=>g.SubGlossaryName));
        Tags = new ObservableCollection<object>(InterpretBankDataContext.GetTags().Distinct().OrderBy(t=>t.TagName).ToList());
        Languages = new ObservableCollection<object>(InterpretBankDataContext.GetDbLanguages().OrderBy(l=>l.Name));

        OnPropertyChanged(nameof(SelectedGlossaryTags));
        OnPropertyChanged(nameof(SelectedTagGlossaries));

        SetupSelectedLanguages();
    }

    private void AttachToEventsOfGlossaryModel(GlossaryModel glossaryModel)
    {
        glossaryModel.Languages.CollectionChanged += Glossary_LanguageCollectionChanged;
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

    private void ClearControl()
    {
        SelectedGlossary = new GlossaryModel();
        Tags = [];
        Glossaries = [];
        Languages = [];
    }

    private void DeleteGlossary(object obj)
    {
        if (obj is not GlossaryModel glossary) return;

        if (!UserInteractionService.Confirm($@"Are you sure you want to delete the glossary ""{glossary.GlossaryName}""?")) return;
        InterpretBankDataContext.RemoveGlossary(glossary.Id);
        Glossaries.Remove(glossary);

        SelectedGlossary = Glossaries.Cast<GlossaryModel>().ToList()[0];

        OnPropertyChanged(nameof(SelectedTagGlossaries));
    }

    private void DeleteTag(object parameter)
    {
        if (parameter is not TagModel tag)
            return;

        if (!UserInteractionService.Confirm($@"Are you sure you want to delete the tag ""{tag.TagName}""?")) return;

        Tags.Remove(Tags.Cast<TagModel>().Single(t => t.TagName == tag.TagName));
        InterpretBankDataContext.RemoveTag(tag.TagName);
        OnPropertyChanged(nameof(SelectedGlossaryTags));
    }

    private void DetachFromEventsOfGlossaryModel(GlossaryModel glossaryModel)
    {
        glossaryModel.Languages.CollectionChanged -= Glossary_LanguageCollectionChanged;
    }

    private void EnterGlossary(object parameter)
    {
        var glossaryName = UserInteractionService.GetInfoFromUser(PluginResources.Message_TypeNameOfNewGlossary);

        if (string.IsNullOrEmpty(glossaryName)) return;

        var newGlossary = new GlossaryModel { GlossaryName = glossaryName };

        if (Glossaries.Cast<GlossaryModel>().Any(g => g.GlossaryName == glossaryName))
        {
            UserInteractionService.WarnUser(PluginResources.Message_GlossaryAlreadyExists);
            return;
        }

        Glossaries.Add(newGlossary);
        AttachToEventsOfGlossaryModel(newGlossary);

        var id = InterpretBankDataContext.InsertGlossary(newGlossary);
        newGlossary.Id = id;
    }

    private void EnterTag(object parameter)
    {
        var tagName = UserInteractionService.GetInfoFromUser(PluginResources.Message_TypeNameOfNewTag);

        if (string.IsNullOrEmpty(tagName)) return;

        var newTag = new TagModel { TagName = tagName };

        if (Tags.Cast<TagModel>().Any(t => t.TagName == newTag.TagName))
        {
            UserInteractionService.WarnUser(PluginResources.Message_TagAlreadyExists);
            return;
        }
        Tags.Add(newTag);
        SelectedTag = newTag;

        InterpretBankDataContext.InsertTag(newTag);
    }

    private void Glossary_LanguageCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems[0] is LanguageModel newLanguage)
                {
                    InterpretBankDataContext.AddLanguageToGlossary(newLanguage, SelectedGlossary?.GlossaryName);
                }
                break;
        }
    }

    private void ImportIntoSelectedGlossary(object obj)
    {
        if (!UserInteractionService.GetFilePath(out var filepath, "All Supported Formats (*.tbx;*.xlsx)|*.tbx;*.xlsx|TermBase eXchange (*.tbx)|*.tbx|Microsoft Excel spreadsheet (*.xlsx)|*.xlsx")) return;
        var importTerms = ExchangeService.ImportTerms(filepath).ToArray();

        var glossaryImport = new GlossaryImport(importTerms);

        InterpretBankDataContext.AddCompatibleLanguageEquivalentsFromImport(glossaryImport, SelectedGlossary?.GlossaryName);
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

        foreach (var language in Languages.Cast<LanguageModel>())
        {
            var selectedIndex = allLanguages.IndexOf(language.Name);
            SelectedLanguages[language.Index - 1].SelectedIndex = selectedIndex;
            SelectedLanguages[language.Index - 1].IsEditable = false;
        }

        SelectedLanguages.ForEach(sl => sl.PropertyChanged += LanguageListBoxItem_SelectedLanguagesChanged);
    }

    private void TagLinks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        var selectedGlossary = SelectedGlossary;
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add when e.NewItems[0] is TagModel tagModel:
                TagLinks.Add(new TagLinkModel
                {
                    GlossaryId = selectedGlossary.Id,
                    TagName = tagModel.TagName
                });
                InterpretBankDataContext.TagGlossary(tagModel, selectedGlossary.GlossaryName);
                if (tagModel.TagName == SelectedTag.TagName) OnPropertyChanged(nameof(SelectedTagGlossaries));

                break;

            case NotifyCollectionChangedAction.Add when e.NewItems[0] is GlossaryModel glossaryModel:
                TagLinks.Add(new TagLinkModel
                {
                    GlossaryId = glossaryModel.Id,
                    TagName = SelectedTag.TagName
                });
                InterpretBankDataContext.TagGlossary(SelectedTag, glossaryModel.GlossaryName);
                if (glossaryModel.GlossaryName == selectedGlossary.GlossaryName) OnPropertyChanged(nameof(SelectedGlossaryTags));
                break;

            case NotifyCollectionChangedAction.Remove when e.OldItems[0] is TagModel tagModel:
                var tagLinkRemoved = TagLinks.FirstOrDefault(tl => tl.TagName == tagModel.TagName && tl.GlossaryId == selectedGlossary.Id);
                TagLinks.Remove(tagLinkRemoved);
                InterpretBankDataContext.RemoveTagFromGlossary(tagModel.TagName, selectedGlossary.GlossaryName);
                if (tagModel.TagName == SelectedTag.TagName) OnPropertyChanged(nameof(SelectedTagGlossaries));
                break;

            case NotifyCollectionChangedAction.Remove when e.OldItems[0] is GlossaryModel glossaryModel:
                tagLinkRemoved = TagLinks.FirstOrDefault(tl => tl.TagName == SelectedTag.TagName && tl.GlossaryId == glossaryModel.Id);
                TagLinks.Remove(tagLinkRemoved);
                InterpretBankDataContext.RemoveTagFromGlossary(SelectedTag.TagName, glossaryModel.GlossaryName);
                if (glossaryModel.GlossaryName == selectedGlossary.GlossaryName) OnPropertyChanged(nameof(SelectedGlossaryTags));
                break;
        }
    }
}