using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public static class DomainService
	{
		#region Public Methods

		/// <summary>
		/// Get domains from IATE database.
		/// </summary>
		/// <returns>domains</returns>
		public static ObservableCollection<ItemsResponseModel> GetDomains()
		{
			var domains = new ObservableCollection<ItemsResponseModel>();

			var httpClient = new HttpClient {BaseAddress = new Uri(ApiUrls.GetDomainUri())};
			Utils.AddDefaultParameters(httpClient);


			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get
			};

			var httpResponse = httpClient.SendAsync(httpRequest);

			var httpResponseAsString = httpResponse.Result.Content.ReadAsStringAsync().Result;

			var jsonDomainsModel =
				JsonConvert.DeserializeObject<JsonDomainResponseModel>(httpResponseAsString);

				if (jsonDomainsModel?.Items != null)
				{
					foreach (var item in jsonDomainsModel.Items)
					{
						var domain = new ItemsResponseModel
						{
							Code = item.Code, Name = item.Name, Subdomains = item.Subdomains
						};
						domains.Add(domain);
					}
				}

				return domains;
		}

		

		#endregion
	}
}