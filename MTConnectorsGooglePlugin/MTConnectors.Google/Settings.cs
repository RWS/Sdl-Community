using Sdl.LanguagePlatform.MTConnectors.Google.GoogleService;
using Sdl.LanguagePlatform.MTConnectors.Google.Interfaces;
using Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.Text;

namespace Sdl.LanguagePlatform.MTConnectors.Google
{
    [Serializable]
    public class Settings : IGoogleSettings
    {
        private const string GoogleUriString = "googlemt://";
        private TranslationProviderUriBuilder _uriBuilder;

        public Settings()
            : this(null, null)
        {
        }

        public Settings(Uri uri)
        {
            InitializeURIBuilder(uri);
        }

        public Settings(string apiKey, string name)
        {
            InitializeURIBuilder(null);

            this.UserKey = name;
            this.ApiKey = apiKey;
        }

        public Settings(Settings settings)
        {
            InitializeURIBuilder(settings.Uri);
            ApiKey = settings.ApiKey;
            UserKey = settings.UserKey;
        }

        public string ApiKey { get; set; }

        public string UserKey
        {
            get
            {
                if (string.IsNullOrEmpty(_uriBuilder.UserName))
                {
                    return string.Empty;
                }

                return _uriBuilder.UserName;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _uriBuilder.UserName = value;
                    _uriBuilder["UserKey"] = value;
                }
                else
                {
                    _uriBuilder.UserName = string.Empty;
                    _uriBuilder["UserKey"] = null;
                }
            }
        }

        public string BaseUri
        {
            get { return URLs.TranslateUrl; }
        }

        /// <summary>
        /// Gets / sets the machine translation model used by Google Cloud to perform the traslation
        /// </summary>
        public MachineTranslationModel TranslationModel
        {
            get
            {
                var model = _uriBuilder["Model"];
                if (string.IsNullOrWhiteSpace(model))
                {
                    return MachineTranslationModel.Neural;
                }
                MachineTranslationModel modelValue;
                Enum.TryParse(model, out modelValue);
                return modelValue;
            }
            set { _uriBuilder["Model"] = value.ToString(); }
        }

        /// <summary>
        /// Gets a Uri that can be used as a <see cref="ITranslationProvider.Uri"/> value.  NB: This Uri will not contain the private ApiKey.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return _uriBuilder.Uri;
            }
        }

        /// <summary>
        /// Gets a Uri that can be used for accessing credentials on a <see cref="ITranslationProviderCredentialStore"/> interface.
        /// </summary>
        public Uri CredentialsUri
        {
            get
            {
                return new Uri(GoogleUriString);
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            Settings other = obj as Settings;

            if (other == null)
            {
                return false;
            }

            if (!Equals(_uriBuilder, other._uriBuilder))
            {
                return false;
            }

            if (!Equals(ApiKey, other.ApiKey))
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hashString = new StringBuilder();
            hashString.Append(_uriBuilder);
            hashString.Append("#" + ApiKey);
            return hashString.ToString().GetHashCode();
        }

        /// <summary>
        /// Sets up the uri builder handling the case of the old google uri format (googlemt:///)
        /// </summary>
        /// <param name="uri"></param>
        private void InitializeURIBuilder(Uri uri)
        {
            if (uri == null || string.Equals(uri.ToString(), "googlemt:///"))
            {
                // If the uri is in the old format that change it to the new format
                _uriBuilder = new TranslationProviderUriBuilder(GoogleTranslationProvider.GetUriScheme);
                Uri parsedBaseUri = new Uri(BaseUri);
                _uriBuilder.Protocol = parsedBaseUri.Scheme;
                _uriBuilder.HostName = parsedBaseUri.Host;
                _uriBuilder.Port = parsedBaseUri.Port;
            }
            else
            {
                _uriBuilder = new TranslationProviderUriBuilder(uri);
            }
        }
    }
}
