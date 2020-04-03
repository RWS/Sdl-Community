using System;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.SDLBatchAnonymize.Service
{
	public class UserNameService : IUserNameService
	{
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
		
		public void AnonymizeCommentAuthor(ISegmentPair segmentPair, string value)
		{
			
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
