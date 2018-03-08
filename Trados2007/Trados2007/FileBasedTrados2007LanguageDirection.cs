// ---------------------------------
// <copyright file="LanguageDirection.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.TranslationStudio.Plugins.Trados2007
{
    using System;
    using System.Collections.Generic;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemory;

    using Trados.Interop.TMAccess;

    using Action = Sdl.LanguagePlatform.TranslationMemory.Action;
    using SearchResult = Sdl.LanguagePlatform.TranslationMemory.SearchResult;

    /// <summary>
    /// Encapsulates Filebased Language Direction-specifc logic for Trados 2007 transaltion Provider.
    /// </summary>
    public sealed class FileBasedTrados2007LanguageDirection : AbstractTrados2007LanguageDirection
    {
        /// <summary>
        /// Typical locker
        /// </summary>
        //private readonly object locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBasedTrados2007LanguageDirection"/> class.
        /// </summary>
        /// <param name="translationProvider">The Trados 2007 translation provider.</param>
        public FileBasedTrados2007LanguageDirection(FileBasedTrados2007TranslationMemory translationProvider)
            : base(translationProvider)
        {
            this.TranslationProvider = translationProvider;
        }

        #region Properties

        /// <summary>
        /// Gets the translation provider to which this language direction belongs.
        /// </summary>
        public new FileBasedTrados2007TranslationMemory TranslationProvider { get; private set; }

        #endregion // Properties

    }
}