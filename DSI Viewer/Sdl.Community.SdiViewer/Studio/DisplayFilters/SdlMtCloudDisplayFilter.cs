using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.DsiViewer.Studio.DisplayFilters
{
	public class SdlMtCloudDisplayFilter : IDisplayFilter
	{
	    public SdlMtCloudFilterSettings Settings { get; } = new SdlMtCloudFilterSettings();

		public bool EvaluateRow(DisplayFilterRowInfo rowInfo)
		{
			if (!rowInfo.IsSegment) return false;

			var shouldDisplayRow = true;
			var translationOrigin = rowInfo?.SegmentPair?.Properties?.TranslationOrigin;

			if (Settings.ByModel)
			{
				shouldDisplayRow = translationOrigin?.GetMetaData("model") == Settings.Model;
			}

			if (Settings.ByQualityEstimation)
			{
				var anyQualityEstimation = false;
				if (Settings.QeNoneAvailable)
				{
					anyQualityEstimation = GetQualityEstimation(translationOrigin) == DsiViewerInitializer.NoneAvailable;
				}
				if (Settings.QePoor)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == DsiViewerInitializer.PoorQuality;
				}
				if (Settings.QeGood)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == DsiViewerInitializer.GoodQuality;
				}
				if (Settings.QeAdequate)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == DsiViewerInitializer.AdequateQuality;
				}

				shouldDisplayRow &= anyQualityEstimation;
			}

			return shouldDisplayRow;
		}

		private static string GetQualityEstimation(ITranslationOrigin translationOrigin)
		{
			return translationOrigin?.GetMetaData("quality_estimation");
		}
	}
}