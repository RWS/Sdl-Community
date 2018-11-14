using System.Collections.Generic;
using System.Threading.Tasks;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Model.ResponseModels;
using RestSharp;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Service
{
	public class TermSearchService
	{
		private ProviderSettings _providerSettings;
		public TermSearchService(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}
	
		public IRestResponse GetTerms(string text, ILanguage source, ILanguage destination, int maxResultsCount)
		{
			var client = new RestClient(ApiUrls.BaseUri());
			var request = new RestRequest("", Method.POST);
			request.AddParameter("Connection", "keep-alive");
			request.AddParameter("Content-Type", "application/json");
			request.AddParameter("Accept", "application/json");
			request.AddParameter("expand", "true");
			request.AddParameter("offset", _providerSettings.Offset.ToString());
			request.AddParameter("limit", _providerSettings.Limit.ToString());

			var targetLanguges = new List<string>();
			targetLanguges.Add(destination.Locale.TwoLetterISOLanguageName);

			var searchInFields = new List<int>();
			searchInFields.Add(0);

			var searchInTermsTypes = new List<int>();
			searchInFields.Add(0);
			searchInFields.Add(1);
			searchInFields.Add(2);
			searchInFields.Add(3);
			searchInFields.Add(4);

			var requestModel = new RequestResponseModel
			{
				Query = text,
				Source = source.Locale.TwoLetterISOLanguageName,
				Targets = targetLanguges,
				IncludeSubdomains = true,
				FilterByEntryCollection = new List<int>(),
				SearchInFields = searchInFields,
				SearchInTermTypes = searchInTermsTypes,
				FilterByDomains = new List<int>(),
				QueryOperator = 1
			};

			request.AddJsonBody(requestModel);

			var response = client.Execute(request);
			return response;
		}
	}
}