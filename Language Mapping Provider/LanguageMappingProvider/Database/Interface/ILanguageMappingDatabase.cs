using LanguageMappingProvider.Extensions;
using Sdl.Core.Globalization;
using System.Collections.Generic;

namespace LanguageMappingProvider;

public interface ILanguageMappingDatabase
{
	int Count { get; }

	bool CanResetToDefaults { get; }

	/// <summary>
	/// Adds a new <see cref="LanguageMapping"/> object to the database.
	/// </summary>
	/// <param name="mappedLanguage">The <see cref="LanguageMapping"/> object to be inserted into the database.</param>
	void InsertLanguage(LanguageMapping mappedLanguage);

	/// <summary>
	/// Retrieves a <see cref="LanguageMapping"/> object based on the provided language code.
	/// </summary>
	/// <param name="languageCode">The language code in the format "xx-XX" representing the desired language.</param>
	/// <returns>A <see cref="LanguageMapping"/> object containing the retrieved language data.</returns>
	/// <exception cref="LanguageNotFoundException">Thrown if the requested language is not found.</exception>
	LanguageMapping GetLanguage(string languageCode);

	/// <summary>
	/// Retrieves a <see cref="LanguageMapping"/> object based on the provided <see cref="CultureCode"/>.
	/// </summary>
	/// <param name="cultureCode">The <see cref="CultureCode"/> representing the desired language.</param>
	/// <returns>A <see cref="LanguageMapping"/> object containing the retrieved language data.</returns>
	/// <exception cref="LanguageNotFoundException">Thrown if the requested language is not found.</exception>
	LanguageMapping GetLanguage(CultureCode cultureCode);

	/// <summary>
	/// Attempts to retrieve a <see cref="LanguageMapping"/> object based on a language code in format "xx-XX" and stores the result in the <paramref name="languageMapping"/> parameter.
	/// </summary>
	/// <param name="languageCode">The language code in the format "xx-XX" representing the desired language.</param>
	/// <param name="languageMapping">An <see cref="LanguageMapping"/> object to store the retrieved language data.</param>
	/// <returns><c>True</c> if the data was successfully retrieved, <c>False</c> otherwise.</returns>
	bool TryGetLanguage(string languageCode, out LanguageMapping languageMapping);

	/// <summary>
	/// Tries to obtain a <see cref="LanguageMapping"/> object using the provided <see cref="CultureCode"/> and saves the result in the <paramref name="languageMapping"/> parameter.
	/// </summary>
	/// <param name="cultureCode">The culture code in the format "xx-XX" representing the desired culture and language.</param>
	/// <param name="languageMapping">An <see cref="LanguageMapping"/> object to store the retrieved language data.</param>
	/// <returns><c>true</c> if the data was successfully retrieved, <c>false</c> otherwise.</returns>
	bool TryGetLanguage(CultureCode cultureCode, out LanguageMapping languageMapping);

	/// <summary>
	/// This function is responsible for updating all the 
	/// modified values of mapped languages in the database. 
	/// It takes an enumerable collection of <see cref="LanguageMapping"/> objects as input, 
	/// which contains the updated language values, 
	/// and updates the corresponding records in the database.
	/// </summary>
	/// <param name="mappedLanguages">An enumerable collection of <see cref="LanguageMapping"/> objects that represent the modified language values to be updated in the database.</param>
	void UpdateAll(IEnumerable<LanguageMapping> mappedLanguages);

	/// <summary>
	/// The updateAt function updates a specific field of a language record in the database at the given index.
	/// It takes the index, field name, and new value as parameters and applies the modification to the corresponding field of the language record.
	/// </summary>
	/// <param name="index">The index of the language record to be updated in the database.</param>
	/// <param name="field">The name of the field to be modified in the language record.</param>
	/// <param name="value">The new value to be assigned to the specified field.</param>
	void UpdateAt(int index, string field, string value);

	/// <summary>
	/// This function compares an IEnumerable of <see cref="LanguageMapping"/> objects to the existing data in the database
	/// and determines if any changes have been made.
	/// </summary>
	/// <param name="mappedLanguages">An enumerable collection of <see cref="LanguageMapping"/> objects representing the languages to be checked for changes.</param>
	/// <returns>It returns a boolean value indicating whether the IEnumerable is different from the data stored in the database.</returns>
	bool HasMappedLanguagesChanged(IEnumerable<LanguageMapping> mappedLanguages);

	/// <summary>
	/// Retrieves language data from the database and returns it as an IEnumerable of <see cref="LanguageMapping"/> objects.
	/// </summary>
	/// <returns>
	/// An IEnumerable of <see cref="LanguageMapping"/> objects representing the language data from the database.
	/// </returns>
	IEnumerable<LanguageMapping> GetMappedLanguages();

	/// <summary>
	/// This function sets all the values in the database to their default values.
	/// It retrieves the default values from the default database and applies them to overwrite the existing values in the current database.
	/// </summary>
	void ResetToDefault();
}