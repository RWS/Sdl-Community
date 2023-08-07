using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.Extensions;
using InterpretBank.TermbaseViewer.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.TermbaseViewer.ViewModel
{
	public class TermbaseViewerViewModel : ViewModelBase.ViewModel
	{
		private TermModel _selectedItem;

		private ObservableCollection<TermModel> _terms;
		private int _selectedIndex;

		public TermbaseViewerViewModel(ITerminologyService termSearchService)
		{
			TerminologyService = termSearchService;
		}

		public ICommand AddNewTermCommand => new RelayCommand(AddNewTerm);
		public bool AnyEditedTerms => Terms.Any(t => t.Edited);
		public ICommand CommitAllToDatabaseCommand => new RelayCommand(CommitAllToDatabase);
		public ICommand RevertCommand => new RelayCommand(RevertChanges);

		public TermModel SelectedItem
		{
			get => _selectedItem;
			set => SetField(ref _selectedItem, value);
		}

		public Language SourceLanguage { get; set; }
		public Image SourceLanguageFlag { get; set; }
		public Language TargetLanguage { get; set; }
		public Image TargetLanguageFlag { get; set; }
		public ITerminologyService TerminologyService { get; }

		public ObservableCollection<TermModel> Terms
		{
			get => _terms;
			set
			{
				if (SetField(ref _terms, value))
					OnPropertyChanged(nameof(AnyEditedTerms));
			}
		}

		public int SelectedIndex
		{
			get => _selectedIndex;
			set
			{
				if (value == _selectedIndex) return;
				_selectedIndex = value;

				//SelectedItem = Terms[value == -1 ? 0 : value];
				SelectedItem = value == -1 ? null : Terms[value];

				OnPropertyChanged();
			}
		}

		public void JumpToTerm(IEntry entry)
		{
			SelectedItem = Terms.FirstOrDefault(t => t.Id == entry.Id);
		}

		public void LoadTerms(Language source, Language target, List<string> glossaries)
		{
			Glossaries = glossaries;
			SourceLanguage = source;
			TargetLanguage = target;

			SourceLanguageFlag = source.GetFlagImage();
			TargetLanguageFlag = target.GetFlagImage();

			LoadTermsFromDb(source, target, glossaries);
			Terms.ForEach(t => t.PropertyChanged += OnTermModelOnPropertyChanged);

			SelectedIndex = 1;
			SelectedIndex = 0;
		}

		private void LoadTermsFromDb(Language source, Language target, List<string> glossaries)
		{
			var sourceLanguage = source.DisplayName.Split(' ')[0];
			var targetLanguage = target.DisplayName.Split(' ')[0];

			Terms = new ObservableCollection<TermModel>(TerminologyService.GetAllTerms(sourceLanguage, targetLanguage,
				glossaries));
		}

		private void OnTermModelOnPropertyChanged(object o, PropertyChangedEventArgs propertyChangedEventArgs)
		{
			OnPropertyChanged(nameof(AnyEditedTerms));
		}

		private void ReloadTerms()
		{
			Terms.ForEach(t => t.PropertyChanged -= OnTermModelOnPropertyChanged);
			LoadTermsFromDb(SourceLanguage, TargetLanguage, Glossaries);
			Terms.ForEach(t => t.PropertyChanged += OnTermModelOnPropertyChanged);
		}

		private List<string> Glossaries { get; set; }

		private void AddNewTerm(object obj)
		{
			var termModel = new TermModel
			{
				IsEditing = true
			};

			Terms.Add(termModel);

			
			termModel.PropertyChanged += OnTermModelOnPropertyChanged;
			termModel.SetOriginalTerm();

			//TODO Find another method of taking the indices: the list may be empty
			termModel.SourceLanguageIndex = Terms[0].SourceLanguageIndex;
			termModel.TargetLanguageIndex= Terms[0].TargetLanguageIndex;
			termModel.GlossaryName = Terms[0].GlossaryName;

			

			SelectedIndex = Terms.IndexOf(termModel);
		}

		private void CommitAllToDatabase(object obj)
		{
			var changedTerms = Terms.Where(t => t.Edited).ToList();

			TerminologyService.SaveAllTerms(changedTerms);
			changedTerms.ForEach(t => t.SetOriginalTerm(true));

			ReloadTerms();

			SelectedIndex = Terms.Count - 1;
		}

		private void RevertChanges(object obj)
		{
			SelectedItem.IsRemoved = false;
			if (SelectedItem.Id == -1)
			{
				var selectedItem = SelectedItem;
				Terms.Remove(selectedItem);

				SelectedIndex = Terms.Any() ? Terms.Count - 1: -1;
			}
			else
				SelectedItem.Revert();

			OnPropertyChanged(nameof(AnyEditedTerms));
		}
	}
}