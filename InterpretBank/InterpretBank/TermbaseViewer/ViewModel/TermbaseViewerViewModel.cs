using InterpretBank.Extensions;
using InterpretBank.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

namespace InterpretBank.TermbaseViewer.ViewModel
{
    public class TermbaseViewerViewModel : ViewModelBase.ViewModel
    {
        private ObservableCollection<EntryModel> _entries;
        private EntryModel _selectedEntry;
        private int _selectedEntryIndex;
        private Image _sourceLanguageFlag;
        private string _sourceLanguageName;
        private Image _targetLanguageFlag;
        private string _targetLanguageName;

        public TermbaseViewerViewModel(ITerminologyService terminologyService)
        {
            TerminologyService = terminologyService;
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

        private Language SourceLanguage { get; set; }

        private Language TargetLanguage { get; set; }

        private ITerminologyService TerminologyService { get; set; }

        public void LoadTerms()
        {
            Entries = TerminologyService.GetEntriesFromDb(Glossaries);
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

        private void SetEntryNames(ObservableCollection<EntryModel> entries)
        {
            entries.ForEach(entryModel =>
            {
                entryModel.Name = entryModel.Terms.FirstOrDefault(t => t.LanguageName == SourceLanguageName)?.Term;
            });
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

        private void SetSelectedEntry(EntryModel previousTerm)
        {
            if (Entries.Contains(previousTerm))
                SelectedEntry = Entries.FirstOrDefault(e => e.Equals(previousTerm));
            else if (Entries.Any()) SelectedEntry = Entries.FirstOrDefault();
        }
    }
}