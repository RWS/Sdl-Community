// <copyright file="TagInfo.cs" company="SDL International">
// Copyright (c) 2010 All Right Reserved
// </copyright>
// <author>Oleksandr Tkachenko</author>
// <email>otkachenko@sdl.com</email>
// <date>2010-06-10</date>
// <summary>TagInfo</summary>

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
    /// <summary>
    /// Represents information about the tag
    /// </summary>    
    public class TagInfo
    {
        /// <summary>
        /// Initializes a new instance of the TagInfo class
        /// </summary>
        public TagInfo()
        {
            this.IndexStart = -1;
            this.IndexEnd = -1;
            this.TagID = Tags.None;
            this.Name = string.Empty;
            this.Text = string.Empty;
        }

        /// <summary>
        /// Gets or sets index of the start
        /// </summary>
        public int IndexStart { get; set; }

        /// <summary>
        /// Gets or sets index of the end
        /// </summary>
        public int IndexEnd { get; set; }
        
        /// <summary>
        /// Gets or sets the id of the tag
        /// </summary>
        public Tags TagID { get; set; }
        
        /// <summary>
        /// Gets or sets name of the tag
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets or sets tag's text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Copying tag info
        /// </summary>
        /// <returns>Copy of tag info</returns>
        public TagInfo Copy()
        {
            TagInfo copy = new TagInfo();
            copy.IndexStart = this.IndexStart;
            copy.IndexEnd = this.IndexEnd;
            copy.TagID = this.TagID;
            copy.Name = this.Name;
            copy.Text = this.Text;
            return copy;
        }
    }
}
