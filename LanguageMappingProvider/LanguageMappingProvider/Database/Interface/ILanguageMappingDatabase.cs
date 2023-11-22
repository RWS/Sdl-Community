using System.Collections.Generic;
using LanguageMappingProvider.Model;

namespace LanguageMappingProvider.Database.Interface
{
	public interface ILanguageMappingDatabase
    {
        /// <summary>
        /// This function is responsible for adding a new MappedLanguage object to the database.
        /// It accepts a MappedLanguage object as input,
        /// representing the language to be inserted,
        /// and inserts the corresponding data into the appropriate table in the database.
        /// </summary>
        /// <param name="mappedLanguage">A MappedLanguage object representing the language to be inserted into the database.</param>
        void InsertLanguage(LanguageMapping mappedLanguage);

        /// <summary>
        /// This function is responsible for updating all the 
        /// modified values of mapped languages in the database. 
        /// It takes an enumerable collection of MappedLanguage objects as input, 
        /// which contains the updated language values, 
        /// and updates the corresponding records in the database.
        /// </summary>
        /// <param name="mappedLanguages">An enumerable collection of MappedLanguage objects that represent the modified language values to be updated in the database.</param>
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
        /// This function compares an IEnumerable of MappedLanguage objects to the existing data in the database
        /// and determines if any changes have been made.
        /// </summary>
        /// <param name="mappedLanguages">An enumerable collection of MappedLanguage objects representing the languages to be checked for changes.</param>
        /// <returns>It returns a boolean value indicating whether the IEnumerable is different from the data stored in the database.</returns>
        bool HasMappedLanguagesChanged(IEnumerable<LanguageMapping> mappedLanguages);

        /// <summary>
        /// Retrieves language data from the database and returns it as an IEnumerable of MappedLanguage objects.
        /// </summary>
        /// <returns>
        /// An IEnumerable of MappedLanguage objects representing the language data from the database.
        /// </returns>
        IEnumerable<LanguageMapping> GetMappedLanguages();

        /// <summary>
        /// This function sets all the values in the database to their default values.
        /// It retrieves the default values from the default database and applies them to overwrite the existing values in the current database.
        /// </summary>
        void ResetToDefault();
    }
}