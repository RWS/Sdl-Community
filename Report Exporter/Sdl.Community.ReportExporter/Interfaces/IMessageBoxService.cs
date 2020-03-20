using System.Windows.Forms;

namespace Sdl.Community.ReportExporter.Interfaces
{
	public interface IMessageBoxService
	{
		void ShowMessage(string text, string header);
		void ShowWarningMessage(string text, string header);
		void ShowInformationMessage(string text, string header);
		void ShowOwnerInformationMessage(IWin32Window owner, string text, string header);
		bool AskForConfirmation(string message);
	}
}