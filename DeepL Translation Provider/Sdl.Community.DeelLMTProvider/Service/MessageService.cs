using System.Windows;
using Sdl.Community.DeepLMTProvider.Interface;
using System.Runtime.CompilerServices;

namespace Sdl.Community.DeepLMTProvider.Service
{
    public class MessageService : IMessageService
    {
        public void ShowWarning(string message, [CallerMemberName] string failingMethod = null) =>
            MessageBox.Show(message, failingMethod, MessageBoxButton.OK, MessageBoxImage.Warning);

        public bool ShowDialog(string message, [CallerMemberName] string method = null) =>
            MessageBox.Show(message, method, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }
}