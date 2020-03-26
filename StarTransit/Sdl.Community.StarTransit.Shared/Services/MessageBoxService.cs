using System.Windows.Forms;
using Sdl.Community.StarTransit.Shared.Interfaces;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class MessageBoxService : IMessageBoxService
	{
		public void ShowMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButtons.OK);
		}

		public void ShowWarningMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		public void ShowInformationMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		public bool AskForConfirmation(string message)
		{
			var result = MessageBox.Show(message, string.Empty, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			return result.HasFlag(MessageBoxButtons.OK);
		}
	}
}