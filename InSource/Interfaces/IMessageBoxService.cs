namespace Sdl.Community.InSource.Interfaces
{
    public interface IMessageBoxService
    {
        bool AskForConfirmation(string message);

        void ShowInformation(string text, string header);

        void ShowWarningMessage(string text, string header);
    }
}