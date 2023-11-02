using InterpretBank.Commands;
using InterpretBank.Extensions;
using InterpretBank.Interface;
using InterpretBank.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;

namespace InterpretBank.TermbaseViewer.ViewModel
{
    public class TermbaseViewerViewModel : ViewModelBase.ViewModel
    {
        private int _selectedIndex;
        private TermModel _selectedItem;

        private string _sourceLanguage;
        private Image _sourceLanguageFlag;
        private string _targetLanguage;
        private Image _targetLanguageFlag;
        private ObservableCollection<TermModel> _terms;

        public TermbaseViewerViewModel(IUserInteractionService dialogService)
        {
            DialogService = dialogService;
            //TerminologyService.ShouldReload += () => LoadTerms();
        }

        public event Action<bool> AnyEditedTermsChanged;

        public ICommand AddNewTermCommand => new RelayCommand(AddNewTerm);
        public bool AnyEditedTerms => Terms?.Any(t => t.Edited) ?? false;
        public ICommand CommitAllToDatabaseCommand => new RelayCommand(CommitAllToDatabase);
        public ICommand RevertCommand => new RelayCommand(RevertChanges);

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value == _selectedIndex)
                    return;
                _selectedIndex = value;

                if (Terms.Any()) SelectedItem = value == -1 ? null : Terms[value];

                OnPropertyChanged();
            }
        }

        public TermModel SelectedItem
        {
            get => _selectedItem;
            set => SetField(ref _selectedItem, value);
        }

        public string SourceLanguage
        {
            get => _sourceLanguage;
            set => SetField(ref _sourceLanguage, value);
        }

        public Image SourceLanguageFlag
        {
            get => _sourceLanguageFlag;
            set => SetField(ref _sourceLanguageFlag, value);
        }

        public string TargetLanguage
        {
            get => _targetLanguage;
            set => SetField(ref _targetLanguage, value);
        }

        public Image TargetLanguageFlag
        {
            get => _targetLanguageFlag;
            set => SetField(ref _targetLanguageFlag, value);
        }

        public ITerminologyService TerminologyService { get; set; }

        public ObservableCollection<TermModel> Terms
        {
            get => _terms;
            set
            {
                if (SetField(ref _terms, value))
                    RaiseAnyEditedPropertyChanged();
            }
        }

        private IUserInteractionService DialogService { get; }
        private List<string> Glossaries { get; set; }

        public void EditTerm(IEntry term)
        {
            JumpToTerm(term);
            SelectedItem.IsEditing = true;
        }

        public void JumpToTerm(IEntry entry)
        {
            var term = Terms.FirstOrDefault(t => t.Id == entry.Id);
            SelectedIndex = Terms.IndexOf(term);
        }

        //public void LoadTerms()

        public void LoadTerms(Language source, Language target, List<string> glossaries, ITerminologyService terminologyService)
        {
            TerminologyService = terminologyService;
            Glossaries = glossaries;

            SetLanguagePair(source, target);

            LoadTermsFromDb(SourceLanguage, TargetLanguage, glossaries);
            Terms.ForEach(t => t.PropertyChanged += OnTermModelOnPropertyChanged);

            ResetIndex();
        }

        public void ReloadTerms(Language sourceLanguage, Language targetLanguage)
        {
            SetLanguagePair(sourceLanguage, targetLanguage);
            ReloadTerms();
        }

        private void AddNewTerm(object obj)
        {
            //if (Terms.Any(t => t.Id == -1 && !t.Edited)) return;

            var termModel = obj as TermModel ?? new TermModel();
            termModel.IsEditing = true;

            termModel.PropertyChanged += OnTermModelOnPropertyChanged;
            termModel.SetOriginalTerm();

            var sourceLanguageIndex = TerminologyService.GetLanguageIndex(SourceLanguage);
            var targetLanguageIndex = TerminologyService.GetLanguageIndex(TargetLanguage);

            termModel.SourceLanguageIndex = sourceLanguageIndex;
            termModel.TargetLanguageIndex = targetLanguageIndex;

            var glossaryNameFromUser = DialogService.GetGlossaryNameFromUser(Glossaries);
            if (glossaryNameFromUser is null) return;

            termModel.GlossaryName = glossaryNameFromUser;
            Terms.Add(termModel);
            SelectedIndex = Terms.IndexOf(termModel);
            RaiseAnyEditedPropertyChanged();
        }

        private void CommitAllToDatabase(object obj)
        {
            var changedTerms = Terms.Where(t => t.Edited).ToList();

            TerminologyService.SaveAllTerms(changedTerms);
            changedTerms.ForEach(t => t.SetOriginalTerm(true));

            ReloadTerms();
        }

        private void LoadTermsFromDb(string source, string target, List<string> glossaries) =>
            Terms = new ObservableCollection<TermModel>(TerminologyService.GetAllTerms(source, target,
                glossaries));

        private void OnTermModelOnPropertyChanged(object o, PropertyChangedEventArgs propertyChangedEventArgs) => RaiseAnyEditedPropertyChanged();

        private void RaiseAnyEditedPropertyChanged()
        {
            OnPropertyChanged(nameof(AnyEditedTerms));
            AnyEditedTermsChanged?.Invoke(AnyEditedTerms);
        }

        private void ReloadTerms()
        {
            Terms.ForEach(t => t.PropertyChanged -= OnTermModelOnPropertyChanged);
            LoadTermsFromDb(SourceLanguage, TargetLanguage, Glossaries);
            Terms.ForEach(t => t.PropertyChanged += OnTermModelOnPropertyChanged);
            ResetIndex();
        }

        private void ResetIndex()
        {
            SelectedIndex = 1;
            SelectedIndex = 0;
        }

        private void RevertChanges(object obj)
        {
            SelectedItem.IsRemoved = false;
            if (SelectedItem.Id == -1)
            {
                var selectedItem = SelectedItem;
                Terms.Remove(selectedItem);

                SelectedIndex = Terms.Any() ? Terms.Count - 1 : -1;
            }
            else
                SelectedItem.Revert();

            RaiseAnyEditedPropertyChanged();
        }

        private void SetLanguagePair(Language sourceLanguage, Language targetLanguage)
        {
            SourceLanguage = sourceLanguage.GetInterpretBankLanguageName();
            TargetLanguage = targetLanguage.GetInterpretBankLanguageName();

            SourceLanguageFlag = sourceLanguage.GetFlagImage();
            TargetLanguageFlag = targetLanguage.GetFlagImage();
        }
    }
}