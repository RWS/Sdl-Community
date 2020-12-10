using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider
{
	[ApplicationInitializer]
	public class IateApplicationInitializer: IApplicationInitializer
	{
		public static HttpClient Clinet = new HttpClient();
		private static readonly AccessTokenService AccessTokenService = new AccessTokenService();

		public void Execute()
		{
			var eventAggregator = SdlTradosStudio.Application.GetService<IStudioEventAggregator>();
			eventAggregator.GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
			InitializeHttpClientSettings();
		}

		public static void SetAccessToken()
		{
			RefreshAccessToken();
			if (!string.IsNullOrEmpty(AccessTokenService.AccessToken))
			{
				Clinet.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessTokenService.AccessToken);
			}
		}

		private async void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent e)
		{
			var domanService = new DomainService();
			var termTypeService = new TermTypeService();
			await domanService.GetDomains();
			await termTypeService.GetTermTypes();
		}

		private void InitializeHttpClientSettings()
		{
			Utils.AddDefaultParameters(Clinet);
			SetAccessToken();
		}

		private static void RefreshAccessToken()
		{
			if (AccessTokenService.RefreshTokenExpired
			    || AccessTokenService.RequestedAccessToken == DateTime.MinValue
			    || string.IsNullOrEmpty(AccessTokenService.AccessToken))
			{
				var success = AccessTokenService.GetAccessToken("SDL_PLUGIN", "E9KWtWahXs4hvE9z");
				if (!success)
				{
					throw new Exception(PluginResources.TermSearchService_Error_in_requesting_access_token);
				}
			}
			else if (AccessTokenService.AccessTokenExpired && !AccessTokenService.AccessTokenExtended)
			{
				var success = AccessTokenService.ExtendAccessToken();
				if (!success)
				{
					throw new Exception(PluginResources.TermSearchService_Error_in_refreshing_access_token);
				}
			}
		}
	}
}
