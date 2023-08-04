using System.Collections.Generic;
using System.Collections.ObjectModel;
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

				SelectedItem = Terms[value];

				OnPropertyChanged();
			}
		}

		public void JumpToTerm(IEntry entry)
		{
			SelectedItem = Terms.FirstOrDefault(t => t.Id == entry.Id);
		}

		public void LoadTerms(Language source, Language target, List<string> glossaries)
		{
			SourceLanguage = source;
			TargetLanguage = target;

			SourceLanguageFlag = source.GetFlagImage();
			TargetLanguageFlag = target.GetFlagImage();

			var sourceLanguage = source.DisplayName.Split(' ')[0];
			var targetLanguage = target.DisplayName.Split(' ')[0];

			Terms = new ObservableCollection<TermModel>(TerminologyService.GetAllTerms(sourceLanguage, targetLanguage, glossaries));

			foreach (var termModel in Terms)
			{
				termModel.PropertyChanged += (_, _) =>
				{
					OnPropertyChanged(nameof(AnyEditedTerms));
				};
			}

			SelectedItem = Terms.FirstOrDefault();
		}

		private void AddNewTerm(object obj)
		{
			var termModel = new TermModel
			{
				IsEditing = true
			};

			Terms.Add(termModel);
			termModel.SetOriginalTerm();

			SelectedItem = termModel;
		}

		private void CommitAllToDatabase(object obj)
		{
			var changedTerms = Terms.Where(t => t.Edited);

			TerminologyService.SaveAllTerms(changedTerms);
			changedTerms.ForEach(t => t.SetOriginalTerm(true));
		}

		private void RevertChanges(object obj)
		{
			if (SelectedItem.Id == -1)
			{
				var selectedItem = SelectedItem;
				SelectedItem = Terms.FirstOrDefault();

				Terms.Remove(selectedItem);
			}
			else
				SelectedItem.Revert();

			OnPropertyChanged(nameof(AnyEditedTerms));
		}
	}
}