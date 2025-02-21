using System.Collections.Generic;

namespace SDLCommunityCleanUpTasks.Utilities
{
	public class HtmlTagTable
    {
        public readonly Dictionary<string, HtmlTag> _tagTable = new Dictionary<string, HtmlTag>();

        public HtmlTagTable(string html)
        {
            BuildTagTable(html);
        }

        public Dictionary<string, HtmlTag> Table => _tagTable;

        private void BuildTagTable(string html)
        {
            HtmlTag tag;
            var parse = new HtmlParser(html);
            while (parse.ParseNext("*", out tag))
            {
                if (!_tagTable.ContainsKey(tag.Name))
                {
                    if (tag.HasEndTag)
                    {
                        // If this is an end tag, it means there was no corresponding start tag found
                        tag.IsEndGhostTag = true;
                    }

                    _tagTable.Add(tag.Name, tag);
                }
                else
                {
                    _tagTable[tag.Name].HasEndTag = tag.HasEndTag;
                }
            }
        }
    }
}