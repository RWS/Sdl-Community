using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.FileTypeSupport.Framework.IntegrationApi;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class SdlxliffWriter
	{
		private readonly SegmentBuilder _segmentBuilder;
		private readonly IFileTypeManager _fileTypeManager;
		private readonly ImportOptions _importOptions;
		private readonly List<AnalysisBand> _analysisBands;

		public SdlxliffWriter(IFileTypeManager fileTypeManager, SegmentBuilder segmentBuilder,
			ImportOptions importOptions, List<AnalysisBand> analysisBands)
		{
			_fileTypeManager = fileTypeManager;
			_segmentBuilder = segmentBuilder;
			_importOptions = importOptions;
			_analysisBands = analysisBands;
			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; private set; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; private set; }

		public bool UpdateFile(Xliff xliff, string filePathInput, string filePathOutput)
		{
			_segmentBuilder.ExistingTagIds = GetTagIds(filePathInput);

			var converter = _fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);
			var contentWriter = new ContentWriter(xliff, _segmentBuilder, _importOptions, _analysisBands);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			ConfirmationStatistics = contentWriter.ConfirmationStatistics;
			TranslationOriginStatistics = contentWriter.TranslationOriginStatistics;

			return true;
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
