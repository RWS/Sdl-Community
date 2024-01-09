using InterpretBank.Model;
using InterpretBank.TerminologyService.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;

namespace InterpretBank.Booth.ViewModel
{
    public class BoothWindowViewModel(ITerminologyService terminologyService) : ViewModelBase.ViewModel
    {
        private ObservableCollection<EntryModel> _entries;
        private string _filepath;
        private List<string> _languages;
        private string _searchText;
        private string _sourceLanguage;
        private string _targetLanguage;

        public ObservableCollection<EntryModel> Entries
        {
            get => _entries;
            set => SetField(ref _entries, value);
        }

        public string Filepath
        {
            get => _filepath;
            set
            {
                SetField(ref _filepath, value);
                Setup();
            }
        }

        public List<string> Languages
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

        public string SourceLanguage
        {
            get => _sourceLanguage;
            set => SetField(ref _sourceLanguage, value);
        }

        public string TargetLanguage
        {
            get => _targetLanguage;
            set => SetField(ref _targetLanguage, value);
        }

        private void Filter()
        {
            if (Entries == null) return;
            var collectionView = CollectionViewSource.GetDefaultView(Entries);

            if (string.IsNullOrWhiteSpace(SearchText) || SearchText == "Search...")
            {
                collectionView.Filter = null;
                return;
            }

            collectionView.Filter = null;

            collectionView.Filter = entry => ((EntryModel)entry).Terms.Count > 8;
            collectionView.Filter = entry =>
            {
                var entryModel = (EntryModel)entry;
                var terms = entryModel.Terms;

                var sourceTerms = terms.Where(t => t.LanguageName == SourceLanguage);
                var targetTerms = terms.Where(t => t.LanguageName == TargetLanguage);

                var relevantSourceTerms = sourceTerms.Where(t => t.Term?.Contains(SearchText) ?? false);
                var relevantTargetTerms = targetTerms.Where(t => t.Term?.Contains(SearchText) ?? false);

                return relevantSourceTerms.Any() || relevantTargetTerms.Any();
            };
        }

        private void ResetData()
        {
            Entries = null;
            SourceLanguage = null;
            TargetLanguage = null;
            Languages = null;
            SearchText = "";
        }

        private void Setup()
        {
            ResetData();

            if (string.IsNullOrWhiteSpace(Filepath)) return;
            terminologyService.Setup(Filepath);

            Entries = terminologyService.GetEntriesFromDb(null);

            Languages = terminologyService.GetLanguages().Select(l => l.Name).ToList();
        }
    }
}