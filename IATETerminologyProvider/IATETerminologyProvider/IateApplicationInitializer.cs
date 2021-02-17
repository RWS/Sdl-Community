using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.IATETerminologyProvider
{
	[ApplicationInitializer]
	public class IateApplicationInitializer: IApplicationInitializer
	{
		public static HttpClient Clinet = new HttpClient();
		private static readonly AccessTokenService AccessTokenService = new AccessTokenService();
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		public async void Execute()
		{
			Log.Setup();
			Logger.Info("-->IATE Application Initializer");
			InitializeHttpClientSettings();

			//If IATE service is unavailable an error will be thrown in Studio, and studio will shut down without try/catch
			try
			{
				Logger.Info("-->Try to get domains");

				var domainService = new DomainService();
				var termTypeService = new TermTypeService();
				await domainService.GetDomains();
				await termTypeService.GetTermTypes();
			}
			catch (Exception e)
			{
				Logger.Error(e);
			}
		}

		public static void SetAccessToken()
		{
			Logger.Info("-->IATE Set token");

			RefreshAccessToken();
			if (!string.IsNullOrEmpty(AccessTokenService.AccessToken))
			{
				Clinet.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessTokenService.AccessToken);
			}
		}

		private void InitializeHttpClientSettings()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
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
					Logger.Error(PluginResources.TermSearchService_Error_in_requesting_access_token);
				}
			}
			else if (AccessTokenService.AccessTokenExpired && !AccessTokenService.AccessTokenExtended)
			{
				var success = AccessTokenService.ExtendAccessToken();
				if (!success)
				{
					Logger.Error(PluginResources.TermSearchService_Error_in_refreshing_access_token);
				}
			}
		}
	}
}
