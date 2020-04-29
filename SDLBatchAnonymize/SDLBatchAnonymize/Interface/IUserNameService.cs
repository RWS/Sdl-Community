using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize.Interface
{
	public interface IUserNameService
	{
		/// <summary>
		/// Anonymize segment pair metadata (Created by and Last Edited By)
		/// </summary>
		void AnonymizeCreatedByAndEdited(ISegmentPair segment, IBatchAnonymizerSettings anonymizerSettings);
		/// <summary>
		/// Anonymize segment comments
		/// </summary>
		void AnonymizeCommentAuthor(ISegmentPair segmentPair, string commentAuthor);
		/// <summary>
		/// Anonymize file comments
		/// </summary>
		void AnonymizeCommentAuthor(IFileProperties fileProperties, string commentAuthor);

		/// <summary>
		/// Anonymize segment tracked changes
		/// </summary>
		void AnonymizeRevisionMarker(ISegmentPair segmentPair, string revisionAuthor);
	}
}
