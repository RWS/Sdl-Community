using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public class DomainService
	{
		public static readonly Log Log = Log.Instance;
		public static ObservableCollection<ItemsResponseModel> Domains { get; set; }

		/// <summary>
		/// Get domains from IATE database.
		/// </summary>
		/// <returns>domains</returns>
		public async Task<ObservableCollection<ItemsResponseModel>> GetDomains()
		{
			var domains = new ObservableCollection<ItemsResponseModel>();
			var httpClient = new HttpClient
			{
				BaseAddress = new Uri(ApiUrls.GetDomainUri()),
				Timeout = TimeSpan.FromMinutes(2)
			};
			Utils.AddDefaultParameters(httpClient);

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
			};

			try
			{
				var httpResponse = await httpClient.SendAsync(httpRequest);
				if (httpResponse.StatusCode == HttpStatusCode.OK)
				{
					var httpResponseAsString = await httpResponse.Content.ReadAsStringAsync();

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
					Domains = domains;
					return domains;
				}
				Log.Logger.Error($"Get Domains status code:{httpResponse.StatusCode}");
			}
			catch (Exception e)
			{
				Log.Logger.Error($"{e.Message}\n{e.StackTrace}");
			}
			return domains;
		}
	}
}