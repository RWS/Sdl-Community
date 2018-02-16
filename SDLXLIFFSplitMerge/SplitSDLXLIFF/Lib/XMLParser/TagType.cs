// <copyright file="TagType.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>TagType</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    /// <summary>
    /// Types of parser elements
    /// </summary>
    public enum TagType
    {
        /// <summary>
        /// Represents Text tag type
        /// </summary>
        Text,

        /// <summary>
        /// Represents Inline tag type
        /// </summary>
        InLineTag,

        /// <summary>
        /// Represents Structure tag type
        /// </summary>
        StructureTag,

        /// <summary>
        /// Represents pair tag Starting type
        /// </summary>
        TagPairStart,

        /// <summary>
        /// Represents pair tag Ending type
        /// </summary>
        TagPairEnd,

        /// <summary>
        /// Represents context info tag type
        /// </summary>
        ContextInfo
    }
}
