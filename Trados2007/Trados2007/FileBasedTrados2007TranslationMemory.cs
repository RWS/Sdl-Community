// ---------------------------------
// <copyright file="FileBasedTrados2007TranslationProvider.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.IO;
    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemoryApi;
    using Trados.Interop.TMAccess;

    /// <summary>
    /// Filebased Trados 2007 TM
    /// </summary>
    public class FileBasedTrados2007TranslationMemory : AbstractTrados2007TranslationProvider
    {
        #region Fields

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedTrados2007TranslationMemory"/> class. 
        /// Opens file based Trados 2007 Translation Memory in Read-only mode.
        /// </summary>
        /// <param name="path">The physical path to tmw file.</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown if any part of Trados 2007 translation memory was not found.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the passed parameters is null or empty.
        /// </exception>
        public FileBasedTrados2007TranslationMemory(string path)
            : this(path, "user", TranslationMemoryAccessMode.Read)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedTrados2007TranslationMemory"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="accessMode">The access mode.</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown if any part of Trados 2007 translation memory was not found.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the passed parameters is null or empty.
        /// </exception>
        public FileBasedTrados2007TranslationMemory(string path, TranslationMemoryAccessMode accessMode)
            : this(path, "user", accessMode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedTrados2007TranslationMemory"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="accessMode">The access mode.</param>
        /// <exception cref="FileNotFoundException">
        /// Thrown if any part of Trados 2007 translation memory was not found.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Thrown if any of the passed parameters is null or empty.
        /// </exception>
        public FileBasedTrados2007TranslationMemory(string path, string userName, TranslationMemoryAccessMode accessMode) :
            base(new TranslationProviderSettings()
                {
                    UserName = userName,
                    Path = path,
                    TranslationMemoryName = Path.GetFileNameWithoutExtension(path),
                    Host = "localhost",
                    AccessMode = accessMode
                })
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            
            // trados 2007 filebased TM consists of 5 files. Check all present in one dir
            this.CheckProviderConsistency(path);
        }

        #endregion // Constructor

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

        /// <summary>
        /// Gets the user-friendly name of this provider. It is not necessarily unique across the system.
        /// </summary>
        public override string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.Settings.Path);
            }
        }

        /// <summary>
        /// Gets the trados provider.
        /// </summary>
        public override TranslationMemoryClass TradosProvider
        {
            get
            {
                return providersPull.GetProvider(this.Settings.Path, this.Settings.UserName, this.Settings.AccessMode.ToUnmanaged());
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
            return new FileBasedTrados2007LanguageDirection(this);
        }

        #endregion // Methods

        #region Methods private

        /// <summary>
        /// Checks the provider consistency - whether directory of tmw file contains other parts of Trados 2007 TM.
        /// </summary>
        /// <param name="tmwPath">The tmw file path.</param>
        private void CheckProviderConsistency(string tmwPath)
        {
            string dir = Path.GetDirectoryName(tmwPath);
            string fileName = Path.GetFileNameWithoutExtension(tmwPath);

            string iixPath = string.Format("{0}{1}{2}.iix", dir, Path.DirectorySeparatorChar, fileName);
            if (!File.Exists(iixPath))
            {
                //FileNotFoundException caused infinite loop so make it less special
                throw new Exception(string.Format(PluginResources.Exception_TMPartMissing, iixPath));
            }

            string mdfPath = string.Format("{0}{1}{2}.mdf", dir, Path.DirectorySeparatorChar, fileName);
            if (!File.Exists(mdfPath))
            {
                throw new Exception(string.Format(PluginResources.Exception_TMPartMissing, mdfPath));
            }

            string mtfPath = string.Format("{0}{1}{2}.mtf", dir, Path.DirectorySeparatorChar, fileName);
            if (!File.Exists(mtfPath))
            {
                throw new Exception(string.Format(PluginResources.Exception_TMPartMissing, mtfPath));
            }

            string mwfPath = string.Format("{0}{1}{2}.mwf", dir, Path.DirectorySeparatorChar, fileName);
            if (!File.Exists(iixPath))
            {
                throw new Exception(string.Format(PluginResources.Exception_TMPartMissing, mwfPath));
            }
        }

        #endregion // Methods private
    }
}