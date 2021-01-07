using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace StudioViews.Services
{
	public class SdlxliffImporter
	{
		public bool UpdateFile(string importFilePath, string filePathInput, string filePathOutput)
		{
			var updatedParagraphUnits = GetUpdatedParagraphUnits(importFilePath);

			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, filePathOutput, null);
			
			var contentWriter = new ContentImporter(updatedParagraphUnits);

			converter.AddBilingualProcessor(contentWriter);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return true;
		}

		private List<IParagraphUnit> GetUpdatedParagraphUnits(string filePathInput)
		{
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);
			var converter = fileTypeManager.GetConverterToDefaultBilingual(filePathInput, null, null);

			var contentReader = new ContentReader();

			converter.AddBilingualProcessor(contentReader);
			converter.SynchronizeDocumentProperties();

			converter.Parse();

			return contentReader.ParagraphUnits;
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
