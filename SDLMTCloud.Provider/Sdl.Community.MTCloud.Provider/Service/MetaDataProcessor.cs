using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.MTCloud.Provider.Service
{
	internal class MetaDataProcessor : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentId> _usedIds = new List<SegmentId>();

		public MetaDataProcessor(List<MetadataTransferObject> translationData)
		{
			TranslationData = translationData;
		}

		public List<MetadataTransferObject> TranslationData { get; set; }

		public List<ISegmentPair> GetValidSegmentPairs(IParagraphUnit paragraphUnit, IEnumerable<SegmentId> segmentIds)
		{
			var segmentPairs = segmentIds.Select(paragraphUnit.GetSegmentPair).ToList();
			segmentPairs.RemoveAll(sp => sp is null || _usedIds.Contains(sp.Properties.Id));

			return segmentPairs.Where(sp => !sp.Properties.IsLocked).ToList();
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);

			if (paragraphUnit.IsStructure) return;

			var segmentPairsRemaining = paragraphUnit.SegmentPairs.Count();
			var markedForRemoval = new List<MetadataTransferObject>();

			foreach (var datum in TranslationData)
			{
				if (segmentPairsRemaining == 0) break;
				var metaData = datum.TranslationOriginData;
				var segmentPairs = GetValidSegmentPairs(paragraphUnit, datum.SegmentIds);

				foreach (var segmentPair in segmentPairs)
				{
					segmentPairsRemaining--;

					var segmentPairProperties = segmentPair.Properties;
					var translationOrigin = segmentPairProperties.TranslationOrigin;

					translationOrigin.SetMetaData("quality_estimation", metaData.QualityEstimations[segmentPairProperties.Id]);
					translationOrigin.SetMetaData("model", metaData.Model);
				}
				_usedIds.AddRange(segmentPairs.Select(sp => sp.Properties.Id));
				datum.SegmentIds = datum.SegmentIds.Except(_usedIds).ToList();

				if (datum.SegmentIds.Count != 0) continue;
				MarkDatumForRemoval(markedForRemoval, datum);
			}

			TranslationData = TranslationData.Except(markedForRemoval).ToList();
		}

		private void MarkDatumForRemoval(List<MetadataTransferObject> markedForRemoval, MetadataTransferObject datum)
		{
			markedForRemoval.Add(datum);
		}
	}
}