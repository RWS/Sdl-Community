//-----------------------------------------------------------------------
// <copyright file="SdlxTmTranslationProvider.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Helper class for SDLX translation memory
    /// </summary>
    public static class SdlxTmTranslationProvider
    {
        /// <summary>
        /// The URI scheme for SDLX translation memory
        /// </summary>
        internal static readonly string SdlxTmUriScheme = "sdlxtm";

        /// <summary>
        /// The name of the SDLX translation memory provider plugin
        /// </summary>
        internal static readonly string SdlxTmName = "SDLX Translation Memory";

        /// <summary>
        /// The version number of the plugin
        /// </summary>
        internal static readonly string SdlxTmVersion = "1.0";

        /// <summary>
        /// The single SDLX TM controller class for this assembly
        /// </summary>
        private static Sdl.Sdlx.S42TMOBJ translationMemoryController = new Sdl.Sdlx.S42TMOBJClass();

        /// <summary>
        /// Gets the single SDLX TM controller class for this assembly
        /// </summary>
        /// <value>The TM controller.</value>
        public static Sdl.Sdlx.S42TMOBJ TmController
        {
            get { return translationMemoryController; }
        }
    }
}
