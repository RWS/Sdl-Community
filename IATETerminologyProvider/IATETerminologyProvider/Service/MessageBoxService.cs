using System.Windows;
using IATETerminologyProvider.Interface;

namespace IATETerminologyProvider.Service
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
