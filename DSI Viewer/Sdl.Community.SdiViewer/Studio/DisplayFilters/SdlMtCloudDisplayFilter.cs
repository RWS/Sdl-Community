using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Services;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.DisplayFilters;

namespace Sdl.Community.DsiViewer.Studio.DisplayFilters
{
	public class SdlMtCloudDisplayFilter : IDisplayFilter
	{
		private readonly TqeReader _tqeReader = new();

		public SdlMtCloudFilterSettings Settings { get; } = new();

		public bool EvaluateRow(IDisplayFilterRowInfo rowInfo)
		{
			if (!rowInfo.IsSegment) return false;

			var shouldDisplayRow = true;
			var translationOrigin = rowInfo.SegmentPair?.Properties?.TranslationOrigin;

			if (Settings.ByModel)
			{
				shouldDisplayRow = GetModel(translationOrigin) == Settings.Model;
			}

			if (Settings.ByQualityEstimation)
			{
				// Segments without an estimation (not from Language Weaver, or no QE returned) count as "None".
				var qualityEstimation = GetQualityEstimation(translationOrigin);
				if (string.IsNullOrWhiteSpace(qualityEstimation)) qualityEstimation = Constants.NoneQuality;

				var anyQualityEstimation =
					(Settings.QeNone && qualityEstimation == Constants.NoneQuality)
					|| (Settings.QePoor && qualityEstimation == Constants.PoorQuality)
					|| (Settings.QeGood && qualityEstimation == Constants.GoodQuality)
					|| (Settings.QeAdequate && qualityEstimation == Constants.AdequateQuality);

				shouldDisplayRow &= anyQualityEstimation;
			}

			return shouldDisplayRow;
		}

		// "quality_estimation" / "nmt_model" are only written by the Language Weaver Apply Metadata task or feedback panel;
		// segments translated in the Editor carry just the tqe-* keys, so fall back to the current TQE entry.
		private string GetQualityEstimation(ITranslationOrigin translationOrigin)
		{
			if (translationOrigin?.OriginSystem is null) return null;
			if (!translationOrigin.OriginSystem.ToLower().Contains(PluginResources.ProviderId)) return null;

			var qualityEstimation = translationOrigin.GetMetaData(Constants.QualityEstimation);
			return string.IsNullOrWhiteSpace(qualityEstimation)
				? _tqeReader.GetCurrentTqe(translationOrigin)?.QualityEstimation
				: qualityEstimation;
		}

		private string GetModel(ITranslationOrigin translationOrigin)
		{
			if (translationOrigin is null) return null;

			var model = translationOrigin.GetMetaData("nmt_model");
			return string.IsNullOrWhiteSpace(model)
				? _tqeReader.GetCurrentTqe(translationOrigin)?.Model
				: model;
		}
	}
}