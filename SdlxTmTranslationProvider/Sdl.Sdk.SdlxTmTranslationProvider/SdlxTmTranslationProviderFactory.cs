//-----------------------------------------------------------------------
// <copyright file="SdlxTmTranslationProviderFactory.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.IO;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// The factory for creating the SDLX translation memory provider class
    /// </summary>
    [TranslationProviderFactory(
        Id = "Sdl.Sdk.SdlxTmTranslationProviderFactory",
        Name = "Sdl.Sdk.SdlxTmTranslationProviderFactory",
        Description = "Sdl.Sdk.SdlxTmTranslationProviderFactory")]
    public class SdlxTmTranslationProviderFactory : ITranslationProviderFactory
    {
        #region ITranslationProviderFactory Members

        /// <summary>
        /// Creates the translation provider.
        /// </summary>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        /// <param name="translationProviderState">State of the translation provider.</param>
        /// <param name="credentialStore">The credential store.</param>
        /// <returns>An SDLX translation memory translation provider.</returns>
        public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            string translationMemoryType = translationProviderUri.Host.ToUpper();
            string translationMemoryConnectionString = Uri.UnescapeDataString(translationProviderUri.PathAndQuery).Substring(1).Replace(@"/", @"\");
            SdlxTmTranslationProviderPlugin returnProvider = new SdlxTmTranslationProviderPlugin(translationMemoryType, translationMemoryConnectionString);
            return returnProvider;
        }

        /// <summary>
        /// Gets the translation provider info.
        /// </summary>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        /// <param name="translationProviderState">State of the translation provider.</param>
        /// <returns>A <see cref="TranslationProviderInfo"/> object.</returns>
        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            string translationMemoryType = translationProviderUri.Host.ToUpper();
            string translationMemoryConnectionString = Uri.UnescapeDataString(translationProviderUri.PathAndQuery).Substring(1).Replace(@"/", @"\");
            if (translationMemoryType == "S42TM-STD")
            {
                translationMemoryConnectionString = new FileInfo(translationMemoryConnectionString).Name;
            }

            TranslationProviderInfo myInfo = new TranslationProviderInfo();
            myInfo.Name = string.Format("SDLX TM [{0}] [{1}]", translationMemoryType.Substring(6), translationMemoryConnectionString);
            myInfo.TranslationMethod = TranslationMethod.TranslationMemory;
            return myInfo;
        }

        /// <summary>
        /// Determines whether the factory supports a particular URI scheme.
        /// </summary>
        /// <param name="translationProviderUri">The translation provider URI.</param>
        /// <returns>True for schemes that start with "sdlxtm://".</returns>
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            return translationProviderUri.Scheme.Equals(SdlxTmTranslationProvider.SdlxTmUriScheme);
        }

        #endregion
    }
}
