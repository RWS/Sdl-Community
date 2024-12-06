using Sdl.FileTypeSupport.Framework.BilingualApi;
using SDLXLIFFSliceOrChange;
using System.Text.RegularExpressions;

namespace SdlXliff.Toolkit.Integration.File
{
    internal class SegmentVisitor : IMarkupDataVisitor
    {
        public SegmentVisitor(ReplaceSettings settings)
        {
            SetUp(settings);
        }

        private RegexOptions RegexOptions { get; set; }

        private string ReplacementText { get; set; }

        private string SearchPattern { get; set; }

        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            foreach (var element in commentMarker)
                element.AcceptVisitor(this);
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            foreach (var element in marker)
                element.AcceptVisitor(this);
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            foreach (var element in revisionMarker)
                element.AcceptVisitor(this);
        }

        public void VisitSegment(ISegment segment)
        {
            foreach (var element in segment)
                element.AcceptVisitor(this);
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            foreach (var element in tagPair)
            {
                element.AcceptVisitor(this);
            }
        }

        public void VisitText(IText text)
        {
            var originalText = text.Properties.Text;
            var replacedText = Regex.Replace(originalText, SearchPattern, ReplacementText, RegexOptions);
            text.Properties.Text = replacedText;
        }

        private void SetUp(ReplaceSettings settings)
        {
            var isSource = settings.SourceSearchText is not null && !string.IsNullOrEmpty(settings.SourceSearchText);

            if (isSource)
            {
                SearchPattern = settings.SourceSearchText;
                ReplacementText = settings.SourceReplaceText;
            }
            else
            {
                SearchPattern = settings.TargetSearchText;
                ReplacementText = settings.TargetReplaceText;
            }

            RegexOptions = !settings.MatchCase
                ? RegexOptions.IgnoreCase | RegexOptions.Multiline
                : RegexOptions.None | RegexOptions.Multiline;

            if (settings.UseRegEx) return;

            SearchPattern = Regex.Escape(SearchPattern);
            SearchPattern = settings.MatchWholeWord ? string.Format(@"(\b(?<!\w){0}\b|(?<=^|\s){0}(?=\s|$))", SearchPattern) : SearchPattern;
        }
    }
}