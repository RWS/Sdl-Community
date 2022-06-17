using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.StudioViews.Providers
{
	public class DisplayFilter : IDisplayFilter
	{
		public bool EvaluateRow(IDisplayFilterRowInfo rowInfo)
		{
			var success = rowInfo.IsSegment;
			return success;
		}
	}
}
