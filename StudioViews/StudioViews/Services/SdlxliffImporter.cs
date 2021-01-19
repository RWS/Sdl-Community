using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.StudioViews.Services
{
	
	public class SdlxliffImporter
	{
		private readonly CommonService _commonService;
		private readonly FilterItemHelper _filterItemHelper;
		private readonly List<AnalysisBand> _analysisBands;

		public SdlxliffImporter(CommonService commonService, FilterItemHelper filterItemHelper, List<AnalysisBand> analysisBands)
		{
			_commonService = commonService;
			_filterItemHelper = filterItemHelper;
			_analysisBands = analysisBands;
		}
		
		public ImportResult UpdateFile( string importFilePath, List<string> excludeFilterIds, string filePathInput, string filePathOutput)
		{
			var updatedSegmentPairs = GetSegmentPairs(importFilePath);
			return UpdateFile(updatedSegmentPairs, excludeFilterIds, filePathInput, filePathOutput);
		}

		public ImportResult UpdateFile(List<SegmentPairInfo> updatedSegmentPairs, List<string> excludeFilterIds, string filePathInput, string filePathOutput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);

			var contentWriter = new ContentImporter(updatedSegmentPairs, excludeFilterIds, _filterItemHelper, _analysisBands);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return new ImportResult
			{
				Success = true,
				UpdatedSegments = contentWriter.UpdatedSegments,
				IgnoredSegments = contentWriter.IgnoredSegments,
				FilePath = filePathInput,
				UpdatedFilePath = filePathOutput,
				BackupFilePath = _commonService.GetUniqueFileName(filePathInput, "Backup")
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
