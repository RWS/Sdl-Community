using System.Collections.Generic;
using Sdl.Community.EmbeddedContentProcessor.Infrastructure;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public class GeneratorEmbeddedContentVisitor: IMarkupDataVisitor,IParagraphCreator
    {
        private const string EmbeddedContentMetaKey = "OriginalEmbeddedContent";

        private readonly IDocumentItemFactory _itemFactory;
        private readonly IParagraph _parentParagraph;
        private IAbstractMarkupDataContainer _currentContainer;

        public GeneratorEmbeddedContentVisitor(IDocumentItemFactory itemFactory)
        {
            this._itemFactory = itemFactory;

            var paragraphUnit = _itemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            _parentParagraph = paragraphUnit.Source;
            _currentContainer = _parentParagraph;
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            //Nothing to do
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            //Nothing to do
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            //Nothing to do
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            var otherMarker = _itemFactory.CreateOtherMarker();
            otherMarker.Id = marker.Id;
            otherMarker.MarkerType = marker.MarkerType;

            _currentContainer.Add(otherMarker);
            _currentContainer = otherMarker;

            VisitChildElements(marker);

            _currentContainer = otherMarker.Parent;
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            TagToOriginalText(tag.Properties);
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            var newRevisionMarker = CreateRevisionOrFeedback(revisionMarker.Properties);

            _currentContainer.Add(newRevisionMarker);
            _currentContainer = newRevisionMarker;

            VisitChildElements(revisionMarker);

            _currentContainer = newRevisionMarker.Parent;
        }

        public void VisitSegment(ISegment segment)
        {
            var newSegment = _itemFactory.CreateSegment(segment.Properties);

            _currentContainer.Add(newSegment);
            _currentContainer = newSegment;

            VisitChildElements(segment);

            _currentContainer = newSegment.Parent;
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            var newTagPair = _itemFactory.CreateTagPair(tagPair.StartTagProperties, tagPair.EndTagProperties);
            TagToOriginalText(tagPair.StartTagProperties);

            _currentContainer.Add(newTagPair);
            _currentContainer = newTagPair;
            VisitChildElements(tagPair);

            TagToOriginalText(tagPair.EndTagProperties);
            _currentContainer = newTagPair.Parent;
        }

        public void VisitText(IText text)
        {
           _currentContainer.Add((IText)text.Clone());
        }

        public IParagraph GetParagraph()
        {
            return _parentParagraph;
        }

        private void VisitChildElements(IEnumerable<IAbstractMarkupData> markupDatas)
        {
            foreach (var child in markupDatas)
            {
                child.AcceptVisitor(this);
            }
        }

        private void TagToOriginalText(IMetaDataContainer properties)
        {
            var originalTag = properties.GetMetaData(EmbeddedContentMetaKey);
            if (string.IsNullOrEmpty(originalTag)) return;
            var text = CreateText(originalTag);
            _currentContainer.Add(text);
        }

        private IText CreateText(string textContent)
        {
            ITextProperties textProps = _itemFactory.PropertiesFactory.CreateTextProperties(textContent);
            return _itemFactory.CreateText(textProps);
        }

        private IRevisionMarker CreateRevisionOrFeedback(IRevisionProperties revisionProperties)
        {
            switch (revisionProperties.RevisionType)
            {
                case RevisionType.Insert:
                case RevisionType.Delete:
                case RevisionType.Unchanged:
                    return _itemFactory.CreateRevision(revisionProperties);

                case RevisionType.FeedbackAdded:
                case RevisionType.FeedbackComment:
                case RevisionType.FeedbackDeleted:
                    return _itemFactory.CreateFeedback(revisionProperties);

                default:
                    return _itemFactory.CreateRevision(revisionProperties);
            }
        }
    }
}
