using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Core.Globalization;

namespace SdlXliff.Toolkit.Integration.Data
{
    class DataSearcher
    {
        private SearchSettings _settings;
        private List<IndexData> _lockedContent;

        /// <summary>
        /// search results in text (no tags)
        /// </summary>
        public List<IndexData> resultsInText
        {
            get;
            private set;
        }

        /// <summary>
        /// tags data + search results in tags only
        /// </summary>
        public List<TagData> resultsInTags
        {
            get;
            private set;
        }

        public DataSearcher(SearchSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// checks if segment with settings in parameters should be processed
        /// </summary>
        /// <param name="isLocked">true - locked</param>
        /// <param name="status">segment status</param>
        /// <returns>true - process</returns>
        public bool checkSegment(bool isLocked, ConfirmationLevel status)
        {
            // check locked setting
            if (isLocked && !_settings.SearchInLocked)
                return false;

            // check confirmation status setting
            if (_settings.NotSearchStatus.Contains(status))
                return false;

            return true;
        }

        public void SearchInSegment(string text, List<IndexData> lockedContent)
        {
            resultsInTags = null;
            resultsInText = new List<IndexData>();
            _lockedContent = lockedContent;

            // modify search string according to settings
            string textToSearch = GetSearchString(_settings.SearchText);

            SearchText(text, textToSearch);
        }

        public void SearchInSegment(string text, List<TagData> tags, List<IndexData> lockedContent)
        {
            // modify search string according to settings
            string textToSearch = GetSearchString(_settings.SearchText);

            // find matches & save to list
            if (_settings.SearchInTag)
            {
                resultsInText = null;
                resultsInTags = tags;

                SearchTag(textToSearch);
            }
            else
            {
                resultsInText = new List<IndexData>();
                resultsInTags = null;
                _lockedContent = lockedContent;

                SearchText(text, textToSearch);
            }
        }

        #region private methods
        /// <summary>
        /// search for matches in text
        /// </summary>
        /// <param name="text">text to search in</param>
        /// <returns>list of indexes of search string matches</returns>
        private void SearchText(string text, string textToSearch)
        {
            // get the matches of search string
            Match searchResults = Regex.Match(text, textToSearch, GetRegexOptions());
            while (searchResults.Success)
            {
                foreach (Group searchGroup in searchResults.Groups)
                    if (_settings.SearchInLocked || !isLockedContent(searchGroup.Index, searchGroup.Length))
                        if (searchGroup.Length > 0)
                            resultsInText.Add(new IndexData(searchGroup.Index,
                            searchGroup.Length));

                searchResults = searchResults.NextMatch();
            }

            // check for index overlap, update property if overlapped
            ValidateIndexes();
        }

        private void SearchTag(string textToSearch)
        {
            foreach (TagData data in resultsInTags)
            {
                data.SearchResults = new List<IndexData>();

                // get the matches of search string
                Match searchResults = Regex.Match(data.TagText, textToSearch, GetRegexOptions());
                while (searchResults.Success)
                {
                    foreach (Group searchGroup in searchResults.Groups)
                        if (searchGroup.Length > 0)
                            data.SearchResults.Add(new IndexData(searchGroup.Index,
                            searchGroup.Length));

                    searchResults = searchResults.NextMatch();
                }
            }
        }

        private void ValidateIndexes()
        {
            List<IndexData> index = resultsInText;

            int indexSt = 0;
            int indexE = 0;
            bool isOverlapFound;
            for (int i = 0; i < index.Count; i++)
                if (!index[i].IsIndexOverlap)
                {
                    indexSt = index[i].IndexStart;
                    indexE = index[i].IndexStart + index[i].Length;
                    isOverlapFound = false;
                    foreach (var x in index.Where(x => !x.IsIndexOverlap
                          && !(indexSt == x.IndexStart && indexE == (x.IndexStart + x.Length))
                          // && ((indexSt >= x.IndexStart && indexSt <= (x.IndexStart + x.Length))
                          // || (indexE >= x.IndexStart && indexE <= (x.IndexStart + x.Length))))) 2.8.2011 fix
                          && ((indexSt >= x.IndexStart && indexSt < (x.IndexStart + x.Length))
                          || (indexE > x.IndexStart && indexE <= (x.IndexStart + x.Length)))))
                    {
                        x.IsIndexOverlap = true;
                        isOverlapFound = true;
                    }

                    if (isOverlapFound)
                        index[i].IsIndexOverlap = true;
                }
        }

        private RegexOptions GetRegexOptions()
        {
            RegexOptions options = new RegexOptions();

            if (!_settings.MatchCase)
                options |= RegexOptions.IgnoreCase;

            return options;
        }

        private string GetSearchString(string text)
        {
            // check regex setting
            if (!_settings.UseRegex)
            {
                text = Regex.Escape(text);

                // check match whole word setting
                if (_settings.MatchWholeWord)
                    text = string.Format("{0}{1}{0}", @"\b", text);
            }

            return text;
        }

        private bool isLockedContent(int startIndex, int length)
        {
            int lastIndex = startIndex + length;
            if (_lockedContent != null && _lockedContent.Count > 0)
                foreach (IndexData lockData in _lockedContent)
                    if ((startIndex > lockData.IndexStart && startIndex < lockData.IndexStart + lockData.Length) ||
                    (lastIndex > lockData.IndexStart && lastIndex < lockData.IndexStart + lockData.Length))
                        return true;

            return false;
        }
        #endregion
    }
}
