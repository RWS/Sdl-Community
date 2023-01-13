using System.Collections.Generic;
using System.Windows.Input;
using MicrosoftTranslatorProvider.Model;

namespace MicrosoftTranslatorProvider.Interfaces
{
	public interface IMainWindow
	{
		bool DialogResult { get; set; }

		ViewDetails SelectedView { get; set; }

		List<ViewDetails> AvailableViews { get; set; }
		
		ITranslationOptions Options { get; set; }

		bool IsWindowValid();
	}
}