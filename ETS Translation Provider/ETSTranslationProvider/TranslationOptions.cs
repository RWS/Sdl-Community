using ETSLPConverter;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;

namespace ETSTranslationProvider
{
    public class TranslationOptions
    {
        public static readonly TranslationMethod ProviderTranslationMethod = TranslationMethod.MachineTranslation;

        TranslationProviderUriBuilder _uriBuilder;

        public TranslationOptions()
        {
            _uriBuilder = new TranslationProviderUriBuilder(TranslationProvider.TranslationProviderScheme);
            UseBasicAuthentication = true;
            Port = 8001;
            LPPreferences = new Dictionary<CultureInfo, ETSApi.ETSLanguagePair>();
        }

        public TranslationOptions(Uri uri) : this()
        {
            _uriBuilder = new TranslationProviderUriBuilder(uri);
        }

        public Dictionary<CultureInfo, ETSApi.ETSLanguagePair> LPPreferences { get; private set; }

        public bool PersistCredentials { get; set; }

        [JsonIgnore]
        public string ApiToken { get; set; }

        public bool UseBasicAuthentication { get; set; }

        #region URI Properties
        public string Host
        {
            get
            {
                return _uriBuilder.HostName;
            }
            set
            {
                _uriBuilder.HostName = value;
                ResolvedHost = null;
            }
        }

        private string ResolvedHost { get; set; }

        private string ResolveHost()
        {
            if (ResolvedHost != null)
                return ResolvedHost;

            // If the host is an IP address, preserve that, otherwise get the DNS host and cache it.
            IPAddress address;
            ResolvedHost = IPAddress.TryParse(Host, out address) ? Host : Dns.GetHostEntry(Host).HostName;
            return ResolvedHost;
        }

        public ETSApi.TradosToETSLP[] SetPreferredLanguages(LanguagePair[] languagePairs)
        {
            ETSApi.ETSLanguagePair[] etsLanguagePairs = ETSApi.ETSTranslatorHelper.GetLanguagePairs(this);
            if (!etsLanguagePairs.Any())
                return null;

            ETSApi.TradosToETSLP[] languagePairChoices = languagePairs.GroupJoin(
                etsLanguagePairs,
                requestedLP =>
                    new
                    {
                        SourceLanguageId = requestedLP.SourceCulture.ToETSCode(),
                        TargetLanguageId = requestedLP.TargetCulture.ToETSCode()
                    },
                installedLP =>
                    new
                    {
                        SourceLanguageId = installedLP.SourceLanguageId,
                        TargetLanguageId = installedLP.TargetLanguageId
                    },
                (requestedLP, installedLP) =>
                    new ETSApi.TradosToETSLP(
                        tradosCulture: requestedLP.TargetCulture, 
                        etsLPs: installedLP.OrderBy(lp => lp.LanguagePairId).ToList())
                ).ToArray();

            // Empty out the previous LP choices.
            foreach (var lpChoice in languagePairChoices)
            {
                // By default, select the preferences to be the first LP of each set.
                ETSApi.ETSLanguagePair defaultOption = lpChoice.ETSLPs.FirstOrDefault();
                // Verify that the preferred LP still exists on ets server and if not, remove it from preferences
                if (LPPreferences.ContainsKey(lpChoice.TradosCulture) &&
                    !lpChoice.ETSLPs.Contains(LPPreferences[lpChoice.TradosCulture]))
                    LPPreferences.Remove(lpChoice.TradosCulture);

                if (defaultOption != null && !LPPreferences.ContainsKey(lpChoice.TradosCulture))
                    LPPreferences[lpChoice.TradosCulture] = defaultOption;
            }
            return languagePairChoices;
        }

        public int Port
        {
            get
            {
                return _uriBuilder.Port;
            }
            set
            {
                _uriBuilder.Port = value;
            }
        }
        public ETSApi.APIVersion APIVersion { get; set; }

        public string APIVersionString
        {
            get
            {
                return APIVersion == ETSApi.APIVersion.v1 ? "v1" : "v2";
            }
        }

        public Uri Uri
        {
            get
            {
                UriBuilder resolvedUri = new UriBuilder(_uriBuilder.Uri);
                resolvedUri.Host = ResolveHost();
                return resolvedUri.Uri;
            }
        }
        #endregion
    }
}
