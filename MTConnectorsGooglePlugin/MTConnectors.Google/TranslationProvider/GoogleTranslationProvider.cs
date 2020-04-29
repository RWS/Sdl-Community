using Sdl.LanguagePlatform.MTConnectors.Google.DataContracts;
using Sdl.LanguagePlatform.MTConnectors.Google.GoogleService;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider
{
    public class GoogleTranslationProvider : AbstractMachineTranslationProvider
    {
        #region Variables
        internal const bool PlainTextOnly = false;
        internal static readonly string GoogleUriScheme = "googlemt";

        internal static readonly string Version = "2.0";

        private List<GoogleLanguageDirection> _languageDirections;
        private List<Core.LanguagePair> _supportedLanguages = null;
        private Settings _settings;
        private GoogleTranslationService _service;
        #endregion

        #region  Constructor
        public GoogleTranslationProvider(Settings settings)
            : this(settings, null)
        {
        }

        public GoogleTranslationProvider(Settings settings, IEnumerable<Core.LanguagePair> languageDirections)
        {
            Initialize(settings);
            var queryRequestBuilder = new QueryRequestBuilder(settings);
            _service = new GoogleTranslationService(queryRequestBuilder);

            _languageDirections = new List<GoogleLanguageDirection>();
            if (languageDirections != null)
            {
                foreach (Core.LanguagePair ld in languageDirections)
                {
                    GetLanguageDirection(ld);
                }
            }
        }
        #endregion

        #region Properties
        public Settings Settings
        {
            get { return _settings; }
            set { UpdateSettings(value); }
        }

        /// <inheritdoc/>
        public override bool SupportsSearchForTranslationUnits
        {
            get { return true; }
        }

        public override Uri Uri
        {
            get
            {
                if (_settings == null)
                {
                    throw new InvalidOperationException(PluginResources.Google_Uninitialized);
                }

                return _settings.Uri;
            }
        }

        public override string Name => string.Concat(GetTranslationProviderName(), GetConnectionInstanceName());

        public override bool SupportsTaggedInput => true;


        public override IList<Core.LanguagePair> SupportedLanguageDirections
        {
            get
            {
                if (_supportedLanguages == null)
                {
                    ProviderStatusInfo info = GetStatusInfo();
                    if (info.Available)
                    {
                        _supportedLanguages = _service.GetSupportedLanguageDirections();
                    }
                }

                return _supportedLanguages;
            }
        }

        public IList<Core.LanguagePair> CurrentLanguageDirections
        {
            get { return _languageDirections.Select(x => x.LanguageDirection).ToList(); }
        }

        internal static string GetUriScheme => GoogleUriScheme;
        #endregion

        #region Methods
        public void UpdateSettings(Settings settings)
        {
            Initialize(settings);
        }

        public bool IsAPIKeyValid()
        {
            return _service.IsConnectionAndAPIKeyValid();
        }

        public override ITranslationProviderLanguageDirection GetLanguageDirection(Core.LanguagePair languageDirection)
        {
            foreach (GoogleLanguageDirection ld in _languageDirections)
            {
                if (ld.LanguageDirection.IsCompatible(languageDirection))
                {
                    return ld;
                }
            }

            var newLd = new GoogleLanguageDirection(this, _settings, languageDirection);
            _languageDirections.Add(newLd);
            return newLd;
        }

        public override bool SupportsLanguageDirection(Core.LanguagePair languageDirection)
        {
            IList<Core.LanguagePair> supported = SupportedLanguageDirections;
            if (supported != null)
            {
                foreach (Core.LanguagePair ld in supported)
                {
                    if (_service.AreCompatible(languageDirection, ld))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static string GetTranslationProviderName()
        {
            return "Google Cloud Translation API";
        }

        protected override ProviderStatusInfo GetStatusInfo()
        {
            return new ProviderStatusInfo(true, "OK");
        }

        private void Initialize(Settings settings)
        {
            _settings = settings ?? throw new ArgumentNullException();
        }

        private string GetConnectionInstanceName()
        {
            if (_settings != null && !string.IsNullOrEmpty(_settings.UserKey))
            {
                return string.Concat("(", _settings.UserKey, ")");
            }

            return string.Empty;
        }
        #endregion
    }
}
