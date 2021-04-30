using System.Windows;
using Sdl.Community.IATETerminologyProvider.Interface;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class MessageBoxService : IMessageBoxService
	{
		public MessageDialogResult ShowYesNoMessageBox(string title, string message)
		{
			var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);

			return result == MessageBoxResult.Yes ? MessageDialogResult.Yes : MessageDialogResult.No;
		}
	}
}
