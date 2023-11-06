using System;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;

namespace LanguageWeaverProvider.ViewModel.Cloud
{
	public class CloudAuth0ViewModel : BaseViewModel
	{
		private ITranslationOptions _translationOptions;

		public CloudAuth0ViewModel(ITranslationOptions translationOptions)
		{
			Auth0Config = new Auth0Config();
			_translationOptions = translationOptions;
		}

		public Auth0Config Auth0Config { get; set; }

		public delegate void CloseAuth0Raiser();

		public event CloseAuth0Raiser CloseAuth0Raised;

		public async void Navigated(string uri)
		{
			var (success, error) = await CloudService.AuthenticateSSOUser(_translationOptions, Auth0Config, new Uri(uri));
			if (!success)
			{
				ErrorHandling.ShowDialog(error, "SSO Error", "Something went wrong, couldn't authenticate", true);
				CloseAuth0Raised?.Invoke();
				return;
			}

			_translationOptions.AuthenticationType = AuthenticationType.CloudSSO;
			CloseAuth0Raised?.Invoke();
		}
	}
}