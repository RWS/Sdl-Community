using ProjectWizardExample.Wizard.API;

namespace ProjectWizardExample.EventArgs
{
	public class SelectedPageEventArgs : System.EventArgs
	{
		public IProgressHeaderItem ProgressHeaderItem { get; set; }

		public int PagePosition { get; set; }
	}
}
