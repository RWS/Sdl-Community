using System.Diagnostics.Contracts;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.CleanUpTasks.Contracts
{
	[ContractClassFor(typeof(ISegmentHandler))]
    internal abstract class ISegmentHandlerContract : ISegmentHandler
    {
        public void VisitCommentMarker(ICommentMarker commentMarker)
        {
        }

        public void VisitLocationMarker(ILocationMarker location)
        {
        }

        public void VisitLockedContent(ILockedContent lockedContent)
        {
        }

        public void VisitOtherMarker(IOtherMarker marker)
        {
        }

        public void VisitPlaceholderTag(IPlaceholderTag tag)
        {
        }

        public void VisitRevisionMarker(IRevisionMarker revisionMarker)
        {
        }

        public void VisitSegment(ISegment segment)
        {
        }

        public void VisitTagPair(ITagPair tagPair)
        {
        }

        public void VisitText(IText text)
        {
        }
    }
}