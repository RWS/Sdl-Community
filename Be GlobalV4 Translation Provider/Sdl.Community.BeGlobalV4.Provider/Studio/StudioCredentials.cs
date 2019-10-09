using System;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguageCloud.IdentityApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class StudioCredentials
	{
		public string GetToken()
		{
			if (IsLoggedIn())
			{
				var instance = LanguageCloudIdentityApi.Instance;			
				return instance.AccessToken;
			}
			return string.Empty;
		}
		[STAThread]
		public bool IsLoggedIn(out string message)
		{
			message = string.Empty;
			var instance = LanguageCloudIdentityApi.Instance;
			var languageCloudCredential = instance.LanguageCloudCredential;

			if (string.IsNullOrEmpty(languageCloudCredential?.Email))
			{
				return instance.TryLogin(out message);
			}
			return true;
		}

		public string EnsureValidConnection()
		{
			var instance = LanguageCloudIdentityApi.Instance;
			instance.EnsureValidConnection();

			return instance.AccessToken;
		}
		public  bool IsLoggedIn()
		{
			if (!IsLoggedIn(out var message))
			{
				var _messageBoxService = new MessageBoxService();
				_messageBoxService.ShowInformationMessage(message, Constants.Authentication);
				return false;
			}
			return true;
		}
	}
}