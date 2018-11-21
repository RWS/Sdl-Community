using System.Collections.Generic;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;
using RestSharp;

namespace IATETerminologyProvider.Service
{
	public static class DomainService
	{
		#region Public Properties
		public static List<ItemsResponseModel> Domains = new List<ItemsResponseModel>();
		#endregion

		#region Public Methods
		public static async void GetDomains()
		{
			Domains.Clear();

			var client = new RestClient(ApiUrls.GetDomainUri());
			var request = new RestRequest("", Method.GET);
			request.AddHeader("Connection", "Keep-Alive");
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Pragma", "no-cache");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Origin", "https://iate.europa.eu");
			request.AddHeader("Host", "iate.europa.eu");
			request.AddHeader("Access-Control-Allow-Origin", "*");

			var response = await client.ExecuteTaskAsync(request);
			var jsonDomainsModel = JsonConvert.DeserializeObject<JsonDomainResponseModel>(response.Content);

			if(jsonDomainsModel.Items != null)
			{
				foreach(var item in jsonDomainsModel.Items)
				{
					var domain = new ItemsResponseModel
					{
						Code = item.Code,
						Name = item.Name,
						Subdomains = item.Subdomains
					};
					Domains.Add(domain);
				}
			}
		}
		#endregion
	}
}