using System;
using System.IO;
using System.Net;
using System.Reflection;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
	public class GroupShareCredentialManager
	{
		private readonly MethodInfo _getUserCredentialsMethod;
		private readonly Type _identityInfoCacheHelpers;
		private readonly dynamic _serverLogon;

		public GroupShareCredentialManager()
		{
			var assembly = Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.Desktop.Platform.ServerConnectionPlugin.dll"));
			var serverLogonType = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.ServerLogon");
			_identityInfoCacheHelpers = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.Implementation.IdentityInfoCacheHelpers");
			_getUserCredentialsMethod = _identityInfoCacheHelpers.GetMethod("GetUserCredentials");
			_serverLogon = Activator.CreateInstance(serverLogonType);
		}

		

		public CredentialKind GetCredentialType()
		{
			if (_serverLogon.IsWindowsUser && string.IsNullOrEmpty(_serverLogon.Password) && string.IsNullOrEmpty(_serverLogon.UserName))
				return CredentialKind.WindowsSSO;

			if (!_serverLogon.IsWindowsUser && string.IsNullOrEmpty(_serverLogon.Password) && !string.IsNullOrEmpty(_serverLogon.UserName))
				return CredentialKind.SSO;

			return CredentialKind.Normal;
		}

		//public TranslationProviderServer GetCredentials(Credentials credentials)
		//{

		//}

		public TranslationProviderServer TryGetCredentialsWithoutUserInput(Credentials credentials)
		{

		}

		public TranslationProviderServer Login(Credentials credentials)
		{
			_serverLogon.ServerUrl = credentials.Url;
			var dialogResult = _serverLogon.ShowLogonDialog(null);

			if (!dialogResult)
				return null;

			credentials.Url = _serverLogon.ServerUrl;
			credentials.UserName = _serverLogon.UserName;
			credentials.Password = _serverLogon.Password;

			var serviceUri = new Uri(_serverLogon.ServerUrl);
			switch (GetCredentialType())
			{
				case CredentialKind.Normal:
					return new TranslationProviderServer(serviceUri, _serverLogon.IsWindowsUser, _serverLogon.UserName, _serverLogon.Password);

				case CredentialKind.WindowsSSO:
					var windowsSso = new WindowsSsoData(_serverLogon.ServerUrl);
					if (windowsSso.StatusCode != HttpStatusCode.OK)
						return null;
					return new TranslationProviderServer(serviceUri, windowsSso.UserName,
						windowsSso.Token, DateTime.MaxValue);

				case CredentialKind.SSO:
					var data = GetDataFromCache(serviceUri);
					return new TranslationProviderServer(serviceUri, data.UserName, data.AuthToken, DateTime.MaxValue);

				default:
					throw new Exception("credentials");
			}
		}

		private static string ExecutingStudioLocation()
		{
			var entryAssembly = Assembly.GetEntryAssembly()?.Location;
			var location = entryAssembly?.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

			return location;
		}

		private dynamic GetDataFromCache(Uri serviceUri)
		{
			dynamic cachedCredentials =
				_getUserCredentialsMethod.Invoke(_identityInfoCacheHelpers, new object[] { serviceUri });
			return cachedCredentials;
		}
	}
}