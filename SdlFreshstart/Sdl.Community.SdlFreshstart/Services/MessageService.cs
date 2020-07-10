using System.Windows;

namespace Sdl.Community.SdlFreshstart.Services
{
	public class MessageService : IMessageService
	{
		public void ShowErrorMessage(string title, string errorMessage)
		{
			MessageBox.Show(errorMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public void ShowMessage(string title, string message)
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public void ShowWarningMessage(string title, string warningMessage)
		{
			MessageBox.Show(warningMessage, title, MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public MessageBoxResult ShowConfirmationMessage(string title, string question)
		{
			return MessageBox.Show(question, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
		}
	}
}