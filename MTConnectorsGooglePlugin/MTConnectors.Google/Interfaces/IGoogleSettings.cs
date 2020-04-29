using System;

namespace Sdl.LanguagePlatform.MTConnectors.Google.Interfaces
{
    public interface IGoogleSettings
    {
        string ApiKey { get; set; }
        string UserKey { get; set; }
        string BaseUri { get; }

        /// <summary>
        /// Gets / sets the machine translation model used by Google Cloud to perform the traslation
        /// </summary>
        MachineTranslationModel TranslationModel { get; set; }

        /// <summary>
        /// Gets a Uri that can be used as a <see cref="System.Uri"/> value.  NB: This Uri will not contain the private ApiKey.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets a Uri that can be used for accessing credentials on a <see cref="ITranslationProviderCredentialStore"/> interface.
        /// </summary>
        Uri CredentialsUri { get; }
    }
}