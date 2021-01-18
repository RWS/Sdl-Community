using System.Collections.Generic;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class CommonService
	{
		private readonly FileInfoService _fileInfoService;

		public CommonService(FileInfoService fileInfoService)
		{
			_fileInfoService = fileInfoService;
		}

		public ISegmentPair GetSegmentPair(IStudioDocument document, string paragraphUnitId, string segmentId)
		{
			foreach (var segmentPair in document.SegmentPairs)
			{
				if (paragraphUnitId == segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id
					&& segmentId == segmentPair.Properties.Id.Id)
				{
					return segmentPair;
				}
			}

			return null;
		}

		public List<ISegmentPair> GetSegmentPairs(IStudioDocument document, bool exportSelectedSegments)
		{
			List<ISegmentPair> segmentPairs;
			if (exportSelectedSegments)
			{
				segmentPairs = document.GetSelectedSegmentPairs().ToList();
				if (segmentPairs.Count == 0)
				{
					var activeSegmentPair = document.GetActiveSegmentPair();
					if (activeSegmentPair != null)
					{
						segmentPairs = new List<ISegmentPair> { document.GetActiveSegmentPair() };
					}
				}
			}
			else
			{
				segmentPairs = document.FilteredSegmentPairs.ToList();
			}

			return segmentPairs;
		}

		public List<ProjectFile> GetProjectFiles(IEnumerable<ISegmentPair> segmentPairs)
		{
			var projectFiles = new List<ProjectFile>();
			foreach (var segmentPair in segmentPairs)
			{
				var projectFile = segmentPair.GetProjectFile();
				var projectFileId = projectFile.Id.ToString();
				if (!projectFiles.Exists(a => a.Id.ToString() == projectFileId))
				{
					projectFiles.Add(projectFile);
				}
			}

			return projectFiles;
		}

		public List<SegmentPairInfo> GetSegmentPairInfos(List<ProjectFileInfo> projectFiles, IEnumerable<ISegmentPair> selectedSegmentPairs)
		{
			var segmentPairInfos = new List<SegmentPairInfo>();

			foreach (var selectedSegmentPair in selectedSegmentPairs)
			{
				var segmentPairInfo =
					new SegmentPairInfo
					{
						ParagraphUnitId = selectedSegmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id,
						SegmentId = selectedSegmentPair.Properties.Id.Id
					};

				segmentPairInfo.FileId = GetProjectFileInfo(projectFiles, segmentPairInfo.ParagraphUnitId,
					segmentPairInfo.SegmentId).FileId;

				segmentPairInfos.Add(segmentPairInfo);
			}

			return segmentPairInfos;
		}

		public List<ProjectFileInfo> GetProjectFileInfos(List<ProjectFile> projectFiles)
		{
			var projectFileInfos = new List<ProjectFileInfo>();
			foreach (var documentFile in projectFiles)
			{
				var files = _fileInfoService.GetProjectFiles(documentFile.LocalFilePath);
				foreach (var projectFileInfo in files)
				{
					projectFileInfos.Add(projectFileInfo);
				}
			}

			return projectFileInfos;
		}

		public ProjectFileInfo GetProjectFileInfo(List<ProjectFileInfo> projectFiles, string paragraphId, string segmentId)
		{
			if (projectFiles == null)
			{
				return null;
			}

			foreach (var projectFile in projectFiles)
			{
				var paragraphInfo = projectFile.ParagraphInfos.FirstOrDefault(a => a.ParagraphId == paragraphId);
				var segmentInfo = paragraphInfo?.SegmentInfos.FirstOrDefault(a => a.OriginalSegmentId == segmentId);
				if (segmentInfo != null)
				{
					return projectFile;
				}
			}

			return null;
		}
	}
}
