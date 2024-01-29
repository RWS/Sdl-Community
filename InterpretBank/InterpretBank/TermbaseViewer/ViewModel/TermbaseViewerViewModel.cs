using InterpretBank.Commands;
using InterpretBank.Extensions;
using InterpretBank.Helpers;
using InterpretBank.Interface;
using InterpretBank.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InterpretBank.TermbaseViewer.ViewModel
{
    public class TermbaseViewerViewModel(
        ITerminologyService terminologyService,
        IUserInteractionService userInteractionService)
        : ViewModelBase.ViewModel
    {
        private ICommand _addEntryCommand;
        private NotifyTaskCompletion<ObservableCollection<EntryModel>> _entries;
        private ICommand _removeSelectedEntryCommand;

        private ICommand _saveEditCommand;

        private EntryModel _selectedEntry;
        private Image _sourceLanguageFlag;
        private string _sourceLanguageName;
        private Image _targetLanguageFlag;
        private string _targetLanguageName;
        public ICommand AddEntryCommand => _addEntryCommand ??= new RelayCommand(s => OpenAddTermPopup(null, null));

        public NotifyTaskCompletion<ObservableCollection<EntryModel>> Entries
        {
            get => _entries;
            set
            {
                if (!SetField(ref _entries, value)) return;

                if (_entries == null) return;
                _entries.PropertyChanged -= Entries_PropertyChanged;
                _entries.PropertyChanged += Entries_PropertyChanged;
            }
        }

        public ICommand RemoveSelectedEntryCommand => _removeSelectedEntryCommand ??= new RelayCommand(RemoveSelectedEntry);

        public ICommand SaveEditCommand => _saveEditCommand ??= new RelayCommand(UpdateTerm);

        public EntryModel SelectedEntry
        {
            get => _selectedEntry;
            set => SetField(ref _selectedEntry, value);
        }

        public Image SourceLanguageFlag
        {
            get => _sourceLanguageFlag;
            set => SetField(ref _sourceLanguageFlag, value);
        }

        public string SourceLanguageName
        {
            get => _sourceLanguageName;
            set => SetField(ref _sourceLanguageName, value);
        }

        public Image TargetLanguageFlag
        {
            get => _targetLanguageFlag;
            set => SetField(ref _targetLanguageFlag, value);
        }

        public string TargetLanguageName
        {
            get => _targetLanguageName;
            set => SetField(ref _targetLanguageName, value);
        }

        public IUserInteractionService UserInteractionService { get; set; } = userInteractionService;

        private List<string> Glossaries { get; set; }

        private Language SourceLanguage { get; set; }

        private Language TargetLanguage { get; set; }

        private ITerminologyService TerminologyService { get; set; } = terminologyService;

        public void InitializeEntry(EntryModel entryModel)
        {
            entryModel.Name =
                entryModel.Terms.FirstOrDefault(t => t.LanguageName == SourceLanguageName)?.Term;
        }

        public void LoadTerms()
        {
            Entries = null;

            var loadEntriesFromDb = new Task<ObservableCollection<EntryModel>>(() =>
                TerminologyService.GetEntriesFromDb(Glossaries));

            Entries = new NotifyTaskCompletion<ObservableCollection<EntryModel>>(loadEntriesFromDb);

            loadEntriesFromDb.RunSynchronously();
        }

        public void OpenAddTermPopup(string source, string target)
        {
            UserInteractionService.GetNewTermDetailsFromUser(Glossaries, SourceLanguageName, TargetLanguageName, source,
                target);

            UserInteractionService.GotTermDetailsEvent -= AddTerm;
            UserInteractionService.GotTermDetailsEvent += AddTerm;
        }

        public void ReloadDb(string filepath)
        {
            TerminologyService.Setup(filepath);
            LoadTerms();
        }

        public void ReloadTerms(Language sourceLanguage, Language targetLanguage)
        {
            SetLanguagePair(sourceLanguage, targetLanguage);
            LoadTerms();
        }

        public void Setup(Language sourceLanguage, Language targetLanguage, List<string> glossaries, string databaseFilePath)
        {
            Glossaries = glossaries;
            SetLanguagePair(sourceLanguage, targetLanguage);

            TerminologyService.Setup(databaseFilePath);

            LoadTerms();
        }

        private void AddTerm(string source, string target, string glossaryName)
        {
            UserInteractionService.GotTermDetailsEvent -= AddTerm;
            if (string.IsNullOrWhiteSpace(glossaryName)) return;

            var addTermActionResult =
                TerminologyService.AddTerm(source, target, glossaryName, SourceLanguageName, TargetLanguageName);
            if (!IsActionSuccessful(addTermActionResult)) return;

            var newEntryModel = addTermActionResult.Result;
            newEntryModel.GlossaryName = glossaryName;

            InitializeEntry(newEntryModel);
            Entries.Result.Add(newEntryModel);
        }

        private void Entries_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Status") return;

            Entries.Result.ForEach(entry => entry.Terms.ForEach(t =>
            {
                t.PropertyChanged -= TermChanged;
                t.PropertyChanged += TermChanged;
            }));

            var previousTerm = SelectedEntry;
            InitializeEntries(Entries.Result);
            MoveSourceAndTargetTermsFirst();
            SetSelectedEntry(previousTerm);
        }

        private void InitializeEntries(ObservableCollection<EntryModel> entries)
        {
            foreach (var entry in entries)
                InitializeEntry(entry);
        }

        private bool IsActionSuccessful<T>(ActionResult<T> actionResult)
        {
            if (actionResult.Success) return true;

            UserInteractionService.WarnUser(actionResult.Message);
            return false;
        }

        private void MoveSourceAndTargetTermsFirst()
        {
            foreach (var entryModel in Entries.Result)
            {
                var sourceTerm = entryModel.Terms.FirstOrDefault(t => t.LanguageName == SourceLanguageName);
                var targetTerm = entryModel.Terms.FirstOrDefault(t => t.LanguageName == TargetLanguageName);

                entryModel.Terms.Remove(sourceTerm);
                entryModel.Terms.Insert(0, sourceTerm);

                entryModel.Terms.Remove(targetTerm);
                entryModel.Terms.Insert(1, targetTerm);
            }
        }

        private void RemoveSelectedEntry(object obj)
        {
            var confirmation = UserInteractionService.Confirm("Are you sure you want to delete this entry?");

            if (!confirmation) return;

            TerminologyService.RemoveTerm(SelectedEntry);
            Entries.Result.Remove(SelectedEntry);
        }

        private void SetLanguagePair(Language sourceLanguage, Language targetLanguage)
        {
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;

            SourceLanguageName = sourceLanguage.GetInterpretBankLanguageName();
            TargetLanguageName = targetLanguage.GetInterpretBankLanguageName();

            SourceLanguageFlag = sourceLanguage.GetFlagImage();
            TargetLanguageFlag = targetLanguage.GetFlagImage();
        }

        private void SetSelectedEntry(EntryModel term)
        {
            if (Entries.Result.Contains(term))
                SelectedEntry = Entries.Result.FirstOrDefault(e => e.Equals(term));
            else if (Entries.Result.Any()) SelectedEntry = Entries.Result.FirstOrDefault();
        }

        private void TermChanged(object sender, PropertyChangedEventArgs e)
        {
            var termModel = sender as TermModel;
            if (termModel?.LanguageName == SourceLanguageName && e.PropertyName == nameof(TermModel.Term))
                SelectedEntry.Name = termModel?.Term;
        }

        private void UpdateTerm(object obj)
        {
            switch (obj)
            {
                case TermModel termModel:
                    termModel.Modified = false;
                    TerminologyService.UpdateTerm(new TermChange
                    {
                        EntryId = SelectedEntry.Id,
                        GlossaryName = SelectedEntry.GlossaryName,
                        LanguageName = termModel.LanguageName,
                        Term = termModel.Term,
                        FirstComment = termModel.FirstComment,
                        SecondComment = termModel.SecondComment,
                    });
                    break;

                case EntryModel entryModel:
                    TerminologyService.UpdateEntry(new EntryChange
                    {
                        EntryId = entryModel.Id,
                        EntryComment = entryModel.EntryComment
                    });
                    break;
            }
        }

        public void JumpToTerm(Entry entry)
        {
          var jumpEntry =  Entries.Result.FirstOrDefault(e => e.Id == entry.Id);
          SetSelectedEntry(jumpEntry);
        }
    }
}