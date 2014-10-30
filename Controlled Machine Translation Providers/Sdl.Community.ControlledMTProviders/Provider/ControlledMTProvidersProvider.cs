using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Newtonsoft.Json;

namespace Sdl.Community.ControlledMTProviders.Provider
{
    public class ControlledMTProvidersProvider : ITranslationProvider
    {
        internal const string ProviderUriScheme = "sdlcommunityMMTP";
        internal const string ProviderUri = ProviderUriScheme + "://";

        private IList<ITranslationProvider> mtProviders;

        private IList<Uri> mtProvidersUri;

        public IList<ITranslationProvider> MTProviders
        {
            get { return mtProviders; }

            set
            {
                mtProviders = value;
            }
        }

        public ControlledMTProvidersProvider():this(new List<ITranslationProvider>())
        {

        }

        public ControlledMTProvidersProvider(List<ITranslationProvider> providers)
        {
            mtProviders = providers;
        }

        public IList<Uri> GetDefaultMTProvidersUri()
        {
            return new List<Uri>
            {
                new Uri("beglobalcommunity://"),
                new Uri("googlemt://"),
                new Uri("msmt://"),
                new Uri("sdlets://"),
                new Uri("languagecloud://"),
                new Uri("apertium://"),
                new Uri("languageweavermt://")
            };
        }

        public IList<Uri> GetAllMTProvidersUri()
        {
            mtProvidersUri = GetDefaultMTProvidersUri();

            foreach (var mtProvider in mtProviders)
            {
                if (mtProvider.TranslationMethod == LanguagePlatform.TranslationMemoryApi.TranslationMethod.MachineTranslation)
                {
                    if (!mtProvidersUri.Any(uri => uri.Scheme.Equals(mtProvider.Uri.Scheme)))
                    {
                        mtProvidersUri.Add(mtProvider.Uri);
                    }
                }
            }

            return mtProvidersUri;
        }

        public IList<Uri> GetSelectedMTProvidersUri()
        {
            List<Uri> providersUri = new List<Uri>();

            foreach (var mtProvider in mtProviders)
            {
                providersUri.Add(mtProvider.Uri);
            }

            return providersUri;
        }

        #region ITranslationProvider Members

        public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new CascadeLanguageDirection(this, languageDirection);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public void LoadState(string translationProviderState)
        {
            foreach (var provider in mtProviders)
            {
                provider.LoadState(translationProviderState);
            }
        }

        public string Name
        {
            get
            {
                return string.Format("Controlled MT Providers: {0}", string.Join(",", mtProviders.Select(provider => provider.Name).ToArray()));
            }
        }

        public void RefreshStatusInfo()
        {
            foreach (var provider in mtProviders)
            {
                provider.RefreshStatusInfo();
            }
        }

        public string SerializeState()
        {
            List<ProviderUriInfo> puis = new List<ProviderUriInfo>();
            foreach (var provider in mtProviders)
            {
                ProviderUriInfo pui = new ProviderUriInfo
                {
                    Name = provider.Name,
                    Uri = provider.Uri,
                    SerializedState = provider.SerializeState()

                };

                puis.Add(pui);
            }
            return JsonConvert.SerializeObject(puis);
        }

        public ProviderStatusInfo StatusInfo
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (var provider in mtProviders)
                {
                    var providerInfo = provider.StatusInfo;
                    sb.AppendLine(providerInfo.StatusMessage);
                }
              
                return new ProviderStatusInfo(true, sb.ToString());
            }
        }

        public bool SupportsConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsDocumentSearches
        {
            get { return true; }
        }

        public bool SupportsFilters
        {
            get { return true; }
        }

        public bool SupportsFuzzySearch
        {
            get { return true; }
        }

        public bool SupportsLanguageDirection(LanguagePair languageDirection)
        {
           return mtProviders.All(provider => provider.SupportsLanguageDirection(languageDirection));
        }

        public bool SupportsMultipleResults
        {
            get { return true; }
        }

        public bool SupportsPenalties
        {
            get { return true; }
        }

        public bool SupportsPlaceables
        {
            get { return true; }
        }

        public bool SupportsScoring
        {
            get { return true; }
        }

        public bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        public bool SupportsSourceConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsStructureContext
        {
            get { return true; }
        }

        public bool SupportsTaggedInput
        {
            get { return true; }
        }

        public bool SupportsTargetConcordanceSearch
        {
            get { return true; }
        }

        public bool SupportsTranslation
        {
            get { return true; }
        }

        public bool SupportsUpdate
        {
            get { return true; }
        }

        public bool SupportsWordCounts
        {
            get { return true; }
        }

        public TranslationMethod TranslationMethod
        {
            get { return LanguagePlatform.TranslationMemoryApi.TranslationMethod.MachineTranslation; }
        }

        public Uri Uri
        {
            get { return new Uri(ProviderUri); }
        }

        #endregion

       
    }

    internal class ProviderUriInfo
    {
        public string Name { get; set; }

        public Uri Uri { get; set; }

        public string SerializedState { get; set; }
    }
}

