// ---------------------------------
// <copyright file="TranslationProviderFactory.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-10-14</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    using System;

    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [TranslationProviderFactory(Name = TranslationProviderFactory.ProviderFactoryName,
        Id = TranslationProviderFactory.ProviderFactoryId,
        Description = TranslationProviderFactory.Description)]
    public class TranslationProviderFactory : ITranslationProviderFactory
    {
        #region Consts

        /// <summary>
        /// WorldServerTranslationProviderFactory string.
        /// </summary>
        internal const string ProviderFactoryName = "Trados2007TranslationProviderFactory";

        /// <summary>
        /// WorldServerTranslationProviderFactory string.
        /// </summary>
        internal const string ProviderFactoryId = "Trados2007TranslationProviderFactory";

        /// <summary>
        /// WorldServer TranslationProvider plug-in string.
        /// </summary>
        internal const string Description = "Trados 2007 TranslationProvider plug-in";

        #endregion Consts

        /// <summary>
        /// Returns true if this factory supports the specified URI.
        /// </summary>
        /// <param name="translationProviderUri">The Uri.</param>
        /// <returns>
        /// True if this factory supports the specified URI.
        /// </returns>
        public bool SupportsTranslationProviderUri(Uri translationProviderUri)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("translationProviderUri");
            }

            return string.Equals(
                AbstractTrados2007TranslationProvider.UriScheme,
                translationProviderUri.Scheme,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates an instance of the translation provider defined by the specified URI.
        /// </summary>
        /// <param name="translationProviderUri">A URI that identifies the translation provider to create.</param><param name="translationProviderState">Serialized state information that should be used for 
        ///             configuring the translation provider. This is typically state information that was previously saved 
        ///             by calling <see cref="M:Sdl.LanguagePlatform.TranslationMemoryApi.ITranslationProvider.SerializeState"/>.</param><param name="credentialStore">A credential store object that can be used to retrieve credentials 
        ///             required for this translation provider. </param>
        /// <returns>
        /// A new translation provider object, ready to be used.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown if <paramref name="translationProviderUri"/>, <paramref name="translationProviderState"/> or <paramref name="credentialStore"/> 
        ///             is null.</exception><exception cref="T:System.ArgumentException">Thrown if <paramref name="translationProviderUri"/> is not supported by this factory.</exception><exception cref="T:System.ArgumentException">Thrown if <paramref name="translationProviderState"/> is invalid.</exception><exception cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderAuthenticationException">Thrown when no appropriate credentials are available in the credential store.</exception>
        public ITranslationProvider CreateTranslationProvider(
            Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
        {
            if (translationProviderUri == null)
            {
                throw new ArgumentNullException("translationProviderUri");
            }

            if (credentialStore == null)
            {
                throw new ArgumentNullException("credentialStore");
            }

            if (!this.SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new ArgumentException(
                    string.Format(PluginResources.Exception_UriNotSupported, translationProviderUri));
            }

            var settings = new TranslationProviderSettings(translationProviderUri);

            return LoadState(translationProviderState, settings, credentialStore);
        }

        private ITranslationProvider LoadState(string translationProviderState, TranslationProviderSettings settings, ITranslationProviderCredentialStore credentialStore)
        {
            var serializer = new XmlSerializer(typeof(TP2007ProviderSerializationHelper));

            var reader = new System.IO.StringReader(translationProviderState);

            var result = (TP2007ProviderSerializationHelper)serializer.Deserialize(reader);

            if (result.Type == typeof(FileBasedTrados2007TranslationMemory).ToString())
            {
                return new FileBasedTrados2007TranslationMemory(settings.Path);
            }

            if (result.Type == typeof(ServerBasedTrados2007TranslationMemory).ToString())
            {
                if (Prerequisites.WarnIfServerManager2007NotInstalled())
                {
                    throw new Exception(Prerequisites.WarnMessage);
                }
                    
                string serverAdress = settings.Path;
                string tmName = settings.TranslationMemoryName;
                string container = settings.Container;

                TranslationMemoryAccessMode acessMode = settings.AccessMode;

                return new ServerBasedTrados2007TranslationMemory(serverAdress, tmName, container, settings.UserName, settings.Password, acessMode);
            }

            return null;
        }

        /// <summary>
        /// Gets general information for the specified translation provider.
        /// </summary>
        /// <param name="translationProviderUri">A translation provider URI, representing the translation provider.</param><param name="translationProviderState">Optional translation provider state information, which can be used to determine 
        ///             certain aspects of the general information.</param>
        /// <returns>
        /// A <see cref="T:Sdl.LanguagePlatform.TranslationMemoryApi.TranslationProviderInfo"/> object, containing general information that allows
        ///             an application to query the translation provider without having to instantiate it.
        /// </returns>
        public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
        {
            if (!this.SupportsTranslationProviderUri(translationProviderUri))
            {
                throw new NotSupportedException(
                    string.Format(PluginResources.Exception_UriNotSupported, translationProviderUri));
            }

            var settings = new TranslationProviderSettings(translationProviderUri);
            TranslationProviderInfo translationInfo = new TranslationProviderInfo()
            {
                Name = settings.TranslationMemoryName,
                TranslationMethod = TranslationMethod.TranslationMemory
            };

            return translationInfo;
        }
    }
}