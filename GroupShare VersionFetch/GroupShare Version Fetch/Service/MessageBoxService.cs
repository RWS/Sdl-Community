﻿using Sdl.Community.GSVersionFetch.Interface;
using System.Windows;

namespace Sdl.Community.GSVersionFetch.Service
{
    public class MessageBoxService : IMessageBoxService
    {
        public bool AskForConfirmation(string message)
        {
            var result = MessageBox.Show(message, string.Empty, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            return result.HasFlag(MessageBoxResult.OK);
        }

        public void ShowInformationMessage(string text, string header)
        {
            MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowMessage(string text, string header)
        {
            MessageBox.Show(text, header, MessageBoxButton.OK);
        }

        public void ShowWarningMessage(string text, string header)
        {
            MessageBox.Show(text, header, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}