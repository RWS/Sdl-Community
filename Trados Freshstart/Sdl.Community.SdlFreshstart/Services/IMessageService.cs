using System.Windows;

namespace Sdl.Community.SdlFreshstart.Services
{
	public interface IMessageService
	{
		void ShowWarningMessage(string title, string warningMessage);
		void ShowMessage(string title, string message);
		void ShowErrorMessage(string title, string errorMessage);
		MessageBoxResult ShowConfirmationMessage(string title, string question);
	}
}