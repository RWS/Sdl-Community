using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.DsiViewer.Studio.DisplayFilters
{
	public class SdlMtCloudDisplayFilter : IDisplayFilter
	{
		public SdlMtCloudFilterSettings Settings { get; } = new();

		public bool EvaluateRow(IDisplayFilterRowInfo rowInfo)
		{
			if (!rowInfo.IsSegment) return false;

			var shouldDisplayRow = true;
			var translationOrigin = rowInfo.SegmentPair?.Properties?.TranslationOrigin;

			if (Settings.ByModel)
			{
				shouldDisplayRow = translationOrigin?.GetMetaData("model") == Settings.Model;
			}

			if (Settings.ByQualityEstimation)
			{
				var anyQualityEstimation = false;
				if (Settings.QeNone)
				{
					anyQualityEstimation = GetQualityEstimation(translationOrigin) == Constants.NoneQuality;
				}
				if (Settings.QePoor)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == Constants.PoorQuality;
				}
				if (Settings.QeGood)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == Constants.GoodQuality;
				}
				if (Settings.QeAdequate)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == Constants.AdequateQuality;
				}

				shouldDisplayRow &= anyQualityEstimation;
			}

			return shouldDisplayRow;
		}

		private string GetQualityEstimation(ITranslationOrigin translationOrigin)
		{
			if (translationOrigin?.OriginSystem is null) return null;
			return translationOrigin.OriginSystem.ToLower()
						   .Contains(PluginResources.ProviderId)
						   ? translationOrigin.GetMetaData("quality_estimation")
						   : null;
		}
	}
}