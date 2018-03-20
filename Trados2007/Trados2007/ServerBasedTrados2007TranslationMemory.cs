// ---------------------------------
// <copyright file="ServerBasedTrados2007TranslationMemory.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-10</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;

    using Trados.Interop.TMAccess;

    /// <summary>
    /// Serverbased Trados 2007 TM
    /// </summary>
    public class ServerBasedTrados2007TranslationMemory : AbstractTrados2007TranslationProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBasedTrados2007TranslationMemory"/> class.
        /// </summary>
        /// <param name="serverAdress">The server adress.</param>
        /// <param name="translationMemoryName">Name of the translation memory.</param>
        /// <param name="container">The name of the translation provider container.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="acessMode">The acess mode.</param>
        /// <exception cref="TypeInitializationException">Universally thrown in case any connection problems.</exception>
        public ServerBasedTrados2007TranslationMemory(
            string serverAdress,
            string translationMemoryName,
            string container,
            string userName,
            string password,
            TranslationMemoryAccessMode acessMode)
            : base(new TranslationProviderSettings()
                {
                    Path = serverAdress,
                    TranslationMemoryName = translationMemoryName,
                    Container = container,
                    UserName = userName,
                    Password = password,
                    AccessMode = acessMode,
                    Host = "localhost"
                })
        {
            this.Container = container;
        }


        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the supported language direction.
        /// </summary>
        public override LanguagePair LanguageDirection
        {
            get
            {
                string src = this.TradosProvider.Setup.SourceLocale;
                string trg = this.TradosProvider.Setup.TargetLocales;

                var source = LanguageMapper.ToCultureInfo(src);
                var target = LanguageMapper.ToCultureInfo(trg);

                return new LanguagePair(source, target);
            }
        }

        public override TranslationMemoryClass TradosProvider
        {
            get
            {
                string formattedPath = string.Format(
                        "tm://{0}:{1}@{2}/{3}",
                        this.Settings.UserName,
                        this.Settings.Password,
                        this.Settings.Path,
                        this.Settings.TranslationMemoryName);

                return providersPull.GetProvider(
                            formattedPath,
                            this.Settings.UserName,
                            this.Settings.AccessMode.ToUnmanaged(),
                            this.Settings.Password);
            }
        }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Gets a translation provider for the specified language direction.
        /// </summary>
        /// <param name="languageDirection">The language direction.</param>
        /// <returns>
        /// The language direction matching the given source and target language.
        /// </returns>
        public override ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
        {
            return new ServerBasedTrados2007LanguageDirection(this);
        }

        #endregion // Methods

        #region Methods private

        #endregion // Methods private

        public string Container { get; private set; }
    }
}