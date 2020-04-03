using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IUserNameService
	{
		void AnonymizeCreatedByAndEdited(ISegmentPair segment, IBatchAnonymizerSettings anonymizerSettings);
		void AnonymizeCommentAuthor(ISegmentPair segmentPair, string value);
		//TODO: value for tracked changes.
	}
}
