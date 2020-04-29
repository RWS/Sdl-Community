using System.Collections.Generic;
using Sdl.LanguagePlatform.MTConnectors.Google.TranslationProvider;

namespace Sdl.LanguagePlatform.MTConnectors.Google
{
    /// <summary>
    /// Implements a IPluginBasedTranslationProvider interface for the <see cref="GoogleTranslationProvider"/> class so that
    /// it can be used as a Translation Provider plug-in.
    /// </summary>
    public class GoogleTranslationProviderPlugin : GoogleTranslationProvider
    {
        private System.Drawing.Image _cachedSearchResultImage;
        private System.Drawing.Icon _cachedCascadeIcon;

        ///// <summary>
        ///// A public constructor to allow creation by the <see cref="GoogleTranslationProviderFactoryPlugin"/> factory.
        ///// </summary>
        ///// <param name="languageDirections">The language pairs to be used initially for this translation provider.</param>
        public GoogleTranslationProviderPlugin(Settings settings, IEnumerable<Core.LanguagePair> languageDirections)
            : base(settings, languageDirections)
        {
        }

        public GoogleTranslationProviderPlugin(Settings settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Returns the Name of the Google MT plug-in
        /// </summary>
        public override string Name
        {
            get
            {
                return GetTranslationProviderName();
            }
        }

        #region IPluginBasedTranslationProvider Members

        /// <summary>
        /// Implements <see cref="IPluginBasedTranslationProvider.PluginFactoryId"/>.
        /// </summary>
        public string PluginFactoryId
        {
            get { return IdValues.PluginProviderGoogleTranslatorFactory_Id; }
        }

        #endregion

        #region ISupportsImageInfo Members

        /// <summary>
        /// Returns the Google logo.
        /// </summary>
        public System.Drawing.Image SearchResultImage
        {
            // The artwork was originally obtained from:
            // http://www.google.com/uds/samples/language/branding.html
            get
            {
                if (_cachedSearchResultImage == null)
                {
                    _cachedSearchResultImage = PluginResources.google_alpha_studio;
                }

                return _cachedSearchResultImage;
            }
        }

        public System.Drawing.Icon CascadeIcon
        {
            get
            {
                if (_cachedCascadeIcon == null)
                {
                    _cachedCascadeIcon = PluginResources.favicon;
                }

                return _cachedCascadeIcon;
            }
        }

        /// <summary>
        /// <see cref="M:System.Object.Equals(object)"/>
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if the specified object is equal to the current object;
        /// otherwise, false.
        /// </returns>
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

            var other = obj as GoogleTranslationProvider;
            if (other == null)
            {
                return false;
            }

            if (!Settings.Equals(other.Settings))
            {
                return false;
            }

            return true;    // no settings yet so always true
        }

        /// <summary>
        /// <see cref="object.GetHashCode()"/>
        /// </summary>
        /// <returns>A hash code for this object</returns>
        public override int GetHashCode()
        {
            return Settings.GetHashCode();
        }
        #endregion
    }
}
