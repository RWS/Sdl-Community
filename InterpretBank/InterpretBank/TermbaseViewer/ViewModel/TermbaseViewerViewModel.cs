using InterpretBank.Commands;
using InterpretBank.Extensions;
using InterpretBank.Helpers;
using InterpretBank.Interface;
using InterpretBank.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;

namespace InterpretBank.TermbaseViewer.ViewModel
{
    public class TermbaseViewerViewModel : ViewModelBase.ViewModel
    {
        private ObservableCollection<EntryModel> _entries;
        private ICommand _saveEditCommand;
        private EntryModel _selectedEntry;
        private int _selectedEntryIndex;
        private TermModel _selectedTerm;
        private Image _sourceLanguageFlag;
        private string _sourceLanguageName;
        private Image _targetLanguageFlag;
        private string _targetLanguageName;

        public TermbaseViewerViewModel(ITerminologyService terminologyService, IUserInteractionService userInteractionService)
        {
            TerminologyService = terminologyService;
            UserInteractionService = userInteractionService;
        }

        public ObservableCollection<EntryModel> Entries
        {
            get => _entries;
            set
            {
                var previousTerm = SelectedEntry;
                if (!SetField(ref _entries, value)) return;

                SetEntryNames(_entries);
                MoveSourceAndTargetTermsFirst();
                SetSelectedEntry(previousTerm);
            }
        }

        public List<string> Glossaries { get; set; }
        public ICommand SaveEditCommand => _saveEditCommand ??= new RelayCommand(UpdateTerm);

        public EntryModel SelectedEntry
        {
            get => _selectedEntry;
            set => SetField(ref _selectedEntry, value);
        }

        public int SelectedEntryIndex
        {
            get => _selectedEntryIndex;
            set => SetField(ref _selectedEntryIndex, value);
        }

        public TermModel SelectedTerm
        {
            get => _selectedTerm;
            set => SetField(ref _selectedTerm, value);
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

        public IUserInteractionService UserInteractionService { get; set; }
        private Language SourceLanguage { get; set; }

        private Language TargetLanguage { get; set; }

        private ITerminologyService TerminologyService { get; set; }

        public void LoadTerms()
        {
            Entries = TerminologyService.GetEntriesFromDb(Glossaries);
        }

        public void OpenAddTermPopup(string source, string target)
        {
            UserInteractionService.GetNewTermDetailsFromUser(Glossaries, SourceLanguageName, TargetLanguageName, source,
                target, SourceLanguageFlag, TargetLanguageFlag);

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

        public void SetEntryName(EntryModel entryModel) => entryModel.Name =
            entryModel.Terms.FirstOrDefault(t => t.LanguageName == SourceLanguageName)?.Term;

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

            SetEntryName(newEntryModel);
            Entries.Add(newEntryModel);
        }

        private bool IsActionSuccessful<T>(ActionResult<T> actionResult)
        {
            if (actionResult.Success) return true;

            UserInteractionService.WarnUser(actionResult.Message);
            return false;
        }

        private void MoveSourceAndTargetTermsFirst()
        {
            foreach (var entryModel in Entries)
            {
                var sourceTerm = entryModel.Terms.FirstOrDefault(t => t.LanguageName == SourceLanguageName);
                var targetTerm = entryModel.Terms.FirstOrDefault(t => t.LanguageName == TargetLanguageName);

                entryModel.Terms.Remove(sourceTerm);
                entryModel.Terms.Insert(0, sourceTerm);

                entryModel.Terms.Remove(targetTerm);
                entryModel.Terms.Insert(1, targetTerm);
            }
        }

        private void SetEntryNames(ObservableCollection<EntryModel> entries) => entries.ForEach(SetEntryName);

        private void SetLanguagePair(Language sourceLanguage, Language targetLanguage)
        {
            SourceLanguage = sourceLanguage;
            TargetLanguage = targetLanguage;

            SourceLanguageName = sourceLanguage.GetInterpretBankLanguageName();
            TargetLanguageName = targetLanguage.GetInterpretBankLanguageName();

            SourceLanguageFlag = sourceLanguage.GetFlagImage();
            TargetLanguageFlag = targetLanguage.GetFlagImage();
        }

        private void SetSelectedEntry(EntryModel previousTerm)
        {
            if (Entries.Contains(previousTerm))
                SelectedEntry = Entries.FirstOrDefault(e => e.Equals(previousTerm));
            else if (Entries.Any()) SelectedEntry = Entries.FirstOrDefault();
        }

        private void UpdateTerm(object obj)
        {
            if (obj is not TermModel { Modified: true } termModel) return;

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
        }
    }
}