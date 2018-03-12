// ---------------------------------
// <copyright file="LanguageDirection.cs" company="SDL International">
// Copyright  2011 All Right Reserved
// </copyright>
// <author>Kostiantyn Lukianets</author>
// <email>klukianets@sdl.com</email>
// <date>2011-11-07</date>
// ---------------------------------
namespace Sdl.Community.Trados2007
{
    using System;
    using System.Collections.Generic;

    using Sdl.LanguagePlatform.Core;
    using Sdl.LanguagePlatform.TranslationMemory;

    using Trados.Interop.TMAccess;

    using Action = Sdl.LanguagePlatform.TranslationMemory.Action;
    using SearchResult = Sdl.LanguagePlatform.TranslationMemory.SearchResult;

    /// <summary>
    /// Encapsulates serverbased Language Direction-specifc logic for Trados 2007 transaltion Provider.
    /// </summary>
    public class ServerBasedTrados2007LanguageDirection : AbstractTrados2007LanguageDirection
    {
        #region Fields

        /// <summary>
        /// Typical locker
        /// </summary>
       // private readonly object locker = new object();

        #endregion // Fields

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerBasedTrados2007LanguageDirection"/> class.
        /// </summary>
        /// <param name="translationProvider">The translation provider.</param>
        public ServerBasedTrados2007LanguageDirection(ServerBasedTrados2007TranslationMemory translationProvider)
            : base(translationProvider)
        {
            this.TranslationProvider = translationProvider;
        }

        #endregion // Ctor

        #region Properties

        /// <summary>
        /// Gets the translation provider to which this language direction belongs.
        /// </summary>
        public new ServerBasedTrados2007TranslationMemory TranslationProvider { get; private set; }

        #endregion // Properties

    }
}