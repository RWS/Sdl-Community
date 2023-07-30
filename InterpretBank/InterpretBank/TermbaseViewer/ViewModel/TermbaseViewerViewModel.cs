using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
		}

		public Image TargetLanguageFlag { get; set; }

		public Image SourceLanguageFlag { get; set; }

		public void JumpToTerm(IEntry entry)
		{
			SelectedItem = Terms.FirstOrDefault(t => t.Id == entry.Id);
		}
	}
}