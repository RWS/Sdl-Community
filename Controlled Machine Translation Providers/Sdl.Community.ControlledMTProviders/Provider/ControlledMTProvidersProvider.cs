using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ControlledMTProviders.Provider
{
	public class ControlledMtProvidersProvider : ITranslationProvider
    {
        internal const string ProviderUriScheme = "sdlcommunityMMTP";
        internal const string ProviderUri = ProviderUriScheme + "://";

        private IList<ITranslationProvider> _mtProviders;

        private IList<Uri> _mtProvidersUri;

        public IList<ITranslationProvider> MtProviders
        {
            get { return _mtProviders; }

            set
            {
                _mtProviders = value;
            }
        }

        public ControlledMtProvidersProvider():this(new List<ITranslationProvider>())
        {

        }

        public ControlledMtProvidersProvider(List<ITranslationProvider> providers)
        {
            _mtProviders = providers;
        }

        public IList<Uri> GetDefaultMtProvidersUri()
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

        public IList<Uri> GetAllMtProvidersUri()
        {
            _mtProvidersUri = GetDefaultMtProvidersUri();

			foreach (var mtProvider in _mtProviders)
			{
				if (!_mtProvidersUri.Any(uri => uri.Scheme.Equals(mtProvider.Uri.Scheme)))
				{
					_mtProvidersUri.Add(mtProvider.Uri);
				}
			}

            return _mtProvidersUri;
        }

        public IList<Uri> GetSelectedMtProvidersUri()
        {
            return _mtProviders.Select(mtProvider => mtProvider.Uri).ToList();
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
            foreach (var provider in _mtProviders)
            {
                provider.LoadState(translationProviderState);
            }
        }

        public string Name
        {
            get
            {
                return string.Format("Controlled MT Providers: {0}", string.Join(",", _mtProviders.Select(provider => provider.Name).ToArray()));
            }
        }

        public void RefreshStatusInfo()
        {
            foreach (var provider in _mtProviders)
            {
                provider.RefreshStatusInfo();
            }
        }

        public string SerializeState()
        {
            var puis = _mtProviders.Select(provider => new ProviderUriInfo
            {
                Name = provider.Name, Uri = provider.Uri, SerializedState = provider.SerializeState()
            }).ToList();
            return JsonConvert.SerializeObject(puis);
        }

        public ProviderStatusInfo StatusInfo
        {
            get
            {
                var sb = new StringBuilder();

                foreach (var provider in _mtProviders)
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
           return _mtProviders.All(provider => provider.SupportsLanguageDirection(languageDirection));
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
            get { return TranslationMethod.MachineTranslation; }
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

