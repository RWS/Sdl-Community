using System.Collections.Generic;
using Sdl.Community.MTCloud.Languages.Provider.Implementation;
using Sdl.Community.MTCloud.Languages.Provider.Interfaces;
using Sdl.Community.MTCloud.Languages.Provider.Model;

namespace Sdl.Community.MTCloud.Languages.Provider
{
	public class LanguageProvider: ILanguageProvider
	{					
		/// <summary>
		/// Saves the MT Clound Languages to the default location in the users
		/// roaming directory.
		/// </summary>
		/// <param name="languages">list of MT Cloud Languages</param>
		/// <returns>Returns true if file was saved correctly</returns>
		public bool SaveLanguages(List<Language> languages)
		{
			return SaveLanguages(languages, Constants.MTLanguageCodesFilePath);
		}

		/// <summary>
		/// Saves the MT Clound Languages to the <param name="path">file path</param>
		/// </summary>
		/// <param name="languages">list of MT Cloud Languages</param>
		/// <param name="path">The full path to the excel file where the languages are saved</param>
		/// <returns>Returns true if file was saved correctly</returns>
		public bool SaveLanguages(List<Language> languages, string path)
		{			
			var writer = new Writer();			
			return writer.WriteLanguages(languages, path);
		}

		/// <summary>
		/// Reads the MT Cloud Languages from the excel file in the users roaming directory.
		/// If the Excel file is empty, the default languages are recovered automatically.
		/// </summary>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of MT Cloud Languages</returns>
		public List<Language> GetLanguages(bool reset = false)
		{
			return GetLanguages(Constants.MTLanguageCodesFilePath, reset);
		}


		/// <summary>
		/// Reads the MT Cloud Languages from the <param name="path">file path</param>
		/// </summary>
		/// <param name="path">The full path to the excel file where the languages are located</param>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of MT Cloud Languages</returns>
		public List<Language> GetLanguages(string path, bool reset = false)
		{
			var common = new Common();
			if (reset)
			{
				common.SaveDefaultLanguagesDocument(path);
			}

			var reader = new Reader(common);
			var languages = GetLanguages(reader.ReadLanguages(path));
			return languages;
		}

		private static List<Language> GetLanguages(IEnumerable<ExcelRow> excelRows)
		{
			var languages = new List<Language>();
			foreach (var row in excelRows)
			{
				var language = new Language
				{
					Index = row.Index
				};

				foreach (var cell in row.Cells)
				{
					switch (cell.Column.Index)
					{
						case 0:
							language.Name = cell.Value;
							break;
						case 1:
							language.Region = cell.Value;
							break;
						case 2:
							language.TradosCode = cell.Value;
							break;
						case 3:
							language.MTCode = cell.Value;
							break;
						case 4:
							language.MTCodeLocale = cell.Value;
							break;
					}
				}

				languages.Add(language);
			}

			return languages;
		}
	}
}
