using System;
using System.Collections.Generic;
using System.IO;
using Multilingual.Excel.FileType.Providers.OpenXml.Model;

namespace Multilingual.Excel.FileType.Providers.PlaceholderPatterns
{
	public class Common
	{
		public bool SaveDefaultPlaceholderPatternsDocument(string path, bool overwrite = true)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new Exception("The file path cannot be null!");
			}

			var fileInfo = new FileInfo(path);
			if (fileInfo.Exists && !overwrite)
			{
				return false;
			}

			if (fileInfo.Exists)
			{
				fileInfo.Delete();
			}

			if (!Directory.Exists(fileInfo.DirectoryName))
			{
				Directory.CreateDirectory(fileInfo.DirectoryName ?? throw new InvalidOperationException("Invalid file path!"));
			}

			File.WriteAllBytes(path, PluginResources.DefaultPlaceHolders);

			return true;
		}

		public List<ExcelColumn> GetDefaultExcelColumns()
		{
			var excelColumns = new List<ExcelColumn>
			{
				new ExcelColumn
				{
					Name = "A",
					Text = Constants.ColumnNamePattern
				},
				new ExcelColumn
				{
					Name = "B",
					Text = Constants.ColumnNameSegmentationHint
				},
				new ExcelColumn
				{
					Name = "C",
					Text = Constants.ColumnNameDescription
				}
			};

			return excelColumns;
		}
	}
}
