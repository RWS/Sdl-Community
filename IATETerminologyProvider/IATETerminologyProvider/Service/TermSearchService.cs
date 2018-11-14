using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using RestSharp;

namespace IATETerminologyProvider.Service
{
	public class TermSearchService
	{
		private ProviderSettings _providerSettings;
		public TermSearchService(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}
	
		public IRestResponse GetTerms()
		{
			var client = new RestClient(ApiUrls.BaseUri());
			var request = new RestRequest("", Method.POST);
			request.AddParameter("expand", "true");
			request.AddParameter("offset", _providerSettings.Offset);
			request.AddParameter("limit", _providerSettings.Limit);

			var response = client.Execute(request);
			return response;
		}
	}
}