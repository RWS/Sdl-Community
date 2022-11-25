using GoogleTranslatorProvider.Interface;
using GoogleTranslatorProvider.Models;

namespace GoogleTranslatorProvider.ViewModels
{
	public class HelpViewModel : BaseModel, IHelpViewModel
	{
		public HelpViewModel()
		{
			ViewModel = this;
		}

		public BaseModel ViewModel { get; set; }
	}
}