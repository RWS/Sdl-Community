﻿using System.Windows;
using Sdl.Community.InSource.Interfaces;

namespace Sdl.Community.InSource.Service
{
	public class MessageBoxService : IMessageBoxService
	{
		public void ShowMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButton.OK);
		}

		public void ShowWarningMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public void ShowInformationMessage(string text, string header)
		{
			MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public bool AskForConfirmation(string message)
		{
			var result = MessageBox.Show(message, string.Empty, MessageBoxButton.OKCancel, MessageBoxImage.Question);
			return result.HasFlag(MessageBoxResult.OK);
		}
	}
}