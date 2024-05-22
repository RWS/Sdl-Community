using Sdl.Community.InSource.Interfaces;
using Sdl.Community.InSource.Tellme.WarningWindow;
using System.Windows;

namespace Sdl.Community.InSource.Service
{
    public class MessageBoxService : IMessageBoxService
    {
        public bool AskForConfirmation(string message)
        {
            var result = GetWindow(message, PluginResources.MessageBoxService_AskForConfirmation_Question, MessageBoxButton.OKCancel, MessageBoxImage.Question).ShowDialog();
            return result == true;
        }

        public void ShowInformation(string text, string header)
        {
            GetWindow(text, header, MessageBoxButton.OK, MessageBoxImage.Information).ShowDialog();
        }

        public void ShowWarningMessage(string text, string header)
        {
            GetWindow(text, header, MessageBoxButton.OK, MessageBoxImage.Warning).ShowDialog();
        }

        private SettingsActionWarning GetWindow(string text, string header, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage messageBoxImage = MessageBoxImage.None)
        {
            var infoWindow = new SettingsActionWarning(text, header, messageBoxButton, messageBoxImage);
            return infoWindow;
        }
    }
}