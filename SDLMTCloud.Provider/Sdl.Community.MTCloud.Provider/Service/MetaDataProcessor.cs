using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	internal class MetaDataProcessor : AbstractBilingualContentProcessor
	{
		public MetaDataProcessor(List<MetadataTransferObject> translationData)
		{
			TranslationData = translationData;
		}

		public List<MetadataTransferObject> TranslationData { get; set; }

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);

			if (paragraphUnit.IsStructure) return;

			var segmentPairsTotal = paragraphUnit.SegmentPairs.Count();
			var segmentPairsRemaining = segmentPairsTotal;

			var markedForRemoval = new List<MetadataTransferObject>();
			foreach (var datum in TranslationData)
			{
				if (segmentPairsRemaining == 0) break;
				var metaData = datum.TranslationOriginInformation;
				var segmentPairs = GetValidSegmentPairs(paragraphUnit, datum.SegmentIds);

				foreach (var segmentPair in segmentPairs.Skip(segmentPairsTotal - segmentPairsRemaining))
				{
					segmentPairsRemaining--;
					var translationOrigin = segmentPair.Properties.TranslationOrigin;

					translationOrigin.SetMetaData("quality_estimation", metaData.QualityEstimation);
					translationOrigin.SetMetaData("model", metaData.Model);
				}
				datum.SegmentIds = datum.SegmentIds.Except(segmentPairs.Select(sp => sp.Properties.Id)).ToList();
				if (datum.SegmentIds.Count == 0) markedForRemoval.Add(datum);
			}

			TranslationData = TranslationData.Except(markedForRemoval).ToList();
		}

		public List<ISegmentPair> GetValidSegmentPairs(IParagraphUnit paragraphUnit, IEnumerable<SegmentId> segmentIds)
		{
			var segmentPairs = segmentIds.Select(paragraphUnit.GetSegmentPair).ToList();
			segmentPairs.RemoveAll(sp => sp is null);

			return segmentPairs.Where(sp => !sp.Properties.IsLocked).ToList();
		}
	}
}