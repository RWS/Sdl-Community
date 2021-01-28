using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public class DomainService
	{
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
								Subdomains = item.Subdomains,
								SubdomainIds = new List<string>()
							};
							SetSubdomainsIds(domain);
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

		private void SetSubdomainsIds(ItemsResponseModel domain)
		{
			var subdomainsIds = new List<string>();
			if (domain.Subdomains != null)
			{
				foreach (var subdomain in domain.Subdomains)
				{
					AddUniqueSubdomainId(subdomain.Code, subdomainsIds);
					GetSubdomainsId(subdomain.Subdomains, subdomainsIds);
					if (subdomainsIds.Any())
					{
						domain.SubdomainIds.AddRange(subdomainsIds);
					}
				}
			}
		}

		private void GetSubdomainsId(List<SubdomainsResponseModel> subdomains, List<string> subdomainsIds)
		{
			if (subdomains != null)
			{
				foreach (var subdomain in subdomains)
				{
					AddUniqueSubdomainId(subdomain.Code, subdomainsIds);

					GetSubdomainsId(subdomain.Subdomains, subdomainsIds);
				}
			}
		}

		private void AddUniqueSubdomainId(string subdomainId, List<string> subdomainsIds)
		{
			if (!string.IsNullOrEmpty(subdomainId))
			{
				if (!subdomainsIds.Any(s => s.Equals(subdomainId)))
				{
					subdomainsIds.Add(subdomainId);
				}
			}
		}
	}
}