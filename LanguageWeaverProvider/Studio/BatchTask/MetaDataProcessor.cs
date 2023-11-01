using System.Collections.Generic;
using System.Linq;
using LanguageWeaverProvider.BatchTask.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace LanguageWeaverProvider.BatchTask
{
	internal class MetaDataProcessor : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentId> _usedIds = new();

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

			if (paragraphUnit.IsStructure)
			{
				return;
			}

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
					var ratedSegment = metaData.RatedSegments[segmentPairProperties.Id];

					translationOrigin.SetMetaData(Constants.SegmentMetadata_QE, ratedSegment.QualityEstimation);
					translationOrigin.SetMetaData(Constants.SegmentMetadata_ShortModelName, ratedSegment.Model);
					translationOrigin.SetMetaData(Constants.SegmentMetadata_LongModelName, ratedSegment.ModelName);
					translationOrigin.SetMetaData(Constants.SegmentMetadata_Translation, ratedSegment.Translation);
				}

				_usedIds.AddRange(segmentPairs.Select(sp => sp.Properties.Id));
				datum.SegmentIds = datum.SegmentIds.Except(_usedIds).ToList();
				if (datum.SegmentIds.Count == 0)
				{
					markedForRemoval.Add(datum);
				}
			}

			TranslationData = TranslationData.Except(markedForRemoval).ToList();
		}
	}
}