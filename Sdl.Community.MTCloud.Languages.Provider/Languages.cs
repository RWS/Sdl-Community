using System.Collections.Generic;
using Sdl.Community.MTCloud.Languages.Provider.Implementation;
using Sdl.Community.MTCloud.Languages.Provider.Model;

namespace Sdl.Community.MTCloud.Languages.Provider
{
	public class Languages
	{					
		/// <summary>
		/// Saves the MT Clound Languages to the default location in the users
		/// roaming directory.
		/// </summary>
		/// <param name="languages">list of MT Cloud Languages</param>
		/// <returns>Returns true if file was saved correctly</returns>
		public bool SaveLanguages(List<MTCloudLanguage> languages)
		{
			return SaveLanguages(languages, Constants.MTLanguageCodesFilePath);
		}

		/// <summary>
		/// Saves the MT Clound Languages to the <param name="path">file path</param>
		/// </summary>
		/// <param name="languages">list of MT Cloud Languages</param>
		/// <param name="path">The full path to the excel file where the languages are saved</param>
		/// <returns>Returns true if file was saved correctly</returns>
		public bool SaveLanguages(List<MTCloudLanguage> languages, string path)
		{			
			var writer = new Writer();			
			return writer.WriteLanguages(languages, path);
		}

		/// <summary>
		/// Reads the MT Cloud Languages from the default excel file from the users
		/// roaming directory.
		/// </summary>
		/// <returns>Returns a list of MT Cloud Languages</returns>
		public List<MTCloudLanguage> GetLanguages()
		{
			return GetLanguages(Constants.MTLanguageCodesFilePath);
		}

		/// <summary>
		/// Reads the MT Cloud Languages from the <param name="path">file path</param>
		/// </summary>
		/// <param name="path">The full path to the excel file where the languages are located</param>
		/// <returns>Returns a list of MT Cloud Languages</returns>
		public List<MTCloudLanguage> GetLanguages(string path)
		{
			var reader = new Reader();
			var languages = GetLanguages(reader.ReadLanguages(path));
			return languages;
		}

		private static List<MTCloudLanguage> GetLanguages(IEnumerable<ExcelRow> excelRows)
		{
			var languages = new List<MTCloudLanguage>();
			foreach (var row in excelRows)
			{
				var language = new MTCloudLanguage
				{
					Index = row.Index
				};

				foreach (var cell in row.Cells)
				{
					switch (cell.Column.Index)
					{
						case 0:
							language.Language = cell.Value;
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
