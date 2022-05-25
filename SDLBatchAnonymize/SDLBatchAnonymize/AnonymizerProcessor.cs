using System;
using System.Linq;
using NLog;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.SDLBatchAnonymize
{
	public class AnonymizerProcessor : AbstractBilingualContentProcessor
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IResourceOriginsService _resourceOriginsService;
		private readonly IBatchAnonymizerSettings _settings;
		private readonly IUserNameService _usernameService;

		public AnonymizerProcessor(IBatchAnonymizerSettings settings, IUserNameService usernameService, IResourceOriginsService resourceOriginsService)
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
						_usernameService.AnonymizeCommentAuthor(segmentPair, _settings.CommentAuthorName);
					}
					if (_settings.TrackedChecked)
					{
						_usernameService.AnonymizeRevisionMarker(segmentPair, _settings.TrackedName);
					}
					if (_settings.ChangeMtChecked)
					{
						_resourceOriginsService.RemoveMt(segmentPair, _settings);
					}
					if (_settings.ChangeTmChecked)
					{
						_resourceOriginsService.RemoveTm(segmentPair, _settings);
					}
					if (_settings.RemoveMtCloudMetadata)
					{
						_resourceOriginsService.RemoveQe(segmentPair);
					}
				}
			}
			catch (Exception exception)
			{
				_logger.Error($"{exception.Message}\n {exception.StackTrace}");
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