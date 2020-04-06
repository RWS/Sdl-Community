using System;
using System.Linq;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.Community.SDLBatchAnonymize.Visitor;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
	public class UserNameService : IUserNameService
	{
		private readonly CommentVisitor _commentVisitor = new CommentVisitor();

		public void AnonymizeCreatedByAndEdited(ISegmentPair segmentPair, IBatchAnonymizerSettings anonymizerSettings)
		{
			var translationOrigin = segmentPair.Properties.TranslationOrigin;

			if (anonymizerSettings.CreatedByChecked)
			{
				EditUserMetadata(translationOrigin, "created_by", anonymizerSettings.CreatedByName);
			}
			if (anonymizerSettings.ModifyByChecked)
			{
				EditUserMetadata(translationOrigin, "last_modified_by", anonymizerSettings.ModifyByName);
			}
		}
		
		public void AnonymizeCommentAuthor(ISegmentPair segmentPair, string commentAuthor)
		{
			_commentVisitor.AnonymizeCommentAuthod(segmentPair.Source, commentAuthor);
			_commentVisitor.AnonymizeCommentAuthod(segmentPair.Target, commentAuthor);
		}

		public void AnonymizeCommentAuthor(IFileProperties fileProperties, string commentAuthor)
		{
			var comments = fileProperties?.Comments?.Comments;
			if (comments == null || !comments.Any()) return;
			foreach (var fileComment in comments)
			{
				fileComment.Author = commentAuthor;
			}
		}

		private void EditUserMetadata(ITranslationOrigin translationOrigin, string metadataKey, string metadataValue)
		{
			var containsKey = translationOrigin.MetaDataContainsKey(metadataKey);
			if (!containsKey) return;
			translationOrigin.RemoveMetaData(metadataKey);
			translationOrigin.SetMetaData(metadataKey, metadataValue);
		}
	}
}
