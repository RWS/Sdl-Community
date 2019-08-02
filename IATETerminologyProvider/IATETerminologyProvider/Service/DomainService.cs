using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public static class DomainService
	{
		public static readonly Log Log = Log.Instance;

		/// <summary>
		/// Get domains from IATE database.
		/// </summary>
		/// <returns>domains</returns>
		public static ObservableCollection<ItemsResponseModel> GetDomains()
		{
			var domains = new ObservableCollection<ItemsResponseModel>();
			var httpClient = new HttpClient { BaseAddress = new Uri(ApiUrls.GetDomainUri()) };
			Utils.AddDefaultParameters(httpClient);

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get
			};

			try
			{
				var httpResponse = httpClient.SendAsync(httpRequest);
				var httpResponseAsString = httpResponse?.Result?.Content?.ReadAsStringAsync().Result;
				Log.Logger.Info($"The response when receiving domains: {httpResponseAsString}");

				var jsonDomainsModel = JsonConvert.DeserializeObject<JsonDomainResponseModel>(httpResponseAsString);
				if (jsonDomainsModel?.Items != null)
				{
					foreach (var item in jsonDomainsModel.Items)
					{
						var domain = new ItemsResponseModel
						{
							Code = item.Code,
							Name = item.Name,
							Subdomains = item.Subdomains
						};
						domains.Add(domain);
					}
				}
				return domains;
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{e.Message}\n{e.StackTrace}");
			}
			return null;
		}
	}
}