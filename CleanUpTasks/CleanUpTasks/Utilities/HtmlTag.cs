using System.Collections.Generic;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public class HtmlTag
	{
		/// <summary>
		/// Name of this tag
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Collection of attribute names and values for this tag
		/// </summary>
		public Dictionary<string, string> Attributes { get; set; }

		/// <summary>
		/// True if this tag contained a trailing forward slash
		/// </summary>
		public bool TrailingSlash { get; set; }
        
		/// <summary>
		/// True if there is a space before the trailing slash
		/// </summary>
		public bool SpaceBeforeTrailingSlash { get; set; }

		/// <summary>
		/// True if this tag has an end tag
		/// </summary>
		public bool HasEndTag { get; set; }

		/// <summary>
		/// Is an end tag with missing start tag
		/// </summary>
		public bool IsEndGhostTag { get; set; }

		/// <summary>
		/// True if attributes have single quotes
		/// </summary>
		public bool AttributesHasSingleQuotes { get; set; }

		/// <summary>
		/// True if attributes have double quotes
		/// </summary>
		public bool AttributesHasDoubleQuotes { get; set; }
	};
}