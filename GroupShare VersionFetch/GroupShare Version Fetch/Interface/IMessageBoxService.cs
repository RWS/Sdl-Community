namespace Sdl.Community.GSVersionFetch.Interface
{
    public interface IMessageBoxService
    {
        bool AskForConfirmation(string message);

        void ShowInformationMessage(string text, string header);

        void ShowMessage(string text, string header);

        void ShowWarningMessage(string text, string header);
    }
}