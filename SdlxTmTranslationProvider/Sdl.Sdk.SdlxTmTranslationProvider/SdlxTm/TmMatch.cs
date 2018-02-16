//-----------------------------------------------------------------------
// <copyright file="TmMatch.cs" company="SDL plc">
//  Copyright (c) SDL plc. All rights reserved.
// </copyright>
// <summary>
//  A match for a segment from the TM.
// </summary>
//-----------------------------------------------------------------------
namespace Sdl.Sdk.SdlxTmTranslationProvider.SdlxTm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// A match for a segment from the TM.
    /// </summary>
    public class TmMatch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TmMatch"/> class.
        /// </summary>
        public TmMatch()
        {
        }

        /// <summary>
        /// Gets or sets the unique id.
        /// </summary>
        /// <value>The unique id.</value>
        public int UniqueId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the translation id.
        /// </summary>
        /// <value>The translation id.</value>
        public int TranslationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the match score.
        /// </summary>
        /// <value>The match score.</value>
        public double MatchScore
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the penalties.
        /// </summary>
        /// <value>The penalties.</value>
        public TmPenalties Penalties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the source match.
        /// </summary>
        /// <value>The source match.</value>
        public TmTranslationVariant SourceMatch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the target match.
        /// </summary>
        /// <value>The target match.</value>
        public TmTranslationVariant TargetMatch
        {
            get;
            set;
        }
    }
}
