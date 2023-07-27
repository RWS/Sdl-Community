using System.Collections.Generic;
using System.Linq;
using InterpretBank.TermbaseViewer.Model;
using InterpretBank.TerminologyService.Interface;
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

		public TermModel SelectedItem
		{
			get => _selectedItem;
			set => SetField(ref _selectedItem, value);
		}

		public void LoadTerms(string source, string target, List<string> glossaries)
		{
			Terms = TerminologyService.GetAllTerms(source, target, glossaries);
		}

		public void JumpToTerm(IEntry entry)
		{
			SelectedItem = Terms.FirstOrDefault(t => t.Id == entry.Id);
		}
	}
}