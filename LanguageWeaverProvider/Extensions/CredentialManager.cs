using System;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider.Extensions
{
	public static class CredentialManager
	{
		public static void GetCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions)
		{
			GetCredentials(credentialStore, translationOptions, PluginVersion.LanguageWeaverCloud);
			GetCredentials(credentialStore, translationOptions, PluginVersion.LanguageWeaverEdge);
		}

		public static void GetCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions, PluginVersion pluginVersion)
		{
			if (pluginVersion == PluginVersion.None)
			{
				return;
			}

			try
			{
				var isCloudService = pluginVersion == PluginVersion.LanguageWeaverCloud;
				var scheme = isCloudService ? Constants.CloudFullScheme : Constants.EdgeFullScheme;
				var uri = new Uri(scheme);
				var translationProviderCredential = credentialStore.GetCredential(uri);
				if (isCloudService)
				{
					var cloudCredentials = JsonConvert.DeserializeObject<CloudCredentials>(translationProviderCredential.Credential);
					translationOptions.CloudCredentials = cloudCredentials;
				}
				else
				{
					var edgeCredentials = JsonConvert.DeserializeObject<EdgeCredentials>(translationProviderCredential.Credential);
					translationOptions.EdgeCredentials = edgeCredentials;
				}
			}
			catch { }
		}

		public static void UpdateCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions)
		{
			if (translationOptions.PluginVersion == PluginVersion.None)
			{
				return;
			}

			var credentials = translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
							? JsonConvert.SerializeObject(translationOptions.CloudCredentials)
							: JsonConvert.SerializeObject(translationOptions.EdgeCredentials);
			var translationProviderCredential = new TranslationProviderCredential(credentials, true);

			credentialStore.RemoveCredential(translationOptions.Uri);
			credentialStore.AddCredential(translationOptions.Uri, translationProviderCredential);
		}

		public static async void ValidateToken(ITranslationOptions translationOptions)
		{
			if (translationOptions.AuthenticationType == AuthenticationType.CloudSSO
			 && IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
			{
				await CloudService.RefreshToken(translationOptions.AccessToken);
				return;
			}
		}

		private static bool IsTimestampExpired(double unixTimeStamp)
		{
			var expirationTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(unixTimeStamp);
			var currentTime = DateTimeOffset.UtcNow;

			return expirationTime <= currentTime;
		}
	}
}