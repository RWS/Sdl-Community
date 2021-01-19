using System;
using System.Collections.Generic;
using System.IO;
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

		public string GetUniqueFileName(string filePath, string suffix)
		{
			var directoryName = Path.GetDirectoryName(filePath);
			var fileName = Path.GetFileName(filePath);
			var fileExtension = Path.GetExtension(fileName);
			var fileNameWithoutExtension = GetFileNameWithoutExtension(fileName, fileExtension);

			var index = 1;
			var uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
															 + "." + (string.IsNullOrEmpty(suffix) ? string.Empty : suffix + "_")
															 + index.ToString().PadLeft(4, '0') + fileExtension);

			if (File.Exists(uniqueFilePath))
			{
				while (File.Exists(uniqueFilePath))
				{
					index++;
					uniqueFilePath = Path.Combine(directoryName, fileNameWithoutExtension
																 + "." + (string.IsNullOrEmpty(suffix) ? string.Empty : suffix + "_")
																 + index.ToString().PadLeft(4, '0') + fileExtension);
				}
			}

			return uniqueFilePath;
		}

		public string GetValidFolderPath(string initialPath)
		{
			if (string.IsNullOrWhiteSpace(initialPath))
			{
				return string.Empty;
			}

			var outputFolder = initialPath;
			if (Directory.Exists(outputFolder))
			{
				return outputFolder;
			}

			while (outputFolder.Contains("\\"))
			{
				outputFolder = outputFolder.Substring(0, outputFolder.LastIndexOf("\\", StringComparison.Ordinal));
				if (Directory.Exists(outputFolder))
				{
					return outputFolder;
				}
			}

			return outputFolder;
		}

		private string GetFileNameWithoutExtension(string fileName, string extension)
		{
			if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(extension))
			{
				return fileName;
			}

			if (extension.Length > fileName.Length || !fileName.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
			{
				return fileName;
			}

			return fileName.Substring(0, fileName.Length - extension.Length);
		}
	}
}
