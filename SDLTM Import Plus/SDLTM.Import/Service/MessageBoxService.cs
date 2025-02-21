using System.Windows;
using SDLTM.Import.Interface;

namespace SDLTM.Import.Service
{
	public class MessageBoxService: IMessageBoxService
	{
		public MessageDialogResult ShowYesNoMessageBox(string title, string message)
		{
			var result=  MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);

			return result == MessageBoxResult.Yes ? MessageDialogResult.Yes : MessageDialogResult.No;
		}
	}
}
