using Sdl.Community.AdvancedDisplayFilter.Helpers;
using Sdl.Community.AdvancedDisplayFilter.Models;
using Sdl.Community.AdvancedDisplayFilter.Services;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.AdvancedDisplayFilter.DisplayFilters
{
	public class ContentDisplayFilter: IDisplayFilter
	{
		private readonly ContentFilterSettings _filterSettings;
		private readonly SegmentTextVisitor _segmentVisitor;
		public ContentDisplayFilter(ContentFilterSettings filterSettings)
		{
			_filterSettings = filterSettings;
			_segmentVisitor = new SegmentTextVisitor();
			
		}
		public bool EvaluateRow(DisplayFilterRowInfo rowInfo)
		{
			if (!rowInfo.IsSegment)
			{
				return false;
			}

			if (_filterSettings.SearchInSource)
			{
				var sourceText = _segmentVisitor.GetText(rowInfo.SegmentPair.Source);
				if (sourceText.Contains(_filterSettings.SelectedText))
				{
					return true;
				}
			}
			else
			{
				var targetText = _segmentVisitor.GetText(rowInfo.SegmentPair.Target);
				if (targetText.Contains(_filterSettings.SelectedText))
				{
					return true;
				}
			}
			return false;
		}
	}
}
