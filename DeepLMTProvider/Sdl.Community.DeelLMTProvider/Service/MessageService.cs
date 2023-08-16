using System.Windows;
using Sdl.Community.DeepLMTProvider.Interface;

namespace Sdl.Community.DeepLMTProvider.Service
{
	public class MessageService : IMessageService
	{
		public void ShowWarning(string message, string failingMethod)
		{
			MessageBox.Show(message, failingMethod, MessageBoxButton.OK, MessageBoxImage.Warning);
		}
	}
}