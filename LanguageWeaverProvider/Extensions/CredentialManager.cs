using LanguageWeaverProvider.Model;
using System;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using LanguageWeaverProvider.Services;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Extensions
{
	public static class CredentialManager
	{
		public static void SetCredentials(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions)
		{
			var uri = new Uri(Constants.CloudFullScheme);
			var translationProviderCredential = credentialStore.GetCredential(uri);
			if (translationProviderCredential is null
			 || translationProviderCredential.Credential is null)
			{
				return;
			}

			var cloudCredentials = JsonConvert.DeserializeObject<CloudCredentials>(translationProviderCredential.Credential);
			translationOptions.CloudCredentials = cloudCredentials;
		}

		public static async void ValidateToken(ITranslationOptions translationOptions)
		{
			if (IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
			{
				await CloudService.AuthenticateUser(translationOptions.CloudCredentials, translationOptions, translationOptions.AuthenticationType);
			}
		}

		static bool IsTimestampExpired(double unixTimeStamp)
		{
			DateTimeOffset expirationTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(unixTimeStamp);
			DateTimeOffset currentTime = DateTimeOffset.UtcNow;

			return expirationTime <= currentTime;
		}
	}
}