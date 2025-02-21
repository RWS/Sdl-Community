using System.Collections.Generic;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Processors
{
    internal class EmbeddedContentGeneratorVisitor : IMarkupDataVisitor
    {
        private readonly IDocumentItemFactory _documentItemFactory;
        private readonly IParagraph _clonedParagraph;
        private readonly Dictionary<string, List<ITagPair>> _autoClonedTagPairsById;
        private bool _shouldRemoveLockedContent;

        internal EmbeddedContentGeneratorVisitor(
            IDocumentItemFactory documentFactory,
            IParagraph clonedParagraph)
        {
            _documentItemFactory = documentFactory;

            _clonedParagraph = clonedParagraph;
            _autoClonedTagPairsById = new Dictionary<string, List<ITagPair>>();
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            var startTagProperties = tagPair.StartTagProperties;
            var originalStartText = startTagProperties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey);

            if (!string.IsNullOrEmpty(originalStartText))
            {
                var clonedTagPair = GetAllSubItems<ITagPair>(_clonedParagraph).First(originalTagPair => originalTagPair.Equals(tagPair));

                var startTagIndex = clonedTagPair.Parent.IndexOf(clonedTagPair);

                var distanceToLastSibling = clonedTagPair.Parent.Count - startTagIndex - 1;

                var shoudWriteStartTagContent = ShouldWriteStartTagContentFor(tagPair);
                if (shoudWriteStartTagContent)
                    clonedTagPair.Parent.Insert(startTagIndex, CreateText(tagPair.StartTagProperties));

                VisitChildren(tagPair);

                clonedTagPair.MoveAllItemsTo(
                    clonedTagPair.Parent,
                    shoudWriteStartTagContent ? startTagIndex + 1 : startTagIndex);

                if (ShouldWriteEndTagContentFor(tagPair))
                {
                    var endTagIndex = clonedTagPair.Parent.Count - distanceToLastSibling;
                    clonedTagPair.Parent.Insert(endTagIndex, CreateText(tagPair.EndTagProperties));
                }

                if (clonedTagPair.Parent is ILockedContainer)
                {
                    _shouldRemoveLockedContent = true;
                }

                clonedTagPair.RemoveFromParent();
            }

            else
            {
                VisitChildren(tagPair);
            }
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            var placeholderText = tag.Properties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey);

            if (placeholderText != null)
            {
                var clonedPlaceholder = GetAllSubItems<IPlaceholderTag>(_clonedParagraph).First(originalplaceholderTag => originalplaceholderTag.Equals(tag));

                var placeholderIndex = clonedPlaceholder.Parent.IndexOf(clonedPlaceholder);

                clonedPlaceholder.Parent.Insert(placeholderIndex, CreateText(tag.TagProperties));

                clonedPlaceholder.RemoveFromParent();
            }
        }

        public void VisitText(IText text)
        {
            // No implementation
        }

        public void VisitSegment(ISegment segment)
        {
            VisitChildren(segment);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            // No implementation
        }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            VisitChildren(commentMarker);
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            VisitChildren(marker);
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            var clonedLockedContent = GetAllSubItems<ILockedContent>(_clonedParagraph).First(originalTagPair => originalTagPair.Equals(lockedContent));

            VisitChildren(lockedContent.Content);
            if (_shouldRemoveLockedContent)
            {
                var lockedContentIndex = clonedLockedContent.Parent.IndexOf(clonedLockedContent);
                clonedLockedContent.Content.MoveAllItemsTo(clonedLockedContent.Parent, lockedContentIndex);

                clonedLockedContent.RemoveFromParent();
                _shouldRemoveLockedContent = false;
            }
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            VisitChildren(revisionMarker);
        }

        public void VisitParagraph(IParagraph paragraph)
        {
            var tagPairs = GetAllSubItems<ITagPair>(paragraph);
            foreach (var tagPair in tagPairs)
            {
                // if auto-cloned regex tag pair
                if (
                    tagPair.StartTagProperties.GetMetaData(Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.Constants.AUTOCLONED_TAG_PAIR_KEY) == bool.TrueString
                    && tagPair.StartTagProperties.MetaDataContainsKey(EmbeddedContentConstants.EmbeddedContentMetaKey))
                {
                    var id = tagPair.TagProperties.TagId.Id;
                    if (_autoClonedTagPairsById.TryGetValue(id, out var list))
                        list.Add(tagPair);
                    else
                        _autoClonedTagPairsById.Add(id, new List<ITagPair> { tagPair });
                }
            }

            VisitChildren(paragraph);

            _autoClonedTagPairsById.Clear();
        }

        private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
        {
            foreach (var element in container.Where(x => !(x is IStructureTag)))
            {
                element.AcceptVisitor(this);
            }
        }

        private IText CreateText(IAbstractBasicTagProperties properties)
        {
            ITextProperties textProps = _documentItemFactory.PropertiesFactory.CreateTextProperties(properties.GetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey));
            return _documentItemFactory.CreateText(textProps);
        }

        private static IList<T> GetAllSubItems<T>(IAbstractMarkupDataContainer container)
        {
            var abstractMarkupDataList = new List<T>();
            abstractMarkupDataList.AddRange(container.AllSubItems.OfType<T>().ToList());

            foreach (var lockedContent in container.AllSubItems.OfType<ILockedContent>().ToList())
            {
                abstractMarkupDataList.AddRange(GetAllSubItems<T>(lockedContent.Content));
            }

            return abstractMarkupDataList;
        }

        private bool ShouldWriteStartTagContentFor(ITagPair tagPair)
        {
            var id = tagPair.TagProperties.TagId.Id;
            if (!_autoClonedTagPairsById.TryGetValue(id, out var list))
                return true;

            return ReferenceEquals(tagPair, list[0]);
        }

        private bool ShouldWriteEndTagContentFor(ITagPair tagPair)
        {
            var id = tagPair.TagProperties.TagId.Id;
            if (!_autoClonedTagPairsById.TryGetValue(id, out var list))
                return true;

            return ReferenceEquals(tagPair, list[list.Count - 1]);
        }
    }
}