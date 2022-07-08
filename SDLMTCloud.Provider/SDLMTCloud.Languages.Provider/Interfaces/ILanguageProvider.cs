using System.Collections.Generic;
using Sdl.Community.MTCloud.Languages.Provider.Model;

namespace Sdl.Community.MTCloud.Languages.Provider.Interfaces
{
	public interface ILanguageProvider
	{
		/// <summary>
		/// Saves the MT Clound Languages to the default location in the users
		/// roaming directory.
		/// </summary>
		/// <param name="mappedLanguages">list of MT Cloud Languages</param>
		/// <returns>Returns true if file was saved correctly</returns>
		bool SaveMappedLanguages(List<MappedLanguage> mappedLanguages);

		/// <summary>
		/// Saves the MT Clound Languages to the <param name="path">file path</param>
		/// </summary>
		/// <param name="mappedLanguages">list of MT Cloud Languages</param>
		/// <param name="path">The full path to the excel file where the languages are saved</param>
		/// <returns>Returns true if file was saved correctly</returns>
		bool SaveMappedLanguages(List<MappedLanguage> mappedLanguages, string path);

		/// <summary>
		/// Reads the MT Cloud Languages from the excel file in the users roaming directory.
		/// If the Excel file is empty, the default languages are recovered automatically.
		/// </summary>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of MT Cloud Languages</returns>
		List<MappedLanguage> GetMappedLanguages(bool reset = false);

		/// <summary>
		/// Reads the MT Cloud Languages from the <param name="path">file path</param>
		/// </summary>
		/// <param name="path">The full path to the excel file where the languages are located</param>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of MT Cloud Languages</returns>
		List<MappedLanguage> GetMappedLanguages(string path, bool reset = false);
	}
}
