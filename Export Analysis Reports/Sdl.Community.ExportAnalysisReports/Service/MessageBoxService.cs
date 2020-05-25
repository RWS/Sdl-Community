using System.Windows.Forms;
using Sdl.Community.ExportAnalysisReports.Interfaces;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class MessageBoxService : IMessageBoxService
	{
		public virtual DialogResult ShowInformationMessage(string text, string header)
		{
			return MessageBox.Show(text, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}		

		public void ShowOwnerInformationMessage(IWin32Window owner, string text, string header)
		{
			MessageBox.Show(owner, text, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public void ShowWarningMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
	}
}