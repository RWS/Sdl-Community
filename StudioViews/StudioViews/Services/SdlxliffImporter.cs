using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	public class SdlxliffImporter
	{
		private readonly ProjectFileService _projectFileService;
		private readonly FilterItemService _filterItemService;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;

		public SdlxliffImporter(ProjectFileService projectFileService, FilterItemService filterItemService, 
			ParagraphUnitProvider paragraphUnitProvider)
		{
			_projectFileService = projectFileService;
			_filterItemService = filterItemService;
			_paragraphUnitProvider = paragraphUnitProvider;
		}

		public ImportResult UpdateFile(List<SegmentPairInfo> updatedSegmentPairs, List<string> excludeFilterIds, string filePathInput, string filePathOutput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);

			var contentWriter = new ContentImporter(updatedSegmentPairs, excludeFilterIds,
				_filterItemService, _paragraphUnitProvider);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return new ImportResult
			{
				Success = true,
				UpdatedSegments = contentWriter.UpdatedSegments,
				ExcludedSegments = contentWriter.ExcludedSegments,
				FilePath = filePathInput,
				UpdatedFilePath = filePathOutput,
				BackupFilePath = _projectFileService.GetUniqueFileName(filePathInput, "Backup")
			};
		}

		private List<SegmentPairInfo> GetSegmentPairs(string filePathInput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, null, null);

			var contentReader = new ContentReader();

			converter.AddBilingualProcessor(contentReader);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return contentReader.SegmentPairInfos;
		}

		private List<string> GetTagIds(string filePath)
		{
			string content;
			using (var r = new StreamReader(filePath, Encoding.UTF8))
			{
				content = r.ReadToEnd();
				r.Close();
			}

			var tagIds = new List<string>();
			var regexTagDefs = new Regex(@"\<tag\-defs[^\>]*\>(?<tags>.*?)\<\/tag\-defs\>", RegexOptions.IgnoreCase);
			var regexTag = new Regex(@"\<tag id=""(?<tagId>[^""].*?)""[^\>]*\>", RegexOptions.IgnoreCase);
			var regexTagDefMatches = regexTagDefs.Matches(content);
			if (regexTagDefMatches.Count > 0)
			{
				foreach (Match regexTagDefMatch in regexTagDefMatches)
				{
					var tags = regexTagDefMatch.Groups["tags"].Value;
					var tagMatches = regexTag.Matches(tags);
					if (tagMatches.Count > 0)
					{
						foreach (Match tagMatch in tagMatches)
						{
							var tagId = tagMatch.Groups["tagId"].Value;
							if (!tagIds.Contains(tagId))
							{
								tagIds.Add(tagId);
							}
						}
					}
				}
			}

			return tagIds;
		}
	}
}
