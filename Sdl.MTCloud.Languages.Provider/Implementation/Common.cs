using System;
using System.Collections.Generic;
using System.IO;
using Sdl.Community.MTCloud.Languages.Provider.Model;

namespace Sdl.Community.MTCloud.Languages.Provider.Implementation
{
	public class Common
	{
		/// <summary>
		/// Save the default langauges document in the <param name="path">file path</param>
		/// 
		/// 1. If the languages <param name="path">file path</param> doesen't exist, then 
		///    it will create/save a new document with the default languages
		/// 2. If the languages <param name="path">file path</param> exists, then it will  
		///    overwrite it if <param name="overwrite">overwrite</param> is set to true
		/// </summary>
		/// <param name="path">The full path to the excel filee</param>
		/// <param name="overwrite">If true and the file exists, it will be overwritten</param>
		/// <returns>True if the file was saved</returns>
		public bool SaveDefaultLanguagesDocument(string path, bool overwrite = true)
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

			File.WriteAllBytes(path, Resources.DefaultMTLanguageCodes);

			return true;
		}

		/// <summary>
		/// Gets a list of the default columns used in the MTCloud Languages excel document
		/// </summary>
		public List<ExcelColumn> GetDefaultExcelColumns()
		{
			var excelColumns = new List<ExcelColumn>
			{
				new ExcelColumn
				{
					Index = 0,
					Name = Constants.ColumnNameLanguage,
					Width = 30
				},
				new ExcelColumn
				{
					Index = 1,
					Name = Constants.ColumnNameRegion,
					Width = 30
				},
				new ExcelColumn
				{
					Index = 2,
					Name = Constants.ColumnNameTradosCode,
					Width = 20
				},
				new ExcelColumn
				{
					Index = 3,
					Name = Constants.ColumnNameMTCode,
					Width = 20
				},
				new ExcelColumn
				{
					Index = 4,
					Name = Constants.ColumnNameMTCodeLocale,
					Width = 20
				}
			};

			return excelColumns;
		}
	}
}
