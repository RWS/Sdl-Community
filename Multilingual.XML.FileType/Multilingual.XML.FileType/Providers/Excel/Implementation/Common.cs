using System;
using System.Collections.Generic;
using System.IO;
using Multilingual.XML.FileType.Providers.Excel.Model;

namespace Multilingual.XML.FileType.Providers.Excel.Implementation
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
					Index = 0,
					Name = Constants.ColumnNamePattern,
					Width = 30
				},
				new ExcelColumn
				{
					Index = 1,
					Name = Constants.ColumnNameSegmentationHint,
					Width = 30
				},
				new ExcelColumn
				{
					Index = 2,
					Name = Constants.ColumnNameDescription,
					Width = 100
				}
			};

			return excelColumns;
		}
	}
}
