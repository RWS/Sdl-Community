using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class InventoriesProvider
	{
		private readonly ConnectionProvider _connectionProvider;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public InventoriesProvider(ConnectionProvider connectionProvider)
		{
			_connectionProvider = connectionProvider;
		}

		public List<IateCollection> Collections { get; set; }
		public List<ItemsResponseModel> Domains { get; private set; }
		public List<IateInstitution> Institutions { get; set; }
		public bool IsInitialized { get; private set; }
		public List<ItemsResponseModel> TermTypes { get; private set; }

		public async Task<bool> Initialize()
		{
			if (IsInitialized)
			{
				return true;
			}

			try
			{
				//_logger.Info("--> Try to recover inventories");

				Institutions = await GetInstitutions();
				//_logger.Info($"--> Recovered {Domains?.Count} Institutions");

				Collections = await GetCollections();
				//_logger.Info($"--> Recovered {Domains?.Count} Collections");

				Domains = await GetDomains();
				//_logger.Info($"--> Recovered {Domains?.Count} Domains");

				TermTypes = await GetTermTypes();
				//_logger.Info($"--> Recovered {TermTypes?.Count} Term Types");

				IsInitialized = true;
			}
			catch (Exception ex)
			{
				IsInitialized = false;
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
			}

			return IsInitialized;
		}

		private async Task<List<IateCollection>> GetCollections()
		{
			var getCollectionsRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetCollectionsUri())
			};
			getCollectionsRequest.Headers.Add("Accept", "application/vnd.iate.collection+json;version=2");

			var getCollectionsResponse = await _connectionProvider.HttpClient.SendAsync(getCollectionsRequest);

			var collectionsJson = new List<IateCollection>();
			try
			{
				getCollectionsResponse?.EnsureSuccessStatusCode();

				if (getCollectionsResponse?.Content != null)
				{
					var getCollectionsResponseAsString = await getCollectionsResponse.Content?.ReadAsStringAsync();
					collectionsJson =
						JObject.Parse(getCollectionsResponseAsString)["items"].ToObject<List<IateCollection>>(
							JsonSerializer.Create(new JsonSerializerSettings
							{
								ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
								NullValueHandling = NullValueHandling.Ignore
							}));
				}
			}
			finally
			{
				getCollectionsResponse?.Dispose();
			}

			return collectionsJson;
		}

		private async Task<List<ItemsResponseModel>> GetDomains()
		{
			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetDomainUri())
			};

			var httpResponse = await _connectionProvider.HttpClient.SendAsync(httpRequest);

			var jsonDomainsModel = new JsonDomainResponseModel();
			try
			{
				httpResponse?.EnsureSuccessStatusCode();

				if (httpResponse?.Content != null)
				{
					var httpResponseAsString = await httpResponse.Content?.ReadAsStringAsync();
					jsonDomainsModel = JsonConvert.DeserializeObject<JsonDomainResponseModel>(httpResponseAsString);
				}
			}
			finally
			{
				httpResponse?.Dispose();
			}

			return jsonDomainsModel.Items;
		}

		private async Task<List<IateInstitution>> GetInstitutions()
		{
			var getInstitutionsRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetInstitutionsUri())
			};

			var getInstitutionsResponse = await _connectionProvider.HttpClient.SendAsync(getInstitutionsRequest);

			var institutionsJson = new List<IateInstitution>();

			try
			{
				getInstitutionsResponse?.EnsureSuccessStatusCode();

				if (getInstitutionsResponse?.Content != null)
				{
					var getCollectionsResponseAsString = await getInstitutionsResponse.Content?.ReadAsStringAsync();
					institutionsJson =
						JObject.Parse(getCollectionsResponseAsString)["items"].ToObject<List<IateInstitution>>(
							JsonSerializer.Create(new JsonSerializerSettings
							{
								ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
								NullValueHandling = NullValueHandling.Ignore
							}));
				}
			}
			finally
			{
				getInstitutionsResponse?.Dispose();
			}

			return institutionsJson;
		}

		private async Task<List<ItemsResponseModel>> GetTermTypes()
		{
			var termTypes = new List<ItemsResponseModel>();

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetTermTypeUri("true", "en", "100", "0")),
			};

			var httpResponse = await _connectionProvider.HttpClient.SendAsync(httpRequest);
			try
			{
				httpResponse?.EnsureSuccessStatusCode();

				if (httpResponse?.Content != null)
				{
					var httpResponseAsString = await httpResponse.Content.ReadAsStringAsync();

					var jsonTermTypesModel = JsonConvert.DeserializeObject<TermTypeResponseModel>(httpResponseAsString);

					if (jsonTermTypesModel?.Items != null)
					{
						var validTermTypes =
							jsonTermTypesModel.Items.Where(t => t.Deprecated is null || t.Deprecated == false);
						termTypes = new List<ItemsResponseModel>(validTermTypes);
					}
				}
			}
			finally
			{
				httpResponse?.Dispose();
			}

			return termTypes;
		}
	}
}