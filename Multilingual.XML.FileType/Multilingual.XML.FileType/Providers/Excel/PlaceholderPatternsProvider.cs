using System;
using System.Collections.Generic;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Providers.Excel.Implementation;
using Multilingual.XML.FileType.Providers.Excel.Interfaces;
using Multilingual.XML.FileType.Providers.Excel.Model;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.Providers.Excel
{
	public class PlaceholderPatternsProvider : IPlaceholderPatternsProvider
	{
		public List<PlaceholderPattern> GetPlaceholderPatterns(string path, bool reset = false)
		{
			var common = new Implementation.Common();
			if (reset)
			{
				common.SaveDefaultPlaceholderPatternsDocument(path);
			}

			var reader = new Reader(common);
			var patterns = GetPlaceholderPatterns(reader.ReadPlaceholderPatterns(path));
			return patterns;
		}

		public bool SavePlaceholderPatterns(List<PlaceholderPattern> placeholderPatterns, string path)
		{
			var writer = new Writer();
			return writer.WritePlaceholderPatterns(placeholderPatterns, path);
		}

		private static List<PlaceholderPattern> GetPlaceholderPatterns(IEnumerable<ExcelRow> excelRows)
		{
			var patterns = new List<PlaceholderPattern>();
			foreach (var row in excelRows)
			{
				var pattern = new PlaceholderPattern
				{
					Order = row.Index
				};

				foreach (var cell in row.Cells)
				{
					switch (cell.Column.Index)
					{
						case 0:
							pattern.Pattern = cell.Value;
							break;
						case 1:
							if (string.IsNullOrEmpty(cell.Value))
							{
								pattern.SegmentationHint = SegmentationHint.MayExclude;
							}
							else
							{
								var success =
									Enum.TryParse<SegmentationHint>(cell.Value, true, out var segmentationHint);
								pattern.SegmentationHint = success ? segmentationHint : SegmentationHint.MayExclude;
							}

							break;
						case 2:
							pattern.Description = cell.Value;
							break;
					}
				}

				patterns.Add(pattern);
			}

			return patterns;
		}
	}
}