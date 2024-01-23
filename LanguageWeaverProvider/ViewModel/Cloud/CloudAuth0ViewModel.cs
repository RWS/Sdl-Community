using System;
using System.Threading.Tasks;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudAuth0ViewModel : BaseViewModel
	{
		readonly ITranslationOptions _translationOptions;

		public CloudAuth0ViewModel(ITranslationOptions translationOptions, CloudAuth0Config auth0Config)
		{
			Auth0Config = auth0Config;
			_translationOptions = translationOptions;
		}

		public CloudAuth0Config Auth0Config { get; set; }

		public bool IsConnected { get; set; }

		public delegate void CloseAuth0Raiser();

		public event CloseAuth0Raiser CloseAuth0Raised;

		public async Task Navigated(string uri)
		{
			var (success, error) = await CloudService.AuthenticateSSOUser(_translationOptions, Auth0Config, new Uri(uri), Auth0Config.PortalRegion);
			IsConnected = success;
			if (!success)
			{
				error?.ShowDialog("SSO Error", error.Message, true);
				CloseAuth0Raised?.Invoke();
				return;
			}

			_translationOptions.AuthenticationType = AuthenticationType.CloudSSO;
			CloseAuth0Raised?.Invoke();
		}
	}
}