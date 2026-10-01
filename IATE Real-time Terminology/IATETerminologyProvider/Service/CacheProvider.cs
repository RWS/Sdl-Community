using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using System.Globalization;
using Sdl.ProjectAutomation.Core;
using TradosStudio.API.TranslationResources.Terminology;
using TradosStudio.API.TranslationResources.Terminology.Definition;


namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class CacheProvider : ICacheProvider
	{
		private readonly SqliteDatabaseProvider _databaseProvider;

		public CacheProvider(SqliteDatabaseProvider databaseProvider)
		{
			_databaseProvider = databaseProvider;
		}

		public bool Connect(IProject project)
		{
			_databaseProvider?.Connect(project);
			return IsDbConnected();
		}

		public IEnumerable<SearchCache> GetAllCachedResults()
		{
			if (IsDbConnected())
			{
				_databaseProvider?.Get();
			}

			return null;
		}

		public void AddSearchResults(SearchCache searchCache, List<SearchResultModel> searchResults)
		{
			if (searchResults == null)
			{
				return;
			}

			var serializedSearchResult = SerializeSearchResult(searchResults);
			if (string.IsNullOrEmpty(serializedSearchResult))
			{
				return;
			}

			searchCache.SearchResultsString = serializedSearchResult;

			_databaseProvider?.Insert(searchCache);
		}

		public void ClearCachedResults()
		{
			_databaseProvider?.RemoveAll();
		}

		public bool IsDbConnected()
		{
			return _databaseProvider?.IsConnected() ?? false;
		}

		public List<SearchResultModel> GetCachedResults(string sourceText, string targetLanguageName, string queryString)
		{
			var searchCache = _databaseProvider?.Get(sourceText, targetLanguageName, queryString);

			return searchCache != null
				? DeserializeSearchResult(searchCache.SearchResultsString)
				: null;
		}

		private string SerializeSearchResult(List<SearchResultModel> searchResults)
		{
			return JsonConvert.SerializeObject(searchResults, Formatting.Indented, GetJsonSettings());
		}

		public List<SearchResultModel> DeserializeSearchResult(string searchResultsString)
		{
			var settings = new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All,
				Converters = { new SearchResultModelConverter(), new CultureInfoConverter() }
			};

			return JsonConvert.DeserializeObject<List<SearchResultModel>>(searchResultsString, settings);
		}

		private JsonSerializerSettings GetJsonSettings()
		{
			return new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.All
			};
		}

		public void Dispose()
		{
			_databaseProvider?.CloseConnection();
		}
	}

	public class CultureInfoConverter : JsonConverter<CultureInfo>
	{
		public override CultureInfo ReadJson(JsonReader reader, Type objectType, CultureInfo existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
			{
				string cultureCodeString = (string)reader.Value;
				return CultureInfo.GetCultureInfo(cultureCodeString);
			}

			if (reader.TokenType == JsonToken.StartObject)
			{
				// Load the JSON object
				var cultureCodeObject = JObject.Load(reader);
				var name = cultureCodeObject["Name"].ToObject<string>(serializer);

				return CultureInfo.GetCultureInfo(name);
			}

			throw new JsonReaderException($"Unexpected token type: {reader.TokenType}");
		}

		public override void WriteJson(JsonWriter writer, CultureInfo value, JsonSerializer serializer)
		{
			writer.WriteValue(value.ToString());
		}
	}

	public class SearchResultModelConverter : JsonConverter<SearchResultModel>
	{
		public override SearchResultModel ReadJson(JsonReader reader, Type objectType, SearchResultModel existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var token = JToken.Load(reader);
			var result = new SearchResultModel();
			serializer.Populate(token.CreateReader(), result);

			// Convert the ILanguage property using the CultureCode converter
			//result.Language = token["Language"].ToObject<ILanguage>(serializer);
			return result;
		}

		public override void WriteJson(JsonWriter writer, SearchResultModel value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}

	public class LanguageConverter : JsonConverter<ILanguage>
	{
		public override void WriteJson(JsonWriter writer, ILanguage value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override ILanguage ReadJson(JsonReader reader, Type objectType, ILanguage existingValue,
			bool hasExistingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.String)
			{
				var cultureCodeString = (string)reader.Value;
				var language = GetLanguage(cultureCodeString);

				return language;
			}

			if (reader.TokenType == JsonToken.StartObject)
			{
				// Load the JSON object
				var cultureCodeObject = JObject.Load(reader);
				var cultureCodeString = cultureCodeObject["Locale"]["Name"].ToString();

				var language = GetLanguage(cultureCodeString);

				return language;
			}

			throw new JsonReaderException($"Unexpected token type: {reader.TokenType}");
		}

		private static ILanguage GetLanguage(string cultureCodeString)
		{
			var languageBase = Sdl.Core.Globalization.LanguageRegistry.LanguageRegistryApi.Instance.GetLanguage(cultureCodeString);

			var definitionLanguage = new DefinitionLanguage
			{
                LanguageIsoCode = languageBase.CultureInfo.Name,
				Name = cultureCodeString
			};

			var language = (ILanguage)definitionLanguage;
			return language;
		}
	}

}
