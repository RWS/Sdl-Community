//-----------------------------------------------------------------------
// <copyright file="SdlxTmLanguagePair.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;

    /// <summary>
    /// A class to represent a language pair from an SDLX translation memory
    /// </summary>
    public class SdlxTmLanguagePair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SdlxTmLanguagePair"/> class.
        /// </summary>
        /// <param name="sourceLanguage">The source language.</param>
        /// <param name="targetLanguage">The target language.</param>
        public SdlxTmLanguagePair(string sourceLanguage, string targetLanguage)
        {
            this.SourceLanguage = sourceLanguage;
            this.TargetLanguage = targetLanguage;
        }

        /// <summary>
        /// Gets or sets the source language.
        /// </summary>
        /// <value>The source language.</value>
        public string SourceLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target language.
        /// </summary>
        /// <value>The target language.</value>
        public string TargetLanguage
        {
            get;
            set;
        }
    }
}