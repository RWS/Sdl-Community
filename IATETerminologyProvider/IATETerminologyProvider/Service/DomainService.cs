using System;
using System.Collections.ObjectModel;
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
		/// <returns>IATE Domains</returns>
		public async Task<ObservableCollection<ItemsResponseModel>> GetDomains()
		{
			var domains = new ObservableCollection<ItemsResponseModel>();

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(ApiUrls.GetDomainUri())
			};
			var httpResponse = await IateApplicationInitializer.Clinet.SendAsync(httpRequest);

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
								Subdomains = item.Subdomains
							};
							domains.Add(domain);
						}
					}
				}
				Domains = domains;
				return domains;
			}
			finally
			{
				httpResponse?.Dispose();
			}
		}
	}
}