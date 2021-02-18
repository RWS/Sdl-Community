using Sdl.Community.DsiViewer.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.DsiViewer.Studio.DisplayFilters
{
	public class SdlMtCloudDisplayFilter : IDisplayFilter
	{
		private const string NoneAvailable = "N/A";
		private const string Poor = "Poor";
		private const string Good = "Good";
		private const string Adequate = "Adequate";

		public SdlMtCloudFilterSettings Settings { get; } = new();

		public bool EvaluateRow(IDisplayFilterRowInfo rowInfo)
		{
			var shouldDisplayRow = rowInfo.IsSegment;
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
					anyQualityEstimation = GetQualityEstimation(translationOrigin) == NoneAvailable;
				}
				if (Settings.QePoor)
				{
					anyQualityEstimation |=  GetQualityEstimation(translationOrigin) == Poor;
				}
				if (Settings.QeGood)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == Good;
				}
				if (Settings.QeAdequate)
				{
					anyQualityEstimation |= GetQualityEstimation(translationOrigin) == Adequate;
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