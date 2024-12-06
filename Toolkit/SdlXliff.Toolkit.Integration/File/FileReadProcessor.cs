using System.Collections.Generic;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SdlXliff.Toolkit.Integration.Data;
using SdlXliff.Toolkit.Integration.Helpers;

namespace SdlXliff.Toolkit.Integration.File
{
	internal class FileReadProcessor : AbstractBilingualContentProcessor
	{
		protected SearchSettings _searchSettings;
		private DataExtractor _dataExtractor;
		private DataSearcher _searcher;

		public FileReadProcessor(string filePath, SearchSettings settings)
		{
			_searchSettings = settings;
			_dataExtractor = new DataExtractor();
			_searcher = new DataSearcher(_searchSettings);

			ResultInSource = new List<SegmentData>();
			ResultInTarget = new List<SegmentData>();
		}

		/// <summary>
		/// list of SegmentData objects - search matches data in source in one file
		/// </summary>
		public List<SegmentData> ResultInSource { get; }

		/// <summary>
		/// list of SegmentData objects - search matches data in target in one file
		/// </summary>
		public List<SegmentData> ResultInTarget { get; }

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var sourceResult = new List<IndexData>();
			var targetResult = new List<IndexData>();

			if (!paragraphUnit.IsStructure)
				foreach (var item in paragraphUnit.SegmentPairs)
				{
					var itemStatus = item.Properties.ConfirmationLevel;

					// extract text and tags from Segment
					var sourceSegment = item.Source;
					_dataExtractor.Process(sourceSegment);
					var sourceTags = _dataExtractor.Tags;
					//we need to add a space before the soft return so the soft return is highlightable when searching for it
					var sourceText = _dataExtractor.PlainText.ToString().Replace("\n", " \n");
					var sourceLContent = _dataExtractor.LockedContent;

					var targetSegment = item.Target;
					_dataExtractor.Process(targetSegment);
					//we need to add a space before the soft return so the soft return is highlightable when searching for it
					var targetText = _dataExtractor.PlainText.ToString().Replace("\n", " \n");
					var targetTags = _dataExtractor.Tags;
					var targetLContent = _dataExtractor.LockedContent;

					// perform search
					if (_searcher.ShouldBeProcessed(item.Properties.IsLocked, item.Properties.ConfirmationLevel))
					{
						if (_searchSettings.SearchInSource && (sourceText.Length > 0 || sourceTags.Count > 0))
						{
							_searcher.SearchInSegment(sourceText, sourceTags, sourceLContent);
							sourceResult = _searcher.ResultsInText;
							sourceTags = _searcher.ResultsInTags;
						}

						if (_searchSettings.SearchInTarget && (targetText.Length > 0 || targetTags.Count > 0))
						{
							_searcher.SearchInSegment(targetText, targetTags, targetLContent);
							targetResult = _searcher.ResultsInText;
							targetTags = _searcher.ResultsInTags;
						}

						// collect results
						if (SegmentHelper.ContainMatches(sourceResult, sourceTags) || SegmentHelper.ContainMatches(targetResult, targetTags))
						{
							CollectResults(item.Properties.Id.Id, sourceText, itemStatus, sourceSegment, sourceResult, sourceTags, true);
							CollectResults(item.Properties.Id.Id, targetText, itemStatus, targetSegment, targetResult, targetTags, false);
						}
					}
				}
		}

		#region private

		private void CollectResults(string segmentId, string segmentText, ConfirmationLevel segmentStatus,
			ISegment segmentContent, List<IndexData> matches, List<TagData> tags, bool isSource)
		{
			int sID;
			if (int.TryParse(segmentId, out sID))
				if (isSource)
				{
					ResultInSource.Add(new SegmentData(ResultInSource.Count, sID, segmentText, segmentStatus, segmentContent));
					ResultInSource[ResultInSource.Count - 1].SearchResults = matches;
					ResultInSource[ResultInSource.Count - 1].Tags = tags;
				}
				else
				{
					ResultInTarget.Add(new SegmentData(ResultInTarget.Count, sID, segmentText, segmentStatus, segmentContent));
					ResultInTarget[ResultInTarget.Count - 1].SearchResults = matches;
					ResultInTarget[ResultInTarget.Count - 1].Tags = tags;
				}
		}

		#endregion private
	}
}