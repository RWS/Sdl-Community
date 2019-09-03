using System.Windows;
using Sdl.CommunityWpfHelpers.Interfaces;

namespace Sdl.CommunityWpfHelpers.Services
{
	/// <summary>
	/// Used for MVVM design pattern
	/// </summary>
	public class MessageBoxService:IMessageBoxService
	{
		public void ShowMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButton.OK);
		}

		public void ShowWarningMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Warning);}

		public void ShowInformationMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public bool AskForConfirmation(string message,string header)
		{
			var result = MessageBox.Show(message, header, MessageBoxButton.OKCancel, MessageBoxImage.Question);
			return result.HasFlag(MessageBoxResult.OK);
		}
	}
}
