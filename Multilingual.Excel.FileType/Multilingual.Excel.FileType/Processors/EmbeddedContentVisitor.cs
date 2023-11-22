using System.Collections.Generic;
using System.Diagnostics;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Processors
{
    class EmbeddedContentVisitor : IMarkupDataVisitor
    {
        readonly List<MatchRule> _matchRules;
        IAbstractMarkupDataContainer _currentContainer;
        readonly IDocumentItemFactory _factory;
        readonly IParagraph _parentParagraph;

        public EmbeddedContentVisitor(IDocumentItemFactory factory, List<MatchRule> matchRules)
        {
            _factory = factory;

            IParagraphUnit paragraphUnit = factory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            _parentParagraph = paragraphUnit.Source;
            _currentContainer = _parentParagraph;

            _matchRules = matchRules;
        }

        public IParagraph GeneratedParagraph
        {
            get
            {
                Debug.Assert(_parentParagraph == _currentContainer);

                return _parentParagraph;
            }
        }

        #region IMarkupDataVisitor Members

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            ICommentMarker newComment = _factory.CreateCommentMarker(commentMarker.Comments);
            _currentContainer.Add(newComment);
            _currentContainer = newComment;

            VisitChildren(commentMarker);

            _currentContainer = newComment.Parent;
        }
        public void VisitParagraph(IParagraph paragraph)
        {
            VisitChildren(paragraph);
        }

        private void VisitChildren(IEnumerable<IAbstractMarkupData> container)
        {
            foreach (var element in container)
            {
                if(element is IStructureTag structureTag)
                    _currentContainer.Add((IStructureTag)structureTag.Clone());
                else
                    element.AcceptVisitor(this);
            }
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            _currentContainer.Add((ILocationMarker)location.Clone());
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            //just clone
            _currentContainer.Add((ILockedContent)lockedContent.Clone());
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            IOtherMarker newOtherMarker = _factory.CreateOtherMarker();
            newOtherMarker.Id = marker.Id;
            newOtherMarker.MarkerType = marker.MarkerType;

            _currentContainer.Add(newOtherMarker);
            _currentContainer = newOtherMarker;

            VisitChildren(marker);

            _currentContainer = newOtherMarker.Parent;
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            _currentContainer.Add((IPlaceholderTag)tag.Clone());
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            IRevisionMarker newRevisionMarker = CreateRevisionOrFeedback(revisionMarker.Properties);

            _currentContainer.Add(newRevisionMarker);
            _currentContainer = newRevisionMarker;

            VisitChildren(revisionMarker);

            _currentContainer = newRevisionMarker.Parent;
        }

        private IRevisionMarker CreateRevisionOrFeedback(IRevisionProperties properties)
        {
            switch (properties.RevisionType)
            {
                case RevisionType.Insert:
                case RevisionType.Delete:
                case RevisionType.Unchanged:
                    return _factory.CreateRevision(properties);

                case RevisionType.FeedbackAdded:
                case RevisionType.FeedbackComment:
                case RevisionType.FeedbackDeleted:
                    return _factory.CreateFeedback(properties);

                default:
                    return _factory.CreateRevision(properties);
            }
        }

        public void VisitSegment(ISegment segment)
        {
            ISegment newSegment = _factory.CreateSegment(segment.Properties);
            _currentContainer.Add(newSegment);
            _currentContainer = newSegment;

            VisitChildren(segment);

            _currentContainer = newSegment.Parent;
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            ITagPair newTagPair = _factory.CreateTagPair(tagPair.StartTagProperties, tagPair.EndTagProperties);
            newTagPair.AddSubSegmentReferences(tagPair.SubSegments);
            _currentContainer.Add(newTagPair);
            _currentContainer = newTagPair;

            VisitChildren(tagPair);

            _currentContainer = newTagPair.Parent;
        }

        public void VisitText(IText text)
        {
            //apply regex rules to text runs
            string inputText = text.Properties.Text;

            var matches = RegexProcessorHelper.ApplyRegexRules(inputText, _matchRules);

            int lastMatchIndex = 0;
            foreach (RegexMatch match in matches)
            {
                if (match.Index > lastMatchIndex)
                {
                    _currentContainer.Add(CreateText(inputText.Substring(lastMatchIndex, match.Index - lastMatchIndex)));
                }
                else if (lastMatchIndex > match.Index)
                {
                    //there are multiple matches found in one string, if already applied match index is behind current, just skip it
                    //this should never be the case, leaving the check for robustness
                    continue;
                }

                switch (match.Type)
                {
                    case RegexMatch.TagType.Placeholder:
                        _currentContainer.Add(CreatePlaceholderTag(match));
                        break;
                    case RegexMatch.TagType.TagPairOpening:
                        AddOpenTagContainer(match);
                        break;
                    case RegexMatch.TagType.TagPairClosing:
                        AddCloseTagContainer(match);
                        break;
                    default:
                        Debug.Assert(false, "Tag match type not defined!");
                        break;
                }

                lastMatchIndex = match.Index + match.Value.Length;
            }

            //output anything that comes after input
            if (lastMatchIndex < inputText.Length)
            {
                _currentContainer.Add(CreateText(inputText.Substring(lastMatchIndex, inputText.Length - lastMatchIndex)));
            }
        }

        private void AddOpenTagContainer(RegexMatch match)
        {
            ITagPair tagPair = CreateTagPair(match);

            if (!match.Rule.IsContentTranslatable)
            {
                //treat non-translatable content as locked
                ILockedContent lockedContent = CreateLockedContent();
                _currentContainer.Add(lockedContent);
                lockedContent.Content.Add(tagPair);
            }
            else
            {
                _currentContainer.Add(tagPair);
            }

            _currentContainer = tagPair;
        }

        private void AddCloseTagContainer(RegexMatch match)
        {
            ITagPair currentTagPair = _currentContainer as ITagPair;

            if (currentTagPair != null)
            {
                currentTagPair.EndTagProperties.TagContent = match.Value;
                currentTagPair.EndTagProperties.SetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey, match.Value);
                _currentContainer = currentTagPair.Parent;
                if (!(_currentContainer is ILockedContainer))
                    return;
            }

            //exit locked content
            ILockedContainer lockedContainer = _currentContainer as ILockedContainer;

            if (lockedContainer != null)
            {
                _currentContainer = lockedContainer.LockedContent.Parent;
            }
            else
            {
                Debug.Assert(false, "Tags or locked content are out of line!");
            }

        }

        private ILockedContent CreateLockedContent()
        {
            ILockedContentProperties lockedProps =
                _factory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);

            return _factory.CreateLockedContent(lockedProps);
        }

        private IText CreateText(string textContent)
        {
            ITextProperties textProps = _factory.PropertiesFactory.CreateTextProperties(textContent);
            return _factory.CreateText(textProps);
        }

        private ITagPair CreateTagPair(RegexMatch match)
        {
            var startProperties = RegexProcessorHelper
                .CreateStartTagProperties(_factory.PropertiesFactory, match.Value, match.Rule);
            startProperties.SetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey, match.Value);

            var endProperties = RegexProcessorHelper
                .CreateEndTagProperties(_factory.PropertiesFactory, match.Value, match.Rule);
            endProperties.SetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey, match.Value);

            return _factory.CreateTagPair(startProperties, endProperties);
        }

        private IPlaceholderTag CreatePlaceholderTag(RegexMatch match)
        {
            var placeholderProps = RegexProcessorHelper
                .CreatePlaceholderTagProperties(_factory.PropertiesFactory, match.Value, match.Rule);
            placeholderProps.SetMetaData(EmbeddedContentConstants.EmbeddedContentMetaKey, match.Value);

            return _factory.CreatePlaceholderTag(placeholderProps);
        }

        #endregion
    }
}
