using System;

namespace LanguageWeaverProvider.Extensions
{
	public class LoginEventArgs : EventArgs
	{
		public string Message { get; }

		public LoginEventArgs(string customMessage)
		{
			Message = customMessage;
		}
	}
}