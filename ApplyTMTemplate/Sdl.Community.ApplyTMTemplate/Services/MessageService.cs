using System.Windows;
using Sdl.Community.ApplyTMTemplate.Services.Interfaces;

namespace Sdl.Community.ApplyTMTemplate.Services
{
	public class MessageService : IMessageService
	{
		public void ShowWarningMessage(string title, string warningMessage)
		{
			MessageBox.Show(warningMessage, title, MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public void ShowMessage(string title, string message)
		{
			MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public void ShowErrorMessage(string title, string errorMessage)
		{
			MessageBox.Show(errorMessage, title, MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}