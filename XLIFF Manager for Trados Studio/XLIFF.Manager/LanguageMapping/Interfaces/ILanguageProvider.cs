using System.Collections.Generic;
using Sdl.Community.XLIFF.Manager.LanguageMapping.Model;

namespace Sdl.Community.XLIFF.Manager.LanguageMapping.Interfaces
{
	public interface ILanguageProvider
	{
		/// <summary>
		/// Saves the Languages to the default location in the users
		/// roaming directory.
		/// </summary>
		/// <param name="mappedLanguages">list of Languages</param>
		/// <returns>Returns true if file was saved correctly</returns>
		bool SaveMappedLanguages(List<MappedLanguage> mappedLanguages);

		/// <summary>
		/// Saves the Languages to the <param name="path">file path</param>
		/// </summary>
		/// <param name="mappedLanguages">list of Languages</param>
		/// <param name="path">The full path to the excel file where the languages are saved</param>
		/// <returns>Returns true if file was saved correctly</returns>
		bool SaveMappedLanguages(List<MappedLanguage> mappedLanguages, string path);

		/// <summary>
		/// Reads the Languages from the excel file in the users roaming directory.
		/// If the Excel file is empty, the default languages are recovered automatically.
		/// </summary>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of Languages</returns>
		List<MappedLanguage> GetMappedLanguages(bool reset = false);

		/// <summary>
		/// Reads the Languages from the <param name="path">file path</param>
		/// </summary>
		/// <param name="path">The full path to the excel file where the languages are located</param>
		/// <param name="reset">Resets the default languages</param>
		/// <returns>Returns a list of Languages</returns>
		List<MappedLanguage> GetMappedLanguages(string path, bool reset = false);
	}
}
