using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using LanguageMappingProvider.Database.Interface;
using LanguageMappingProvider.Extensions;
using LanguageMappingProvider.Model;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;

namespace LanguageMappingProvider.Database
{
	public class LanguageMappingDatabase : ILanguageMappingDatabase, IDisposable
	{
		private readonly string _filePath;
		private readonly SQLiteConnection _sqliteConnection;
		private readonly IList<LanguageMapping> _pluginSupportedLanguages;
		private readonly IDictionary<int, LanguageMapping> _mappedLanguagesDictionary;

		/// <summary>
		/// Initializes a new instance of the <see cref="LanguageMappingDatabase"/> class
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
		/// A collection of <see cref="LanguageMapping"/> objects representing the supported languages for the plugin.
		/// The <see cref="LanguageMapping.LanguageCode"/> values in the database will be updated accordingly when creating or resetting the database.
		/// This ensures seamless integration with the plugin's functionality, reflecting the accurate supported languages in the database.
		/// </param>
		/// 
		/// <exception cref="DatabaseInitializationException">Thrown when the <paramref name="pluginSupportedLanguages"/> is not set and the database doesn't exist.</exception>
		public LanguageMappingDatabase(string pluginName, IList<LanguageMapping> pluginSupportedLanguages)
		{
			_pluginSupportedLanguages = pluginSupportedLanguages;
			_filePath = string.Format(Constants.DatabaseFilePath, pluginName);
			_mappedLanguagesDictionary = new Dictionary<int, LanguageMapping>();
			_sqliteConnection = new SQLiteConnection($"Data Source={_filePath}");
			EnsureDatabaseFileExists();
			EnsureTableExists();
			LoadMappedLanguages();
		}

		public int Count => _mappedLanguagesDictionary.Count;

		public bool CanResetToDefaults => _pluginSupportedLanguages is not null && _pluginSupportedLanguages.Any();

		public void InsertLanguage(LanguageMapping mappedLanguage)
		{
			EnsureMappedLanguageIsValid(mappedLanguage);
			var syntax = string.Format(Constants.SQL_InsertData, mappedLanguage.Name, mappedLanguage.Region, mappedLanguage.TradosCode, mappedLanguage.LanguageCode);
			ExecuteCommand(syntax);
			LoadMappedLanguages();
		}

		public LanguageMapping GetLanguage(string languageCode)
		{
			var foundLanguage = _mappedLanguagesDictionary?.Values?.FirstOrDefault(x => x.TradosCode.Equals(languageCode));
			return foundLanguage is not null
				 ? foundLanguage
				 : throw new LanguageNotFoundException();
		}

		public LanguageMapping GetLanguage(CultureCode cultureCode)
		{
			return GetLanguage(cultureCode.Name);
		}

		public bool TryGetLanguage(string languageCode, out LanguageMapping languageMapping)
		{
			languageMapping = null;
			try
			{
				languageMapping = GetLanguage(languageCode);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool TryGetLanguage(CultureCode cultureCode, out LanguageMapping languageMapping)
		{
			return TryGetLanguage(cultureCode.Name, out languageMapping);
		}

		public void UpdateAll(IEnumerable<LanguageMapping> mappedLanguages)
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
			var syntax = string.Format(Constants.SQL_UpdateData, field, value, index);
			ExecuteCommand(syntax);
			LoadMappedLanguages();
		}

		public bool HasMappedLanguagesChanged(IEnumerable<LanguageMapping> mappedLanguages)
		{
			if (mappedLanguages is null || !mappedLanguages.Any())
			{
				return false;
			}

			EnsureCollectionIsValid(mappedLanguages);
			var mappedLanguagesDictionary = mappedLanguages.ToDictionary(language => language.Index, language => language);
			return HasMappedLanguagesChanged(mappedLanguagesDictionary);
		}

		public IEnumerable<LanguageMapping> GetMappedLanguages()
		{
			return _mappedLanguagesDictionary.Values.Select(mappedLanguage => new LanguageMapping
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
			if (!CanResetToDefaults)
			{
				return;
			}

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
			var databaseCollection = connection.Query<LanguageMapping>(Constants.SQL_SelectData, new DynamicParameters());

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
			if (File.Exists(_filePath))
			{
				return;
			}

			if (!Directory.Exists(_filePath))
			{
				Directory.CreateDirectory(Constants.PluginAppDataLocation);
			}

			EnsurePluginSupportedLanguagesAreValid(_pluginSupportedLanguages);
			SQLiteConnection.CreateFile(_filePath);
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

		private static IList<LanguageMapping> GetTradosLanguages()
		{
			var languages = LanguageRegistryApi.Instance.GetAllLanguages();
			var mappedLanguages = new List<LanguageMapping>();
			foreach (var language in languages)
			{
				if (string.IsNullOrEmpty(language.DisplayName)
				 || language.DefaultSpecificLanguageCode is not null
				 || language.DisplayName.Contains("deprecated"))
				{
					continue;
				}

				var regex = new Regex(@"^(.*?)\s*(?:\((.*?)\))?$");
				var match = regex.Match(language.DisplayName);

				mappedLanguages.Add(new LanguageMapping
				{
					Name = match.Groups[1].Value,
					Region = match.Groups[2].Success ? match.Groups[2].Value : null,
					TradosCode = language.CultureInfo.Name
				});
			}

			return mappedLanguages.OrderBy(x => x.Name).ThenBy(x => x.Region).ToList();
		}

		private void UpdateMappingCodes(IEnumerable<LanguageMapping> mappingList)
		{
			var mappingDictionary = _pluginSupportedLanguages?.ToDictionary(l => (l?.Name, l?.Region), l => l?.LanguageCode);
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

		private void InsertCollection(IEnumerable<LanguageMapping> mappedLanguages)
		{
			if (mappedLanguages is null || !mappedLanguages.Any())
			{
				return;
			}

			var syntax = GenerateInsertSyntax(mappedLanguages);
			ExecuteCommand(syntax);
		}

		private static string GenerateInsertSyntax(IEnumerable<LanguageMapping> collection)
		{
			var syntaxBuilder = new StringBuilder();
			syntaxBuilder.AppendLine(Constants.SQL_InsertData_StringBuilder);

			foreach (var item in collection)
			{
				syntaxBuilder.AppendLine($"(\"{item.Name}\", \"{item.Region}\", \"{item.TradosCode}\", \"{item.LanguageCode}\"),");
			}

			syntaxBuilder.Length -= 3;
			syntaxBuilder.AppendLine(";");
			return syntaxBuilder.ToString();
		}

		private void UpdateAll(IDictionary<int, LanguageMapping> mappedLanguagesDictionary)
		{
			foreach (var mappedLanguage in mappedLanguagesDictionary)
			{
				EnsureMappedLanguageIsValid(mappedLanguage.Value);
				var index = mappedLanguage.Key;
				var currentPair = mappedLanguage.Value;

				if (!_mappedLanguagesDictionary.TryGetValue(index, out var originalPair)
				 || !string.Equals(currentPair.LanguageCode, originalPair.LanguageCode))
				{
					UpdateAt(index, nameof(currentPair.LanguageCode), currentPair.LanguageCode);
				}
			}
		}

		private bool HasMappedLanguagesChanged(IDictionary<int, LanguageMapping> mappedLanguagesDictionary)
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

		private static void EnsurePluginSupportedLanguagesAreValid(IEnumerable<LanguageMapping> pluginSupportedLanguages)
		{
			if (pluginSupportedLanguages is null
			 || !pluginSupportedLanguages.Any())
			{
				throw new DatabaseInitializationException();
			}
		}

		private static void EnsureMappedLanguageIsValid(LanguageMapping mappedLanguage)
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

		private static void EnsureCollectionIsValid(IEnumerable<LanguageMapping> collection)
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