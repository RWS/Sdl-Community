using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IUserNameService
	{
		void AnonymizeModifiedBy(ISegment segment, string value);
		void AnonymizeLastEditedBy(ISegment segment, string value);
		void AnonymizeCommentAuthor(ISegmentPair segmentPair, string value);
		//TODO: value for tracked changes.
	}
}
