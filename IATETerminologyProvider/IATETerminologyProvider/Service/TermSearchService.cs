using System.Collections.Generic;
using System.Linq;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Service
{
	public class TermSearchService
	{
		#region Private Fields
		private ProviderSettings _providerSettings;
		private List<ItemsResponseModel> _domains = DomainService.Domains;
		private List<string> _subdomains = new List<string>();

		private static int _id = new int();
		#endregion

		#region Constructors
		public TermSearchService(ProviderSettings providerSettings)
		{
			_providerSettings = providerSettings;
		}
		public TermSearchService()
		{

		}
		#endregion

		#region Public Methods
		public IList<ISearchResult> GetTerms(string text, ILanguage source, ILanguage destination, int maxResultsCount)
		{
			// maxResults (the number of returned words) value is set from the Termbase -> Search Settings
			var client = new RestClient(ApiUrls.BaseUri("true", "0", maxResultsCount.ToString()));

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
			var domainsJsonResponse = JsonConvert.DeserializeObject<JsonDomainResponseModel>(response.Content);

			var result = MapResponseValues(response, domainsJsonResponse);
			return result;
		}		
		#endregion

		#region Private Methods
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

		private IList<ISearchResult> MapResponseValues(IRestResponse response, JsonDomainResponseModel domainResponseModel)
		{
			var termsList = new List<ISearchResult>();			
			var jObject = JObject.Parse(response.Content);
			var itemTokens = (JArray)jObject.SelectToken("items");
			if (itemTokens != null)
			{
				foreach (var item in itemTokens)
				{
					var itemId = item.SelectToken("id").ToString();
					var domainModel = domainResponseModel.Items.Where(i => i.Id == itemId).FirstOrDefault();
					var domain = SetTermDomain(domainModel);
					SetTermSubdomains(domainModel);

					_id++;
					// get language childrens (source + target languages)
					var languageTokens = item.SelectToken("language").Children().ToList();
					if(languageTokens.Any())
					{
						// foreach language token get the terms
						foreach (JProperty languageToken in languageTokens)
						{
							// Latin translations are automatically returned by IATE API response->"la" code
							// Ignore the "la" translations
							if (!languageToken.Name.Equals("la"))
							{
								var termEntry = languageToken.FirstOrDefault().SelectToken("term_entries").Last;
								var termValue = termEntry.SelectToken("term_value").ToString();
								var langTwoLetters = languageToken.Name;
								var definition = languageToken.Children().FirstOrDefault() != null
									? languageToken.Children().FirstOrDefault().SelectToken("definition")
									: null;

								var languageModel = new LanguageModel
								{
									Name = new Language(langTwoLetters).DisplayName,
									Locale = new Language(langTwoLetters).CultureInfo
								};

								var termResult = new SearchResultModel
								{
									Text = termValue,
									Id = _id,
									Score = 100,
									Language = languageModel,
									Definition = definition != null ? definition.ToString() : string.Empty,
									Domain = domain,
									Subdomain = FormatSubdomain()
								};
								termsList.Add(termResult);
							}
						}
					}
				}
			}
			return termsList;
		}

		// Set term main domain
		private string SetTermDomain(ItemsResponseModel itemDomains)
		{
			var domain = string.Empty;
			foreach (var itemDomain in itemDomains.Domains)
			{
				var result = _domains.Where(d => d.Code.Equals(itemDomain.Code)).FirstOrDefault();
				if (result != null)
				{
					domain = $"{result.Name}, ";
				}
			}
			return domain.TrimEnd(' ').TrimEnd(',');
		}


		// Set term subdomain
		private void SetTermSubdomains(ItemsResponseModel mainDomains)
		{
			// clear _subdomains list for each term
			_subdomains.Clear();
			if (_domains.Count > 0)
			{
				foreach (var mainDomain in mainDomains.Domains)
				{
					foreach (var domain in _domains)
					{
						// if result returns null, means that code belongs to a subdomain
						var result = domain.Code.Equals(mainDomain.Code) ? domain : null;
						if (result == null && domain.Subdomains != null)
						{
							GetSubdomainsRecursively(domain.Subdomains, mainDomain.Code, mainDomain.Note);
						}
					}
				}
			}
		}

		private void GetSubdomainsRecursively(List<SubdomainsResponseModel> subdomains, string code, string note)
		{
			foreach (var subdomain in subdomains)
			{
				if (subdomain.Code.Equals(code))
				{
					if (!string.IsNullOrEmpty(note))
					{
						var subdomainName = $"{subdomain.Name}. {note}";
						_subdomains.Add(subdomainName);
					}
					else
					{
						_subdomains.Add(subdomain.Name);
					}
				}
				else
				{
					if (subdomain.Subdomains != null)
					{
						GetSubdomainsRecursively(subdomain.Subdomains, code, note);
					}
				}
			}
		}

		// Format the subdomain in order to be displayed user friendly.
		private string FormatSubdomain()
		{
			var result = string.Empty;
			int subdomainNo = 0;
			foreach (var subdomain in _subdomains)
			{
				subdomainNo++;
				result+= $"{ subdomainNo}.{subdomain}. ";
			}
			return result.TrimEnd(' ');
		}
		#endregion
	}
}