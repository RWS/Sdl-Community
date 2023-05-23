using Dapper;
using LanguageMappingProvider.Database.Interface;
using LanguageMappingProvider.Model;
using LanguageMappingProvider.Extensions;
using Sdl.Core.Globalization.LanguageRegistry;
using System.Data;
using System.Data.SQLite;
using System.Text;

namespace LanguageMappingProvider.Database
{
    public class LanguageMappingDatabase : ILanguageMappingDatabase, IDisposable
    {
        private readonly string _filePath;
        private readonly SQLiteConnection _sqliteConnection;
        private readonly IList<MappedLanguage> _pluginSupportedLanguages;
        private readonly IDictionary<int, MappedLanguage> _mappedLanguagesDictionary;

        /// <summary>
        /// Initializes a new instance of the SQLiteDatabase class
        /// and establishes a connection to the SQLite database associated with the specified plugin.
        /// </summary>
        /// 
        /// <param name="pluginName">
        /// The unique identifier of the plugin.
        /// This parameter is essential for establishing a secure and reliable connection to the dedicated database, 
        /// ensuring seamless integration with the plugin's functionality. 
        /// If the corresponding database does not exist, a new database will be created.</param>
        /// 
        /// <param name="pluginSupportedLanguages">
        /// A collection of `MappedLanguage` objects representing the supported languages for the plugin.
        /// The `LanguageCode` values in the database will be updated accordingly when creating or resetting the database.
        /// This ensures seamless integration with the plugin's functionality, reflecting the accurate supported languages in the database.
        /// </param>
        /// 
        /// <exception cref="DatabaseInitializationException">Thrown when the pluginSupportedLanguages is not set and the database doesn't exist.</exception>
        public LanguageMappingDatabase(string pluginName, IList<MappedLanguage> pluginSupportedLanguages)
        {
            _pluginSupportedLanguages = pluginSupportedLanguages;
            _filePath = string.Format(Constants.DatabaseFilePath, pluginName);
            _mappedLanguagesDictionary = new Dictionary<int, MappedLanguage>();
            _sqliteConnection = new SQLiteConnection($"Data Source={_filePath}");
            EnsureDatabaseFileExists();
            EnsureTableExists();
            LoadMappedLanguages();
        }

        public void InsertLanguage(MappedLanguage mappedLanguage)
        {
            EnsureMappedLanguageIsValid(mappedLanguage);
            var syntax = string.Format(Constants.SQL_InsertData, mappedLanguage.Name, mappedLanguage.Region, mappedLanguage.TradosCode, mappedLanguage.LanguageCode);
            ExecuteCommand(syntax);
            LoadMappedLanguages();
        }

        public void UpdateAll(IEnumerable<MappedLanguage> mappedLanguages)
        {
            if (mappedLanguages is null || !mappedLanguages.Any())
            {
                return;
            }

            var mappedLanguagesDictionary = mappedLanguages?.ToDictionary(language => language.Index, language => language);
            UpdateAll(mappedLanguagesDictionary);
        }

        public void UpdateAt(int index, string field, string value)
        {
            EnsureCanUpdate(index, field, value);
            string syntax = string.Format(Constants.SQL_UpdateData, field, value, index);
            ExecuteCommand(syntax);
            LoadMappedLanguages();
        }

        public bool HasMappedLanguagesChanged(IEnumerable<MappedLanguage> mappedLanguages)
        {
            if (mappedLanguages is null || !mappedLanguages.Any())
            {
                return false;
            }

            EnsureCollectionIsValid(mappedLanguages);
            var mappedLanguagesDictionary = mappedLanguages.ToDictionary(language => language.Index, language => language);
            return HasMappedLanguagesChanged(mappedLanguagesDictionary);
        }

        public IEnumerable<MappedLanguage> GetMappedLanguages()
        {
            return _mappedLanguagesDictionary.Values.Select(mappedLanguage => new MappedLanguage
            {
                Index = mappedLanguage.Index,
                Name = mappedLanguage.Name,
                Region = mappedLanguage.Region,
                TradosCode = mappedLanguage.TradosCode,
                LanguageCode = mappedLanguage.LanguageCode
            });
        }

        public void ResetToDefault()
        {
            ExecuteCommand(Constants.SQL_DropTable);
            CreateNewTable();
            LoadMappedLanguages();
        }

        public void Dispose()
        {
            _sqliteConnection.Dispose();
        }

        private void LoadMappedLanguages()
        {
            IDbConnection connection = _sqliteConnection;
            var databaseCollection = connection.Query<MappedLanguage>(Constants.SQL_SelectData, new DynamicParameters());

            _mappedLanguagesDictionary.Clear();
            foreach (var pair in databaseCollection)
            {
                _mappedLanguagesDictionary[pair.Index] = pair;
            }
        }

        private void ExecuteCommand(string syntax)
        {
            OpenConnection();
            var command = new SQLiteCommand(syntax, _sqliteConnection);
            command.ExecuteNonQuery();
            CloseConnection();
        }

        private void OpenConnection()
        {
            if (_sqliteConnection.State != ConnectionState.Open)
            {
                _sqliteConnection.Open();
            }
        }

        private void CloseConnection()
        {
            if (_sqliteConnection.State != ConnectionState.Closed)
            {
                _sqliteConnection.Close();
            }
        }

        private void EnsureDatabaseFileExists()
        {
            if (!File.Exists(_filePath))
            {
                if (!Directory.Exists(_filePath))
                {
                    Directory.CreateDirectory(Constants.PluginAppDataLocation);
                }

                EnsurePluginSupportedLanguagesAreValid(_pluginSupportedLanguages);
                SQLiteConnection.CreateFile(_filePath);
            }
        }

        private void EnsureTableExists()
        {
            OpenConnection();
            var command = new SQLiteCommand(Constants.SQL_TableExists, _sqliteConnection);
            var tableExists = command.ExecuteScalar() is not null;
            CloseConnection();

            if (!tableExists)
            {
                EnsurePluginSupportedLanguagesAreValid(_pluginSupportedLanguages);
                CreateNewTable();
            }
        }

        private void CreateNewTable()
        {
            ExecuteCommand(Constants.SQL_CreateTable);
            var codes = GetTradosLanguages();
            UpdateMappingCodes(codes);
            InsertCollection(codes);
        }

        private static IList<MappedLanguage> GetTradosLanguages()
        {
            var languages = LanguageRegistryApi.Instance.GetAllLanguages();
            var mappedLanguages = new List<MappedLanguage>();

            foreach (var language in languages)
            {
                if (string.IsNullOrEmpty(language.DisplayName)
                 || language.DefaultSpecificLanguageCode is not null)
                {
                    continue;
                }

                var languageName = language.DisplayName;
                var regionStartIndex = languageName.IndexOf('(') + 1;
                var regionEndIndex = languageName.Length - 1;

                mappedLanguages.Add(new MappedLanguage
                {
                    Name = languageName[..(regionStartIndex - 2)],
                    Region = languageName[regionStartIndex..regionEndIndex],
                    TradosCode = language.CultureInfo.Name
                });
            }

            return mappedLanguages;
        }

        public void UpdateMappingCodes(IEnumerable<MappedLanguage> mappingList)
        {
            var mappingDictionary = _pluginSupportedLanguages.ToDictionary(l => (l.Name, l.Region), l => l.LanguageCode);
            foreach (var mappedLanguage in mappingList)
            {
                if (mappingDictionary.TryGetValue((mappedLanguage.Name, mappedLanguage.Region), out var languageCode)
                 || mappingDictionary.TryGetValue((mappedLanguage.Name, null), out languageCode)
                 || mappingDictionary.TryGetValue((mappedLanguage.Name, string.Empty), out languageCode))
                {
                    mappedLanguage.LanguageCode = languageCode;
                }
            }
        }

        private void InsertCollection(IEnumerable<MappedLanguage> mappedLanguages)
        {
            if (mappedLanguages is null
             || !mappedLanguages.Any())
            {
                return;
            }

            var syntax = GenerateInsertSyntax(mappedLanguages);
            ExecuteCommand(syntax);
        }

        private static string GenerateInsertSyntax(IEnumerable<MappedLanguage> collection)
        {
            var syntaxBuilder = new StringBuilder();
            syntaxBuilder.AppendLine(Constants.SQL_InsertData_StringBuilder);

            foreach (MappedLanguage item in collection)
            {
                syntaxBuilder.AppendLine($"(\"{item.Name}\", \"{item.Region}\", \"{item.TradosCode}\", \"{item.LanguageCode}\"),");
            }

            syntaxBuilder.Length -= 3;
            syntaxBuilder.AppendLine(";");
            return syntaxBuilder.ToString();
        }

        private void UpdateAll(IDictionary<int, MappedLanguage> mappedLanguagesDictionary)
        {
            foreach (var mappedLanguage in mappedLanguagesDictionary)
            {
                EnsureMappedLanguageIsValid(mappedLanguage.Value);
                var index = mappedLanguage.Key;
                var currentPair = mappedLanguage.Value;

                if (!_mappedLanguagesDictionary.TryGetValue(index, out MappedLanguage originalPair)
                 || !string.Equals(currentPair.LanguageCode, originalPair.LanguageCode))
                {
                    UpdateAt(index, nameof(currentPair.LanguageCode), currentPair.LanguageCode);
                }
            }
        }

        private bool HasMappedLanguagesChanged(IDictionary<int, MappedLanguage> mappedLanguagesDictionary)
        {
            foreach (var pair in _mappedLanguagesDictionary)
            {
                if (!mappedLanguagesDictionary.TryGetValue(pair.Key, out var currentMappedLanguage)
                 || !string.Equals(currentMappedLanguage.Name, pair.Value.Name)
                 || !string.Equals(currentMappedLanguage.Region, pair.Value.Region)
                 || !string.Equals(currentMappedLanguage.TradosCode, pair.Value.TradosCode)
                 || !string.Equals(currentMappedLanguage.LanguageCode, pair.Value.LanguageCode))
                {
                    return true;
                }
            }

            return mappedLanguagesDictionary.Count != _mappedLanguagesDictionary.Count;
        }

        private static void EnsurePluginSupportedLanguagesAreValid(IEnumerable<MappedLanguage> pluginSupportedLanguages)
        {
            if (pluginSupportedLanguages is null
             || !pluginSupportedLanguages.Any())
            {
                throw new DatabaseInitializationException();
            }
        }

        private static void EnsureMappedLanguageIsValid(MappedLanguage mappedLanguage)
        {
            if (mappedLanguage is null)
            {
                throw new MappedLanguageNullException(nameof(mappedLanguage));
            }

            if (string.IsNullOrEmpty(mappedLanguage.Name))
            {
                throw new MappedLanguageValidationException("Mapped language property must be set.", nameof(mappedLanguage.Name));
            }

            if (string.IsNullOrEmpty(mappedLanguage.TradosCode))
            {
                throw new MappedLanguageValidationException("Mapped language property must be set.", nameof(mappedLanguage.TradosCode));
            }
        }

        private void EnsureCanUpdate(int index, string property, string value)
        {
            if (!_mappedLanguagesDictionary.TryGetValue(index, out _))
            {
                throw new MappedLanguageIndexOutOfRangeException();
            }

            if (string.IsNullOrEmpty(property))
            {
                throw new MappedLanguageValidationException("The property must be set.", nameof(property));
            }

            if (string.IsNullOrEmpty(value))
            {
                throw new MappedLanguageValidationException("The value must be set.", nameof(value));
            }
        }

        private static void EnsureCollectionIsValid(IEnumerable<MappedLanguage> collection)
        {
            var indexSet = new HashSet<int>();
            foreach (var mappedLanguage in collection)
            {
                if (indexSet.Contains(mappedLanguage.Index))
                {
                    throw new DuplicateIndexException();
                }

                indexSet.Add(mappedLanguage.Index);
            }
        }
    }
}