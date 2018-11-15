using System.Collections.Generic;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
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
	
		public IList<ISearchResult> GetTerms(string text, ILanguage source, ILanguage destination, int maxResultsCount)
		{
			var result = new List<ISearchResult>();

			var client = new RestClient(ApiUrls.BaseUri("true", _providerSettings.Offset.ToString(), _providerSettings.Limit.ToString()));
			var request = new RestRequest("", Method.POST);
			request.AddHeader("Connection", "Keep-Alive");
			request.AddHeader("Cache-Control", "no-cache");
			request.AddHeader("Pragma", "no-cache");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Accept-Encoding", "gzip, deflate, br");
			request.AddHeader("Content-Type", "application/json");
			request.AddHeader("Origin", "https://iate.europa.eu");
			request.AddHeader("Host", "iate.europa.eu");			
			request.AddHeader("Access-Control-Allow-Origin", "*");
			request.AddHeader("Proxy Connection", "Keep-Alive");

			var bodyModel = SetApiRequestBodyValues(destination, source, text);
			request.AddJsonBody(bodyModel);

			var response = client.Execute(request);

			//To Do: map needed information from response into IList<ISearchResult> and return the list
			return result;
		}

		// Set the needed fields for the API search request
		private object SetApiRequestBodyValues(ILanguage destination, ILanguage source, string text)
		{
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

			var bodyModel = new
			{
				query = text,
				source = source.Locale.TwoLetterISOLanguageName,
				targets = targetLanguges,
				include_subdomains = true,
				filter_by_entry_collection = new List<int>(),
				search_in_fields = searchInFields,
				search_in_term_types = searchInTermsTypes,
				filter_by_domains = new List<int>(),
				query_operator = 1
			};
			return bodyModel;
		}
	}
}