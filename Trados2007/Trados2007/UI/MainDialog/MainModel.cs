// ---------------------------------
// <copyright file="MainModel.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-14</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    /// <summary>
    /// MVP Model
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainModel"/> class.
        /// </summary>
        /// <param name="languagePairs">The language pairs.</param>
        /// <param name="credentialStore">The credential store.</param>
        public MainModel(LanguagePair[] languagePairs, ITranslationProviderCredentialStore credentialStore)
        {

            this.LanguagePairs = languagePairs;
            this.CredentialStore = credentialStore;
        }

        /// <summary>
        /// Gets the credential store.
        /// </summary>
        public ITranslationProviderCredentialStore CredentialStore { get; private set; }

        /// <summary>
        /// Gets the language pairs.
        /// </summary>
        public IList<LanguagePair> LanguagePairs { get; private set; }

        /// <summary>
        /// Gets the selected Trados2007 translation provider.
        /// </summary>
        public ITrados2007TranslationProvider SelectedTrados2007TranslationProvider
        {
            get;
            private set;
        }

        /// <summary>
        /// Selects the file based provider.
        /// </summary>
        /// <param name="path">The local path.</param>
        /// <returns><c>true</c> if succeeded, otherwise - <c>false</c></returns>
        public bool SelectFileBasedProvider(string path)
        {
            try
            {
                var memory = new FileBasedTrados2007TranslationMemory(path);
                this.SelectedTrados2007TranslationProvider = memory;
                return true;
            }
            catch (FileNotFoundException ex)
            {
                MessagingHelpers.ShowError(ex);
                return false;
            }
            catch (ArgumentNullException ex)
            {
                MessagingHelpers.ShowError(ex, PluginResources.Exception_CouldNotOpen);
                return false;
            }
        }

        /// <summary>
        /// Selects the server-based trados 2007 translation provider.
        /// </summary>
        /// <param name="translationMemory">The translation memory.</param>
        /// <returns>
        ///   <c>true</c> if succeeded; otherwise - <c>false</c>
        /// </returns>
        public bool SelectServerBasedProvider(ServerBasedTrados2007TranslationMemory translationMemory)
        {
            try
            {
                var result = this.CheckServer(translationMemory);
                this.SelectedTrados2007TranslationProvider = translationMemory;
                return result;
            }
            catch (Exception ex)
            {
                MessagingHelpers.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Checks the server for inconsistencies. Theoretically, will throw exceptions if connection was unsuccessful.
        /// </summary>
        /// <param name="memory">The server-based translation memory.</param>
        /// <returns>
        ///   <c>true</c> if connection was succesfully established; otherwise - <c>false</c>
        /// </returns>
        private bool CheckServer(ServerBasedTrados2007TranslationMemory memory)
        {
            var langdir = memory.LanguageDirection;
            return memory.SupportsLanguageDirection(langdir);
        }
    }
}
