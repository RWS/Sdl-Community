using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
    public class GroupshareCredentialManager
    {
        private readonly MethodInfo _getUserCredentialsMethod;
        private readonly Type _identityInfoCacheHelpers;
        private readonly dynamic _serverLogon;

        public GroupshareCredentialManager()
        {
            var assembly = Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.Desktop.Platform.ServerConnectionPlugin.dll"));
            var serverLogonType = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.ServerLogon");
            _identityInfoCacheHelpers = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.Implementation.IdentityInfoCacheHelpers");
            _getUserCredentialsMethod = _identityInfoCacheHelpers.GetMethod("GetUserCredentials");
            _serverLogon = Activator.CreateInstance(serverLogonType);
        }

        public CredentialKind GetCredentialTypeFromServerLogon()
        {
            if (_serverLogon.IsWindowsUser && string.IsNullOrEmpty(_serverLogon.Password) && string.IsNullOrEmpty(_serverLogon.UserName))
                return CredentialKind.WindowsSSO;

            if (!_serverLogon.IsWindowsUser && string.IsNullOrEmpty(_serverLogon.Password) && !string.IsNullOrEmpty(_serverLogon.UserName))
                return CredentialKind.SSO;

            return CredentialKind.Normal;
        }

        public TranslationProviderServer GetProvider(Credentials credentials)
        {
            _serverLogon.ServerUrl = credentials.Url;

            var dialogResult = _serverLogon.ShowLogonDialog(null);

            if (!dialogResult) return null;

            var credentialTypeFromServerLogon = GetCredentialTypeFromServerLogon();

            credentials.Url = _serverLogon.ServerUrl;
            credentials.UserName = _serverLogon.UserName;
            credentials.Password = _serverLogon.Password;
            credentials.CredentialType = credentialTypeFromServerLogon;

            var serviceUri = new Uri(_serverLogon.ServerUrl);
            switch (credentialTypeFromServerLogon)
            {
                case CredentialKind.Normal:
                    return new TranslationProviderServer(serviceUri, _serverLogon.IsWindowsUser, _serverLogon.UserName, _serverLogon.Password);

                case CredentialKind.WindowsSSO:
                    var windowsSso = new WindowsSsoData(_serverLogon.ServerUrl);
                    if (windowsSso.StatusCode != HttpStatusCode.OK) return null;

                    credentials.UserName = windowsSso.UserName;
                    credentials.Token = windowsSso.Token;

                    return new TranslationProviderServer(serviceUri, windowsSso.UserName,
                        windowsSso.Token, DateTime.MaxValue);

                case CredentialKind.SSO:
                    var cachedCredentials = GetDataFromCache(serviceUri);
                    credentials.UserName = cachedCredentials.UserName;
                    credentials.Token = cachedCredentials.AuthToken;

                    return new TranslationProviderServer(serviceUri, cachedCredentials.UserName, cachedCredentials.AuthToken, cachedCredentials.ExpirationDate);

                default:
                    throw new Exception(PluginResources.GroupShareCredentialManager_TryGetCredentialsWithoutUserInput_Credentials_invalid_);
            }
        }

        public TranslationProviderServer TryGetProviderWithoutUserInput(Credentials credentials)
        {
            var cachedCredentials = GetDataFromCache(new Uri(credentials.Url));

            switch (credentials.CredentialType)
            {
                case CredentialKind.Normal:

                    credentials.UserName = cachedCredentials.UserName;
                    credentials.Password = cachedCredentials.Password;

                    return new TranslationProviderServer((new Uri(credentials.Url)),
                        cachedCredentials.UserType.ToString() == "WindowsUser",
                        cachedCredentials.UserName, cachedCredentials.Password);

                case CredentialKind.SSO:

                    credentials.UserName = cachedCredentials.UserName;
                    credentials.Token = cachedCredentials.AuthToken;

                    return new TranslationProviderServer(new Uri(credentials.Url), cachedCredentials.UserName,
                        cachedCredentials.AuthToken, cachedCredentials.ExpirationDate);

                case CredentialKind.WindowsSSO:

                    var windowsSso = new WindowsSsoData(credentials.Url);

                    if (windowsSso.StatusCode != HttpStatusCode.OK) return null;

                    credentials.UserName = windowsSso.UserName;
                    credentials.Token = string.IsNullOrWhiteSpace(credentials.Token) ? windowsSso.Token : credentials.Token;

                    return new TranslationProviderServer((new Uri(credentials.Url)), credentials.UserName,
                        credentials.Token, DateTime.MaxValue);

                default:
                    throw new Exception(PluginResources
                        .GroupShareCredentialManager_TryGetCredentialsWithoutUserInput_Credentials_invalid_);
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