using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Core.Globalization;

namespace SdlXliff.Toolkit.Integration.Data
{
    class DataSearcher
    {
        private readonly SearchSettings _settings;
        private List<IndexData> _lockedContent;

        /// <summary>
        /// search results in text (no tags)
        /// </summary>
        public List<IndexData> ResultsInText
        {
            get;
            private set;
        }

        /// <summary>
        /// tags data + search results in tags only
        /// </summary>
        public List<TagData> ResultsInTags
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
        public bool ShouldBeProcessed(bool isLocked, ConfirmationLevel status)
        {
	        if (_settings != null)
	        {
		        // check locked setting
		        if (isLocked && !_settings.SearchInLocked)
		        {
			        return false;
		        }

		        // check confirmation status setting
		        if (_settings.NotSearchStatus.Contains(status))
		        {
			        return false;
		        }
	        }

	        return true;
        }

        public void SearchInSegment(string text, List<IndexData> lockedContent)
        {
            ResultsInTags = null;
            ResultsInText = new List<IndexData>();
            _lockedContent = lockedContent;

            // modify search string according to settings
            var textToSearch = GetSearchString(_settings.SearchText);

            SearchText(text, textToSearch);
        }

        public void SearchInSegment(string text, List<TagData> tags, List<IndexData> lockedContent)
        {
            // modify search string according to settings
            var textToSearch = GetSearchString(_settings.SearchText);

            // find matches & save to list
            if (_settings.SearchInTag)
            {
                ResultsInText = null;
                ResultsInTags = tags;

                SearchTag(textToSearch);
            }
            else
            {
                ResultsInText = new List<IndexData>();
                ResultsInTags = null;
                _lockedContent = lockedContent;

                SearchText(text, textToSearch);
            }
        }

		#region private methods

		/// <summary>
		/// search for matches in text
		/// </summary>
		/// <param name="text"></param>
		/// <param name="textToSearch">text to search in</param>
		/// <returns>list of indexes of search string matches</returns>
		private void SearchText(string text, string textToSearch)
		{
			try
			{
				// get the matches of search string
				var searchResults = Regex.Match(text, textToSearch, GetRegexOptions());

				while (searchResults.Success)
				{
					foreach (Group searchGroup in searchResults.Groups)
						if (_settings.SearchInLocked || !IsLockedContent(searchGroup.Index, searchGroup.Length) &&
							searchGroup.Length > 0)
						{
							ResultsInText.Add(new IndexData(searchGroup.Index, searchGroup.Length));
						}

					searchResults = searchResults.NextMatch();
				}

				// check for index overlap, update property if overlapped
				ValidateIndexes();
			}
			catch
			{
				// catch all; ignore exceptions thrown when Regex is not correct
				// (The regex format is also validated on the UI, using the Validating event)
			}
		}

		private void SearchTag(string textToSearch)
		{
			try
			{
				foreach (var data in ResultsInTags)
				{
					data.SearchResults = new List<IndexData>();

					// get the matches of search string
					var searchResults = Regex.Match(data.TagText, textToSearch, GetRegexOptions());
					while (searchResults.Success)
					{
						foreach (Group searchGroup in searchResults.Groups)
							if (searchGroup.Length > 0)
							{
								data.SearchResults.Add(new IndexData(searchGroup.Index, searchGroup.Length));
							}

						searchResults = searchResults.NextMatch();
					}
				}
			}
			catch
			{
				// catch all; ignore exceptions thrown when Regex is not correct
				// (The regex format is also validated on the UI, using the Validating event)		
			}
		}

		private void ValidateIndexes()
        {
            var index = ResultsInText;
            int indexSt;
            int indexE;
            bool isOverlapFound;

            for (int i = 0; i < index.Count; i++)
                if (!index[i].IsIndexOverlap)
                {
                    indexSt = index[i].IndexStart;
                    indexE = index[i].IndexStart + index[i].Length;
                    isOverlapFound = false;
                    foreach (var x in index.Where(x => !x.IsIndexOverlap 
                                   && !(indexSt == x.IndexStart && indexE == x.IndexStart + x.Length)
                                   && (indexSt >= x.IndexStart && indexSt < x.IndexStart + x.Length || indexE > x.IndexStart && indexE <= x.IndexStart + x.Length)))
                    {
                        x.IsIndexOverlap = true;
                        isOverlapFound = true;
                    }

                    if (isOverlapFound)
                    {
	                    index[i].IsIndexOverlap = true;
                    }
                }
        }

        private RegexOptions GetRegexOptions()
        {
            var options = new RegexOptions();

            if (!_settings.MatchCase)
            {
	            options |= RegexOptions.IgnoreCase;
            }

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

        private bool IsLockedContent(int startIndex, int length)
        {
            var lastIndex = startIndex + length;
            if (_lockedContent != null && _lockedContent.Count > 0)
                foreach (var lockData in _lockedContent)
                    if (startIndex > lockData.IndexStart && startIndex < lockData.IndexStart + lockData.Length ||
                    lastIndex > lockData.IndexStart && lastIndex < lockData.IndexStart + lockData.Length)
                        return true;

            return false;
        }
        #endregion
    }
}
