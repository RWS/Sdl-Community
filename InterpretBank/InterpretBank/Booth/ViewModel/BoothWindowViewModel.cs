using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using InterpretBank.Helpers;
using InterpretBank.Model;
using InterpretBank.SettingsService.Model;
using InterpretBank.TerminologyService.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;

namespace InterpretBank.Booth.ViewModel
{
    public class BoothWindowViewModel : Model.NotifyChangeModel
    {
        private readonly ITerminologyService _terminologyService;
        private NotifyTaskCompletion<ObservableCollection<EntryModel>> _entries;
        private string _filepath;
        private List<GlossaryModel> _glossaries = new();
        private List<LanguageModel> _languages;
        private string _searchText;
        private ObservableCollection<GlossaryModel> _selectedGlossaries = new();
        private ObservableCollection<TagModel> _selectedTags = new();
        private string _sourceLanguage;
        private List<TagModel> _tags = new();
        private string _targetLanguage;
        private bool _useTags;

        public BoothWindowViewModel(ITerminologyService terminologyService)
        {
            _terminologyService = terminologyService;

            UseTags = true;
            AttachSelectedTagsChangedEventHandler();
        }

        public NotifyTaskCompletion<ObservableCollection<EntryModel>> Entries
        {
            get => _entries;
            set
            {
                SetField(ref _entries, value);
                Filter();
            }
        }

        public string Filepath
        {
            get => _filepath;
            set
            {
                SetField(ref _filepath, value);
                Setup();
                OnPropertyChanged(nameof(IsDbValid));
            }
        }

        public List<GlossaryModel> Glossaries
        {
            get => _glossaries;
            set => SetField(ref _glossaries, value);
        }

        public bool IsDbValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Filepath)) return false;
                try
                {
                    _terminologyService.GetGlossaries();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public List<LanguageModel> Languages
        {
            get => _languages;
            set => SetField(ref _languages, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetField(ref _searchText, value);
                Filter();
            }
        }

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
                };
            }
        }

        public string SourceLanguage
        {
            get => _sourceLanguage;
            set
            {
                SetField(ref _sourceLanguage, value);
                Filter();
            }
        }

        public List<TagModel> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }

        public string TargetLanguage
        {
            get => _targetLanguage;
            set
            {
                SetField(ref _targetLanguage, value);
                Filter();
            }
        }

        public bool UseTags
        {
            get => _useTags;
            set
            {
                SetField(ref _useTags, value);

                if (!IsDbValid) return;

                if (value)
                {
                    AttachSelectedTagsChangedEventHandler();
                }
                else
                {
                    SelectedTags.CollectionChanged -= OnSelectionChanged();
                    SelectedGlossaries.CollectionChanged -= OnSelectionChanged();
                    SelectedGlossaries.CollectionChanged += OnSelectionChanged();
                }

                SetupEntries();
            }
        }

        private static bool IsSet(string @string) => !string.IsNullOrWhiteSpace(@string);

        private void AttachSelectedTagsChangedEventHandler()
        {
            SelectedGlossaries.CollectionChanged -= OnSelectionChanged();
            SelectedTags.CollectionChanged -= OnSelectionChanged();
            SelectedTags.CollectionChanged += OnSelectionChanged();
        }

        private void Filter()
        {
            if (Entries == null) return;
            var collectionView = CollectionViewSource.GetDefaultView(Entries.Result);
            if (collectionView == null) return;

            collectionView.Filter = null;
            collectionView.Filter = entry =>
            {
                var entryModel = (EntryModel)entry;

                var terms = entryModel.Terms;

                var sourceTerms = IsSet(SourceLanguage) ? terms.Where(t => t.LanguageName == SourceLanguage) : null;
                var targetTerms = IsSet(TargetLanguage)
                    ? terms.Where(t => t.LanguageName == TargetLanguage)
                    : null;

                sourceTerms = IsSet(SearchText) ? sourceTerms?.Where(t => t.Term?.Contains(SearchText) ?? false) : sourceTerms;
                targetTerms = IsSet(SearchText) ? targetTerms?.Where(t => t.Term?.Contains(SearchText) ?? false) : targetTerms;

                return (sourceTerms?.Any() ?? false) || (targetTerms?.Any() ?? false);
            };
        }

        private NotifyCollectionChangedEventHandler OnSelectionChanged() => (_, _) =>
        {
            if (IsDbValid)
                SetupEntries();
        };

        private void ResetData()
        {
            Entries = null;
            SourceLanguage = null;
            TargetLanguage = null;
            Languages = null;
            SearchText = "";

            SelectedTags.Clear();
            SelectedGlossaries.Clear();
            Glossaries.Clear();
            Tags.Clear();
        }

        private void Setup()
        {
            ResetData();

            if (string.IsNullOrWhiteSpace(Filepath)) return;
            _terminologyService.Setup(Filepath);

            if (!IsDbValid) return;

            SetupEntries();
            Languages = _terminologyService.GetLanguages();
            Glossaries = _terminologyService.GetGlossaries();
            Tags = _terminologyService.GetTags();
        }

        private void SetupEntries()
        {
            Task.Run(() =>
            {
                var loadEntriesFromDb = new Task<ObservableCollection<EntryModel>>
                (() =>
                    (!SelectedGlossaries.Any() && !SelectedTags.Any()) ? _terminologyService.GetEntriesFromDb(null) :
                    UseTags ? _terminologyService.GetEntriesFromDb(_terminologyService.GetTaggedGlossaries(
                        SelectedTags.Select(t => t.TagName).ToList())) :
                    _terminologyService.GetEntriesFromDb(SelectedGlossaries.Select(g => g.GlossaryName).ToList())
                );
                Entries = new NotifyTaskCompletion<ObservableCollection<EntryModel>>
                (
                    loadEntriesFromDb
                );

                loadEntriesFromDb.RunSynchronously();
            });
        }
    }
}