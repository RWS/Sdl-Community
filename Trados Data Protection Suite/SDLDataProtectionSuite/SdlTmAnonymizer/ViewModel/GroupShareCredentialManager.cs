using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.ViewModel
{
    public class GroupshareCredentialManager
    {
        private readonly MethodInfo _getUserCredentialsMethod;
        private readonly object _identityInfoCache;
        private readonly MethodInfo _getServersMethod;

        public GroupshareCredentialManager()
        {
            var assembly = Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.Desktop.Platform.ServerConnectionPlugin.dll"));
            var identityInfoCacheAssembly = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.Client.IdentityModel.IdentityInfoCache");
            _identityInfoCache = identityInfoCacheAssembly?.GetProperty("Default", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null);
            _getUserCredentialsMethod = _identityInfoCache?.GetType().GetMethod(
                "GetUserCredentials",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                [typeof(Uri)],
                null
            );
            _getServersMethod = _identityInfoCache?.GetType().GetMethod(
                "GetServers",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Type.EmptyTypes,
                null
            );

        }

        private CredentialKind GetCredentialType(dynamic credentialData)
        {
            if (string.IsNullOrEmpty(credentialData.Password) && string.IsNullOrEmpty(credentialData.UserName))
                return CredentialKind.WindowsSSO;

            if (string.IsNullOrEmpty(credentialData.Password) && !string.IsNullOrEmpty(credentialData.UserName))
                return CredentialKind.SSO;

            return CredentialKind.Normal;
        }

        public List<TranslationProviderServerWithCredentials> GetProvider()
        {
            var servers = _getServersMethod?.Invoke(_identityInfoCache, null) as List<Uri>;

            var serverUi = new ChooseServerWindow(servers);
            var result = serverUi.ShowDialog();

            if (result is not true)
                return [];

            var tpServers = new List<TranslationProviderServerWithCredentials>();
            foreach (var server in serverUi.Servers.Where(s => s.IsSelected))
            {
                var serverUri = new Uri(server.Uri);
                var credentialData = GetDataFromCache(serverUri);

                var credentials = new Credentials
                {
                    CredentialType = GetCredentialType(credentialData),
                    UserName = credentialData.UserName,
                    Password = credentialData.Password,
                    Url = server.Uri
                };

                switch (credentials.CredentialType)
                {
                    case CredentialKind.Normal:
                        tpServers.Add(new TranslationProviderServerWithCredentials
                        {
                            Server = new TranslationProviderServer(serverUri, credentialData.IsWindowsUser,
                                credentialData.UserName, credentialData.Password),
                        });
                        break;

                    case CredentialKind.WindowsSSO:
                        var windowsSso = new WindowsSsoData(server.Uri);
                        if (windowsSso.StatusCode != HttpStatusCode.OK)
                            break;

                        tpServers.Add(new TranslationProviderServerWithCredentials
                        {
                            Server = new TranslationProviderServer(serverUri, windowsSso.UserName,
                                windowsSso.Token, DateTime.MaxValue),
                        });
                        break;

                    case CredentialKind.SSO:

                        tpServers.Add(new TranslationProviderServerWithCredentials
                        {
                            Server = new TranslationProviderServer(serverUri, credentialData.UserName,
                                credentialData.AuthToken, credentialData.ExpirationDate)
                        });
                        break;

                    default:
                        throw new Exception(PluginResources.GroupShareCredentialManager_TryGetCredentialsWithoutUserInput_Credentials_invalid_);
                }

                tpServers[tpServers.Count - 1].Credentials = credentials;
            }

            return tpServers;
        }

        public TranslationProviderServer TryGetProviderWithoutUserInput(Credentials credentials)
        {   
            var credentialData = GetDataFromCache(new Uri(credentials.Url));

            switch (credentials.CredentialType)
            {
                case CredentialKind.Normal:

                    credentials.UserName = credentialData.UserName;
                    credentials.Password = credentialData.Password;

                    return new TranslationProviderServer((new Uri(credentials.Url)),
                        credentialData.UserType.ToString() == "WindowsUser",
                        credentialData.UserName, credentialData.Password);

                case CredentialKind.SSO:

                    credentials.UserName = credentialData.UserName;
                    credentials.Token = credentialData.AuthToken;

                    return new TranslationProviderServer(new Uri(credentials.Url), credentialData.UserName,
                        credentialData.AuthToken, credentialData.ExpirationDate);

                case CredentialKind.WindowsSSO:

                    var windowsSso = new WindowsSsoData(credentials.Url);

                    if (windowsSso.StatusCode != HttpStatusCode.OK)
                        return null;

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

        private dynamic GetDataFromCache(Uri serverUri)
        {
            dynamic credentialData =
                _getUserCredentialsMethod.Invoke(_identityInfoCache, new object[] { serverUri });
            return credentialData;
        }
    }
}