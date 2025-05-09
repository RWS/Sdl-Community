using Sdl.Community.GSVersionFetch.Interface;
using System.Windows.Input;

namespace Sdl.Community.GSVersionFetch.Commands
{
    // And an extension method to make it easy to raise changed events
    public static class CommandExtensions
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            var canExecuteChanged = command as IRaiseCanExecuteChanged;

            canExecuteChanged?.RaiseCanExecuteChanged();
        }
    }
}