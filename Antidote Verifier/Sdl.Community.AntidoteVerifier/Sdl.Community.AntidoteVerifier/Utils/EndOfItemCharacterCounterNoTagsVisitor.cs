﻿using Sdl.FileTypeSupport.Framework.BilingualApi;
using static Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.CharacterCountingIterator;

namespace Sdl.Community.AntidoteVerifier.Utils
{
    public class EndOfItemCharacterCounterNoTagsVisitor : ICharacterCountingVisitor
    {
	    public int Count { get; set; }

	    public void VisitCommentMarker(ICommentMarker commentMarker)
        {
            //Nothing to add
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
            //Nothing to add
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
            //Nothing to add
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
            //Nothing to add
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
            //Nothing to add
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
            //Nothing to add
        }

        public void VisitSegment(ISegment segment)
        {
            //Nothing to add
        }

        public void VisitTagPair(ITagPair tagPair)
        {
            //Nothing to add
        }

        public void VisitText(IText text)
        {
            //Nothing to add
        }
    }
}
