namespace Sdl.Community.ApplyTMTemplate.Services.Interfaces
{
	public interface IMessageService
	{
		void ShowWarningMessage(string title, string warningMessage);
		void ShowMessage(string title, string message);
		void ShowErrorMessage(string title, string errorMessage);
	}
}