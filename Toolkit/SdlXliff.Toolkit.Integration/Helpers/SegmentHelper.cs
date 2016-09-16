using System.Collections.Generic;
using SdlXliff.Toolkit.Integration.Data;

namespace SdlXliff.Toolkit.Integration.Helpers
{
    static class SegmentHelper
    {
        public static int GetTagMatchesCount(List<TagData> data)
        {
            int count = 0;
            foreach (TagData dItem in data)
                if (dItem.SearchResults != null && dItem.SearchResults.Count > 0)
                    count += dItem.SearchResults.Count;

            return count;
        }

        public static bool ContainMatches(List<IndexData> textMatches)
        {
            if (textMatches != null && textMatches.Count > 0)
                return true;
            return false;
        }

        public static bool ContainMatches(List<IndexData> textMatches, List<TagData> tagMatches)
        {
            if (ContainMatches(textMatches))
                return true;
            if (tagMatches != null && GetTagMatchesCount(tagMatches) > 0)
                return true;
            return false;
        }
    }
}
