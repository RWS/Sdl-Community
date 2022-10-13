using System.Collections.Generic;
using System.Linq;
using Multilingual.XML.FileType.Services;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.FileType.Processors
{
    public class CopySourceToEmptyTargetProcessor : AbstractBilingualContentProcessor
    {
	    private readonly SegmentBuilder _segmentBuilder;

        public const string CopySourceToTargetAppliedMetaKey = "CopySourceToTargetApplied";

        public CopySourceToEmptyTargetProcessor(SegmentBuilder segmentBuilder)
        {
	        _segmentBuilder = segmentBuilder;
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
	        if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
	        {
		        base.ProcessParagraphUnit(paragraphUnit);
                return;
	        }
	        
            foreach (var segmentPair in paragraphUnit.SegmentPairs)
            {
                var sourceSegment = segmentPair.Source;
                var targetSegment = segmentPair.Target;

                if (SegmentIsNotConfirmed(targetSegment) && SegmentIsEmpty(targetSegment))
                {
                    SetTranslationOriginMetadata(targetSegment);
                    DeepCopyFromSourceToTargetRemovingCommentMarker(sourceSegment, targetSegment);
                }
            }

            base.ProcessParagraphUnit(paragraphUnit);
        }

        private bool SegmentIsNotConfirmed(ISegment segment)
        {
	        if (segment == null)
	        {
		        return false;
	        }
            var confirmationLevel = segment.Properties.ConfirmationLevel;

            return confirmationLevel != ConfirmationLevel.ApprovedSignOff &&
                   confirmationLevel != ConfirmationLevel.ApprovedTranslation &&
                   confirmationLevel != ConfirmationLevel.Translated;
        }

        private bool SegmentIsEmpty(ISegment segment)
        {
            return !segment.Any();
        }

        private void DeepCopyFromSourceToTargetRemovingCommentMarker(IEnumerable<IAbstractMarkupData> source, IAbstractMarkupDataContainer target)
        {
            foreach (var item in source)
            {
                target.Add(item.Clone() as IAbstractMarkupData);
            }

            var commentMarkersToBeRemoved = target.AllSubItems.OfType<ICommentMarker>().ToList();

            foreach (var commentMarker in commentMarkersToBeRemoved)
            {
                var commentMarkersParent = commentMarker.Parent;
                var indexInParent = commentMarker.IndexInParent;

                commentMarker.RemoveFromParent();
                commentMarker.MoveAllItemsTo(commentMarkersParent, indexInParent);
            }
        }

        private void SetTranslationOriginMetadata(ISegment targetSegment)
        {
            var properties = targetSegment.Properties;

            if (properties.TranslationOrigin == null)
                properties.TranslationOrigin = _segmentBuilder.CreateTranslationOrigin();
            else
                properties.TranslationOrigin = (ITranslationOrigin)properties.TranslationOrigin.Clone();

            properties.TranslationOrigin.SetMetaData(CopySourceToTargetAppliedMetaKey, bool.TrueString);
        }
    }
}
