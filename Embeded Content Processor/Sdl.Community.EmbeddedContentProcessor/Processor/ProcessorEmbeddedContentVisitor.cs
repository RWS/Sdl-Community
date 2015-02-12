using System;
using System.Collections.Generic;
using System.Diagnostics;
using Sdl.Community.EmbeddedContentProcessor.Infrastructure;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.Formatting;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Community.EmbeddedContentProcessor.Settings;

namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public class ProcessorEmbeddedContentVisitor: IMarkupDataVisitor, IParagraphCreator
    {
        public const string IsCData = "isCData";
        private const string EmbeddedContentMetaKey = "OriginalEmbeddedContent";

        private readonly IDocumentItemFactory _itemFactory;
        private readonly IParagraph _parentParagraph ;
        private IAbstractMarkupDataContainer _currentContainer;
        private readonly IContentEvaluator _contentEvaluator;
        private readonly List<MatchRule> _matchRules;

        public ProcessorEmbeddedContentVisitor(IDocumentItemFactory itemFactory, IContentEvaluator evaluator, List<MatchRule> matchRules )
        {
            this._itemFactory = itemFactory;

            var paragraphUnit = _itemFactory.CreateParagraphUnit(LockTypeFlags.Unlocked);
            _parentParagraph = paragraphUnit.Source;
            _currentContainer = _parentParagraph;
            _contentEvaluator = evaluator;
            _matchRules = matchRules;
        }
        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            var newComment = _itemFactory.CreateCommentMarker(commentMarker.Comments);
            _parentParagraph.Add(newComment);
            _currentContainer = newComment;

            VisitChildElements(commentMarker);

            _currentContainer = newComment.Parent;
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            _currentContainer.Add((ILocationMarker)location.Clone());
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            _currentContainer.Add((ILockedContent)lockedContent.Clone());
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
            _currentContainer.Add((IPlaceholderTag)tag.Clone());
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

            _currentContainer.Add(newTagPair);
            _currentContainer = newTagPair;

            VisitChildElements(tagPair);

            _currentContainer = newTagPair.Parent;

        }

        public void VisitText(IText text)
        {
            var inputText = text.Properties.Text;

            if (HasEmbededContent(text))
            {
                //TODO:apply regex rules
                List<ContentMatch> matches = _contentEvaluator.Evaluate(inputText, _matchRules);

                int lastMatchIndex = 0;
                foreach (var match in matches)
                {
                    if (match.Index > lastMatchIndex)
                    {
                        _currentContainer.Add(CreateText(inputText.Substring(lastMatchIndex, match.Index - lastMatchIndex)));
                    }
                    else if (lastMatchIndex > match.Index)
                    {
                        //there are multiple matches found in one string, if already applied match index is behind current, just skip it
                        continue;
                    }

                    switch (match.Type)
                    {
                        case ContentMatch.TagType.Placeholder:
                            _currentContainer.Add(CreatePlaceholderTag(match));
                            break;
                        case ContentMatch.TagType.TagPairOpening:
                            AddOpenTagContainer(match);
                            break;
                        case ContentMatch.TagType.TagPairClosing:
                            AddCloseTagContainer();
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
            else
            {
                _currentContainer.Add(CreateText(inputText));
            }
        }

        private void AddCloseTagContainer()
        {
            var currentTagPair = _currentContainer as ITagPair;

            if (currentTagPair != null)
            {
                _currentContainer = currentTagPair.Parent;
            }
            else
            {
                //exit locked content
                var lockedContainer = _currentContainer as ILockedContainer;

                if (lockedContainer != null)
                {
                    _currentContainer = lockedContainer.LockedContent.Parent;
                }
                else
                {
                    Debug.Assert(false, "Tags or locked content are out of line!");
                }
            }
        }

        private void AddOpenTagContainer(ContentMatch match)
        {
            if (match.MatchRule.IsContentTranslatable)
            {
                ITagPair tagPair = CreateTagPair(match);
                _currentContainer.Add(tagPair);
                _currentContainer = tagPair;
            }
            else
            {
                //treat non-translatable content as locked
                ILockedContent lockedContent = CreateLockedContent();
                _currentContainer.Add(lockedContent);
                _currentContainer = lockedContent.Content;
            }
        }

        private ILockedContent CreateLockedContent()
        {
            ILockedContentProperties lockedProps =
               _itemFactory.PropertiesFactory.CreateLockedContentProperties(LockTypeFlags.Manual);
            return _itemFactory.CreateLockedContent(lockedProps);
        }

        private ITagPair CreateTagPair(ContentMatch match)
        {
            IStartTagProperties startProperties = _itemFactory.PropertiesFactory.CreateStartTagProperties(match.Value);

            startProperties.CanHide = match.MatchRule.CanHide;
            startProperties.IsSoftBreak = match.MatchRule.IsSoftBreak;
            startProperties.IsWordStop = match.MatchRule.IsWordStop;

            startProperties.DisplayText = GetDisplayName(match.Value);
            startProperties.Formatting = FormattingInflator.InflateFormatting(match.MatchRule.Formatting);
            startProperties.SegmentationHint = match.MatchRule.SegmentationHint;
            startProperties.SetMetaData(EmbeddedContentMetaKey, match.Value);

             IEndTagProperties endProperties = _itemFactory.PropertiesFactory.CreateEndTagProperties(match.Value);

            endProperties.CanHide = match.MatchRule.CanHide;
            endProperties.IsSoftBreak = match.MatchRule.IsSoftBreak;
            endProperties.IsWordStop = match.MatchRule.IsWordStop;
            endProperties.DisplayText = GetDisplayName(match.Value);
            endProperties.SetMetaData(EmbeddedContentMetaKey, match.Value);

            return _itemFactory.CreateTagPair(startProperties, endProperties);
        }

        private IAbstractMarkupData CreatePlaceholderTag(ContentMatch match)
        {
            var placeholderProps = _itemFactory.PropertiesFactory.CreatePlaceholderTagProperties(match.Value);


            placeholderProps.CanHide = match.MatchRule.CanHide;
            placeholderProps.IsSoftBreak = match.MatchRule.IsSoftBreak;
            placeholderProps.IsWordStop = match.MatchRule.IsWordStop;

            placeholderProps.DisplayText = GetDisplayName(match.Value);
            placeholderProps.SegmentationHint = match.MatchRule.SegmentationHint;
            placeholderProps.TagContent = match.Value;

            if (!string.IsNullOrEmpty(match.MatchRule.TextEquivalent))
            {
                placeholderProps.TextEquivalent = match.MatchRule.TextEquivalent;
            }
            placeholderProps.SetMetaData(EmbeddedContentMetaKey, match.Value);

            return _itemFactory.CreatePlaceholderTag(placeholderProps);
        }

        private string GetDisplayName(string tagContent)
        {
            //trim the start and end to get rid of opening and closing XML tags
            string displayName = tagContent.TrimStart('<', '/');
            displayName = displayName.TrimEnd('>', '/');

            int spaceIndex = displayName.IndexOf(" ", StringComparison.Ordinal);
            if (spaceIndex > 0 && spaceIndex < 10)
            {
                //return first word
                displayName = displayName.Substring(0, spaceIndex);
            }
            else if (displayName.Length > 10)
            {
                //restrict to first 10 characters
                displayName = displayName.Substring(0, 10);
            }

            return displayName;
        }

        protected virtual bool HasEmbededContent(IAbstractMarkupData markupData)
        {
            var tagPair = _currentContainer as ITagPair;
            if (tagPair == null) return false;
            var metaDataContainer = tagPair.StartTagProperties as IMetaDataContainer;
            if (metaDataContainer == null || !metaDataContainer.HasMetaData) return false;

            if (!metaDataContainer.MetaDataContainsKey(IsCData)) return false;

            return bool.Parse(metaDataContainer.GetMetaData(IsCData));

        }

        private IText CreateText(string textContent)
        {
            ITextProperties textProps = _itemFactory.PropertiesFactory.CreateTextProperties(textContent);
            return _itemFactory.CreateText(textProps);
        }

        private void VisitChildElements(IEnumerable<IAbstractMarkupData> markupDatas)
        {
            foreach (var child in markupDatas)
            {
                child.AcceptVisitor(this);
            }
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

        public IParagraph GetParagraph()
        {
            return _parentParagraph;
        }
    }
}
