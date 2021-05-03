using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class InventoriesProvider
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly ConnectionProvider _connectionProvider;

		public InventoriesProvider(ConnectionProvider connectionProvider)
		{
			_connectionProvider = connectionProvider;
		}

		public bool IsInitialized { get; private set; }

		public List<ItemsResponseModel> Domains { get; private set; }

		public List<ItemsResponseModel> TermTypes { get; private set; }

		public async Task<bool> Initialize()
		{
			if (IsInitialized)
			{
				return true;
			}

			try
			{
				_logger.Info($"--> Try to recover inventories");

				Domains = await Task.FromResult(await GetDomains());
				_logger.Info($"--> Recoved {Domains?.Count} Domains");

				TermTypes = await Task.FromResult(await GetTermTypes());
				_logger.Info($"--> Recoved {TermTypes?.Count} Term Types");

				IsInitialized = true;
			}
			catch (Exception ex)
			{
				IsInitialized = false;
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
			}

			return IsInitialized;
		}

		private async Task<List<ItemsResponseModel>> GetDomains()
		{
			var domains = new List<ItemsResponseModel>();

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetDomainUri())
			};

			var httpResponse = await _connectionProvider.HttpClient.SendAsync(httpRequest);

			try
			{
				httpResponse?.EnsureSuccessStatusCode();

				if (httpResponse?.Content != null)
				{
					var httpResponseAsString = await httpResponse.Content?.ReadAsStringAsync();

					var jsonDomainsModel = JsonConvert.DeserializeObject<JsonDomainResponseModel>(httpResponseAsString);
					if (jsonDomainsModel?.Items != null)
					{
						foreach (var item in jsonDomainsModel.Items)
						{
							var domain = new ItemsResponseModel
							{
								Code = item.Code,
								Name = item.Name,
								Subdomains = item.Subdomains,
							};
							
							domains.Add(domain);
						}
					}
				}
			}
			finally
			{
				httpResponse?.Dispose();
			}

			return await Task.FromResult(domains);
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
						termTypes = new List<ItemsResponseModel>(jsonTermTypesModel.Items);
					}
				}
			}
			finally
			{
				httpResponse?.Dispose();
			}

			return await Task.FromResult(termTypes);
		}
	}
}