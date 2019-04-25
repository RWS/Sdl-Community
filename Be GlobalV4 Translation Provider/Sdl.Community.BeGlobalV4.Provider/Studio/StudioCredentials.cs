using System;
using System.Windows.Forms;
using Sdl.LanguageCloud.IdentityApi;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class StudioCredentials
	{
		public  string GetToken()
		{
			if (IsLoggedIn())
			{
				var instance = LanguageCloudIdentityApi.Instance;
			
				return instance.AccessToken;
			}
			return string.Empty;
		}
		[STAThread]
		public  bool IsLoggedIn(out string message)
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
		public  bool IsLoggedIn()
		{
			if (!IsLoggedIn(out var message))
			{
				MessageBox.Show(message, @"Authentication", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return false;
			}
			return true;
		}
	}
}
