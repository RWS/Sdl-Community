using System;
using System.Linq;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize
{
	public class AnonymizerProcessor : AbstractBilingualContentProcessor
	{
		public static readonly Log Log = Log.Instance;
		private readonly IBatchAnonymizerSettings _settings;
		private readonly IUserNameService _usernameService;
		private readonly IResourceOriginsService _resourceOriginsService;

		public AnonymizerProcessor(IBatchAnonymizerSettings settings, IUserNameService usernameService,IResourceOriginsService resourceOriginsService)
		{
			_settings = settings;
			_usernameService = usernameService;
			_resourceOriginsService = resourceOriginsService;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			base.ProcessParagraphUnit(paragraphUnit);
			if (paragraphUnit.IsStructure)
			{
				return;
			}
			try
			{
				foreach (var segmentPair in paragraphUnit.SegmentPairs.ToList())
				{
					if (_settings.CreatedByChecked || _settings.ModifyByChecked)
					{
						_usernameService.AnonymizeCreatedByAndEdited(segmentPair, _settings);
					}
					if (_settings.CommentChecked)
					{
						_usernameService.AnonymizeCommentAuthor(segmentPair,_settings.CommentAuthorName);
					}
					if (_settings.TrackedChecked)
					{
						_usernameService.AnonymizeRevisionMarker(segmentPair,_settings.TrackedName);
					}
					if (_settings.ChangeMtChecked)
					{
						_resourceOriginsService.RemoveMt(segmentPair,_settings);
					}
					if (_settings.ChangeTmChecked)
					{
						_resourceOriginsService.RemoveTm(segmentPair, _settings);
					}
				}
			}
			catch (Exception exception)
			{
				Log.Logger.Error($"{exception.Message}\n {exception.StackTrace}");
			}
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			if (_settings.CommentChecked)
			{
				_usernameService.AnonymizeCommentAuthor(fileInfo, _settings.CommentAuthorName);
			}
		}
	}
}
