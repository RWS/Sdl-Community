using System;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider.Extensions
{
	public static class CredentialManager
	{
		private const string CredentialsKey = "credentials";
		private const string TokenKey = "accessToken";

		public static void GetCredentials(ITranslationOptions translationOptions, bool assignAccessToken = false)
		{
			var credentialStore = ApplicationInitializer.CredentialStore;

			var getCloudToken = assignAccessToken && translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud;
			var getEdgeToken = assignAccessToken && translationOptions.PluginVersion == PluginVersion.LanguageWeaverEdge;

			GetAndAssignCredentials<CloudCredentials>(credentialStore, translationOptions, Constants.CloudFullScheme, getCloudToken);
			GetAndAssignCredentials<EdgeCredentials>(credentialStore, translationOptions, Constants.EdgeFullScheme, getEdgeToken);
		}

		public static void GetAndAssignCredentials<T>(ITranslationProviderCredentialStore credentialStore, ITranslationOptions translationOptions, string scheme, bool assignAccessToken = false)
		{
			if (credentialStore is null)
			{
				return;
			}

			var uri = new Uri(scheme);
			var translationProviderCredential = credentialStore.GetCredential(uri);
			if (translationProviderCredential is null
			 || translationProviderCredential.Credential is not string persistedCredentials)
			{
				return;
			}

			try
			{
				var parsedObject = JObject.Parse(persistedCredentials);
				var credentials = parsedObject[CredentialsKey].ToString();
				AssignCredentials<T>(translationOptions, parsedObject[CredentialsKey].ToString());

				if (assignAccessToken)
				{
					var accessToken = parsedObject[TokenKey].ToString();
					AssignAccessToken(translationOptions, accessToken);
				}
			}
			catch { }
		}

		private static void AssignAccessToken(ITranslationOptions translationOptions, string json)
		{
			translationOptions.AccessToken = JsonConvert.DeserializeObject<AccessToken>(json);
		}

		private static void AssignCredentials<T>(ITranslationOptions translationOptions, string credentials)
		{
			var tType = typeof(T);
			if (tType == typeof(CloudCredentials))
			{
				translationOptions.CloudCredentials = DeserializeAndCast<T>(credentials) as CloudCredentials;
			}
			else if (tType == typeof(EdgeCredentials))
			{
				translationOptions.EdgeCredentials = DeserializeAndCast<T>(credentials) as EdgeCredentials;
			}
		}

		private static T DeserializeAndCast<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
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

			var accessToken = JsonConvert.SerializeObject(translationOptions.AccessToken);

			var jsonStructure = new JObject(
				new JProperty(CredentialsKey, JToken.Parse(credentials)),
				new JProperty(TokenKey, JToken.Parse(accessToken))
			).ToString();

			var translationProviderCredential = new TranslationProviderCredential(jsonStructure, true);
			credentialStore.RemoveCredential(translationOptions.Uri);
			credentialStore.AddCredential(translationOptions.Uri, translationProviderCredential);
		}
	}
}