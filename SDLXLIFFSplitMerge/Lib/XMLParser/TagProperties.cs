// <copyright file="TagProperties.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>TagProperties</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents the properties of the tag
    /// </summary>
    internal class TagProperties
    {
        /// <summary>
        /// Initializes a new instance of the TagProperties class
        /// </summary>
        /// <param name="name">Name of the tag</param>
        /// <param name="tagType">Type of the tag</param>
        /// <param name="needTreating">Represents if tag needs treating</param>
        /// <param name="needStoring">Represents if tag needs storing</param>
        public TagProperties(string name, TagType tagType)
        {
            this.Name = name;
            this.TagType = tagType;
        }

        /// <summary>
        /// Gets or sets type of the tag
        /// </summary>
        public TagType TagType { get; set; }

        /// <summary>
        /// Gets or sets name of the tag
        /// </summary>
        public string Name { get; set; }
    }
}
