using System.Collections.Generic;

namespace SdlXliff.Toolkit.Integration.Data
{
    public class TagData
    {
        /// <summary>
        /// tag id
        /// </summary>
        public string TagID
        {
            get;
            private set;
        }

        /// <summary>
        /// position of tag in segment text
        /// </summary>
        public int TagPosition
        {
            get;
            private set;
        }

        /// <summary>
        /// tag content
        /// </summary>
        public string TagText
        {
            get;
            private set;
        }

        /// <summary>
        /// list of search matches (IndexData objects) - starting index, length of match
        /// </summary>
        public List<IndexData> SearchResults
        {
            get;
            set;
        }

        public TagData(string tagId, int tagPos, string tagText)
        {
            TagID = tagId;
            TagPosition = tagPos;
            TagText = tagText;

            SearchResults = new List<IndexData>();
        }

        /// <summary>
        /// make deep copy
        /// </summary>
        /// <returns></returns>
        public TagData Clone()
        {
            List<IndexData> indexes = new List<IndexData>();
            foreach (var ind in SearchResults)
            {
                indexes.Add(new IndexData(ind.IndexStart, ind.Length));
                indexes[indexes.Count - 1].RealStartIndex = ind.RealStartIndex;
            }

            TagData tag = new TagData(TagID, TagPosition, TagText);
            tag.SearchResults = indexes;

            return tag;
        }
    }
}
