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

			// maxResults (the number of returned words) value is set from the Termbase -> Search Settings
			var client = new RestClient(ApiUrls.BaseUri("true", _providerSettings.Offset.ToString(), maxResultsCount.ToString()));

			// _providerSettings.Offset (the number of returned words) is set from the Provider Settings grid
			//var client = new RestClient(ApiUrls.BaseUri("true", _providerSettings.Offset.ToString(), _providerSettings.Limit.ToString()));
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

			var bodyModel = new
			{
				query = text,
				source = source.Locale.TwoLetterISOLanguageName,
				targets = targetLanguges,
				include_subdomains = true
			};
			return bodyModel;
		}
	}
}