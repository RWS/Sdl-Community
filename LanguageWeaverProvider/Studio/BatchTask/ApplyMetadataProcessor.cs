using LanguageWeaverProvider.BatchTask.Model;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using System.Collections.Generic;
using System.Linq;

namespace LanguageWeaverProvider.BatchTask
{
    internal class ApplyMetadataProcessor : AbstractBilingualContentProcessor
    {
        private readonly List<SegmentId> _usedIds = new();

        public ApplyMetadataProcessor(List<MetadataTransferObject> translationData)
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
            if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
            {
                base.ProcessParagraphUnit(paragraphUnit);
                return;
            }

            var segmentPairsRemaining = paragraphUnit.SegmentPairs.Count();
            var markedForRemoval = new List<MetadataTransferObject>();

            foreach (var datum in TranslationData)
            {
                if (segmentPairsRemaining == 0)
                {
                    break;
                }

                var metaData = datum.TranslationOriginData;
                var segmentPairs = GetValidSegmentPairs(paragraphUnit, datum.SegmentIds);
                foreach (var segmentPair in segmentPairs)
                {
                    segmentPairsRemaining--;

                    var segmentPairProperties = segmentPair.Properties;
                    var translationOrigin = segmentPairProperties.TranslationOrigin;
                    var ratedSegment = metaData.RatedSegments[segmentPairProperties.Id];

                    var translationData = new TranslationData
                    {
                        QualityEstimation = ratedSegment.QualityEstimation,
                        Translation = ratedSegment.Translation,
                        ModelName = ratedSegment.ModelName,
                        Model = ratedSegment.Model,
                        AutoSendFeedback = ratedSegment.AutosendFeedback,
                        Index = translationOrigin.GetLastTqeIndex()
                    };

                    translationOrigin.SetMetaData(translationData);
                }

                _usedIds.AddRange(segmentPairs.Select(sp => sp.Properties.Id));
                datum.SegmentIds = datum.SegmentIds.Except(_usedIds).ToList();
                if (datum.SegmentIds.Count == 0)
                {
                    markedForRemoval.Add(datum);
                }
            }

            base.ProcessParagraphUnit(paragraphUnit);

            TranslationData = TranslationData.Except(markedForRemoval).ToList();
        }
    }
}