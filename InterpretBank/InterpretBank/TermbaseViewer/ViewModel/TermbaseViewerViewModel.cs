using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Input;
using InterpretBank.Commands;
using InterpretBank.TermbaseViewer.Model;
using InterpretBank.TerminologyService.Interface;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.TermbaseViewer.ViewModel
{
	public class TermbaseViewerViewModel : ViewModelBase.ViewModel
	{
		//private int _selectedIndex;
		private TermModel _selectedItem;

		public TermbaseViewerViewModel(ITerminologyService termSearchService)
		{
			TerminologyService = termSearchService;
		}

		//public int SelectedIndex
		//{
		//	get => _selectedIndex;
		//	set => SetField(ref _selectedIndex, value);
		//}

		public ITerminologyService TerminologyService { get; }
		public IEnumerable<TermModel> Terms { get; set; }
		public Language SourceLanguage { get; set; }
		public Language TargetLanguage { get; set; }


		public TermModel SelectedItem
		{
			get => _selectedItem;
			set => SetField(ref _selectedItem, value);
		}

		public void LoadTerms(Language source, Language target, List<string> glossaries)
		{
			SourceLanguage = source;
			TargetLanguage = target;

			SourceLanguageFlag = source.GetFlagImage();
			TargetLanguageFlag = target.GetFlagImage();

			var sourceLanguage = source.DisplayName.Split(' ')[0];
			var targetLanguage = target.DisplayName.Split(' ')[0];

			Terms = TerminologyService.GetAllTerms(sourceLanguage, targetLanguage, glossaries);

			foreach (var termModel in Terms)
			{
				termModel.PropertyChanged += (sender, args) => OnPropertyChanged(nameof(AnyEditedTerms));
			}

			SelectedItem = Terms.FirstOrDefault();
		}

		public Image TargetLanguageFlag { get; set; }

		public Image SourceLanguageFlag { get; set; }
		public bool AnyEditedTerms => Terms.Any(t => t.Edited);
		public ICommand CommitAllToDatabaseCommand => new RelayCommand(CommitAllToDatabase);

		private void CommitAllToDatabase(object obj)
		{
			var changedTerms = Terms.Where(t => t.Edited);
			
			TerminologyService.SaveAllTerms(changedTerms);

			foreach (var changedTerm in changedTerms)
			{
				//setting edited to false may not be needed if we are setting original term 
				changedTerm.Edited = false;
				changedTerm.SetOriginalTerm();
			}

			OnPropertyChanged(nameof(AnyEditedTerms));
		}

		public void JumpToTerm(IEntry entry)
		{
			SelectedItem = Terms.FirstOrDefault(t => t.Id == entry.Id);
		}
	}
}