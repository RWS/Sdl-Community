using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Implementation;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Interfaces;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Model;

namespace Sdl.Community.XLIFF.Manager.LanguageMapping
{
	public class LanguageProvider: ILanguageProvider
	{
		private readonly PathInfo _pathInfo;

		public LanguageProvider(PathInfo pathInfo)
		{
			_pathInfo = pathInfo;
		}

		/// <summary>
		/// Saves the Languages to the default location in the users
		/// roaming directory.
		/// </summary>
		/// <param name="mappedLanguages">list of Languages</param>
		/// <returns>Returns true if file was saved correctly</returns>
		public bool SaveMappedLanguages(List<MappedLanguage> mappedLanguages)
		{
			return SaveMappedLanguages(mappedLanguages, _pathInfo.LanguageMappingsFilePath);
		}

		/// <summary>
		/// Saves the Languages to the <param name="path">file path</param>
		/// </summary>
		/// <param name="mappedLanguages">list of Languages</param>
		/// <param name="path">The full path to the excel file where the languages are saved</param>
		/// <returns>Returns true if file was saved correctly</returns>
		public bool SaveMappedLanguages(List<MappedLanguage> mappedLanguages, string path)
		{			
			var writer = new Writer();			
			return writer.WriteLanguages(mappedLanguages, path);
		}

		/// <summary>
		/// Reads the Languages from the excel file in the users roaming directory.
		/// If the Excel file is empty, the default languages are recovered automatically.
		/// </summary>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of Languages</returns>
		public List<MappedLanguage> GetMappedLanguages(bool reset = false)
		{
			return GetMappedLanguages(_pathInfo.LanguageMappingsFilePath, reset);
		}


		/// <summary>
		/// Reads the Languages from the <param name="path">file path</param>
		/// </summary>
		/// <param name="path">The full path to the excel file where the languages are located</param>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of Languages</returns>
		public List<MappedLanguage> GetMappedLanguages(string path, bool reset = false)
		{
			var common = new Implementation.Common();
			if (reset)
			{
				common.SaveDefaultLanguagesDocument(path);
			}

			var reader = new Reader(common);
			var languages = GetLanguages(reader.ReadLanguages(path));
			return languages;
		}

		private static List<MappedLanguage> GetLanguages(IEnumerable<ExcelRow> excelRows)
		{
			var languages = new List<MappedLanguage>();
			foreach (var row in excelRows)
			{
				var language = new MappedLanguage
				{
					Index = row.Index
				};

				foreach (var cell in row.Cells)
				{
					switch (cell.Column.Index)
					{
						case 0:
							language.LanguageCode = cell.Value;
							break;
						case 1:
							language.LanguageName = cell.Value;
							break;
						case 2:
							language.LanguageRegion = cell.Value;
							break;
						case 3:
							language.MappedCode = cell.Value;
							break;
						case 4:
							language.CustomDisplayName = cell.Value;
							break;						
					}
				}

				languages.Add(language);
			}

			return languages;
		}
	}
}
