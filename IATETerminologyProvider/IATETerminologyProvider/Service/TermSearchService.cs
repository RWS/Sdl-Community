using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Model.ResponseModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider.Service
{
	public class TermSearchService
	{
		public static readonly Log Log = Log.Instance;
		private readonly ProviderSettings _providerSettings;
		private readonly ObservableCollection<ItemsResponseModel> _domains;
		private readonly List<string> _subdomains = new List<string>();
		private readonly ObservableCollection<TermTypeModel> _termTypes;
		private readonly AccessTokenService _accessTokenService;
		private int _termIndexId;

		public TermSearchService(ProviderSettings providerSettings)
		{
			_accessTokenService = new AccessTokenService();
			_domains = DomainService.GetDomains();
			_termTypes = TermTypeService.GetTermTypes();
			_providerSettings = providerSettings;
		}

		/// <summary>
		/// Get terms from IATE database.
		/// </summary>
		/// <param name="text">text used for searching</param>
		/// <param name="source">source language</param>
		/// <param name="target">target language</param>
		/// <param name="maxResultsCount">number of maximum results returned(set up in Studio Termbase search settings)</param>
		/// <returns>terms</returns>
		public List<ISearchResult> GetTerms(string text, ILanguage source, ILanguage target, int maxResultsCount)
		{
			SetAccessToken();

			var httpClient = new HttpClient
			{
				BaseAddress = new Uri(ApiUrls.BaseUri("true", "0", maxResultsCount.ToString()))
			};

			Utils.AddDefaultParameters(httpClient);

			var httpRequest = new HttpRequestMessage { Method = HttpMethod.Post };

			if (!string.IsNullOrEmpty(_accessTokenService.AccessToken))
			{
				httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessTokenService.AccessToken);
			}

			var bodyModel = SetApiRequestBodyValues(source, target, text);
			httpRequest.Content = new StringContent(JsonConvert.SerializeObject(bodyModel), Encoding.UTF8, "application/json");

			var httpResponse = httpClient.SendAsync(httpRequest)?.Result;
			var httpResponseString = httpResponse?.Content?.ReadAsStringAsync().Result;
			var domainsJsonResponse = JsonConvert.DeserializeObject<JsonDomainResponseModel>(httpResponseString);

			var result = MapResponseValues(httpResponseString, domainsJsonResponse);
			return result;
		}

		private void SetAccessToken()
		{
			if (_accessTokenService.RefreshTokenExpired
				|| _accessTokenService.RequestedAccessToken == DateTime.MinValue
				|| string.IsNullOrEmpty(_accessTokenService.AccessToken))
			{
				var success = _accessTokenService.GetAccessToken("SDL_PLUGIN", "E9KWtWahXs4hvE9z");
				if (!success)
				{
					throw new Exception(PluginResources.TermSearchService_Error_in_requesting_access_token);
				}
			}
			else if (_accessTokenService.AccessTokenExpired && !_accessTokenService.AccessTokenExtended)
			{
				var success = _accessTokenService.ExtendAccessToken();
				if (!success)
				{
					throw new Exception(PluginResources.TermSearchService_Error_in_refreshing_access_token);
				}
			}
		}

		// Set the needed fields for the API search request
		private object SetApiRequestBodyValues(ILanguage source, ILanguage destination, string text)
		{
			var targetLanguges = new List<string>();
			var filteredDomains = new List<string>();
			var filteredTermTypes = new List<int>();

			targetLanguges.Add(destination.Locale.TwoLetterISOLanguageName);
			if (_providerSettings != null)
			{
				filteredDomains = _providerSettings.Domains.Count > 0 ? _providerSettings.Domains : filteredDomains;
				filteredTermTypes = _providerSettings.TermTypes.Count > 0 ? _providerSettings.TermTypes : filteredTermTypes;
			}

			var bodyModel = new
			{
				query = text,
				source = source.Locale.TwoLetterISOLanguageName,
				targets = targetLanguges,
				include_subdomains = false,
				query_operator = 0,
				filter_by_domains = filteredDomains,
				search_in_term_types = filteredTermTypes
			};

			return bodyModel;
		}

		/// <summary>
		/// Map the terms values returned from the IATE API response with the SearchResultModel
		/// </summary>
		/// <param name="response">IATE API response</param>
		/// <param name="domainResponseModel">domains response model</param>
		/// <returns>list of terms</returns>
		private List<ISearchResult> MapResponseValues(string response, JsonDomainResponseModel domainResponseModel)
		{
			var termsList = new List<ISearchResult>();
			if (!string.IsNullOrEmpty(response))
			{
				var jObject = JObject.Parse(response);
				var itemTokens = (JArray)jObject.SelectToken("items");
				if (itemTokens != null)
				{
					foreach (var item in itemTokens)
					{
						_termIndexId++;

						var itemId = item.SelectToken("id").ToString();
						var domainModel = domainResponseModel.Items.FirstOrDefault(i => i.Id == itemId);
						var domain = SetTermDomain(domainModel);
						SetTermSubdomains(domainModel);

						var searchResultItems = new List<SearchResultModel>();

						// get language childrens (source + target languages)
						var languageTokens = item.SelectToken("language").Children().ToList();
						if (languageTokens.Any())
						{
							// foreach language token get the terms
							foreach (var jToken in languageTokens)
							{
								var languageToken = (JProperty)jToken;

								// Multilingual and Latin translations are automatically returned by IATE API response->"la" code
								// Ignore the "mul" and "la" translations
								if (languageToken.Name.Equals("la") || languageToken.Name.Equals("mul"))
								{
									continue;
								}

								var termEntries = languageToken.FirstOrDefault()?.SelectToken("term_entries");
								if (termEntries == null)
								{
									continue;
								}

								foreach (var termEntry in termEntries)
								{
									var termValue = termEntry.SelectToken("term_value").ToString();
									var termType = GetTermTypeByCode(termEntry.SelectToken("type").ToString());
									var langTwoLetters = languageToken.Name;
									var definition = languageToken.Children().FirstOrDefault() != null
										? languageToken.Children().FirstOrDefault()?.SelectToken("definition")
										: null;

									var displayOrder = -1;
									var evaluation = -1;
									var metaData = termEntry.SelectToken("metadata");

									if (metaData != null)
									{
										var displayOrderObject = metaData.SelectToken("display_order");
										if (displayOrderObject != null)
										{
											displayOrder = displayOrderObject.Value<int>();
										}

										var evaluationObject = metaData.SelectToken("evaluation");
										if (evaluationObject != null)
										{
											evaluation = evaluationObject.Value<int>();
										}
									}

									try
									{
										var languageModel = new LanguageModel
										{
											Name = new Language(langTwoLetters).DisplayName,
											Locale = new Language(langTwoLetters).CultureInfo
										};

										var termResult = new SearchResultModel
										{
											Text = termValue,
											Id = _termIndexId,
											ItemId = itemId,
											Score = 100,
											Language = languageModel,
											Definition = definition?.ToString() ?? string.Empty,
											Domain = domain,
											Subdomain = FormatSubdomain(),
											TermType = termType,
											DisplayOrder = displayOrder,
											Evaluation = evaluation
										};

										searchResultItems.Add(termResult);
									}
									catch (Exception e)
									{
										Log.Logger.Error($"{e.Message}\n{e.StackTrace}");
									}
								}
							}
						}

						termsList.AddRange(searchResultItems);
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
				var result = _domains.FirstOrDefault(d => d.Code.Equals(itemDomain.Code));
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

		// Get subdomains recursively
		private void GetSubdomainsRecursively(IEnumerable<SubdomainsResponseModel> subdomains, string code, string note)
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

		// Format the subdomain in a user friendly mode.
		private string FormatSubdomain()
		{
			var result = string.Empty;
			var subdomainNo = 0;
			foreach (var subdomain in _subdomains.ToList())
			{
				subdomainNo++;
				result += $"{ subdomainNo}.{subdomain}  ";
			}
			return result.TrimEnd(' ');
		}

		// Return the term type name based on the term type code.
		private string GetTermTypeByCode(string termTypeCode)
		{
			var typeCode = int.TryParse(termTypeCode, out _) ? int.Parse(termTypeCode) : 0;
			return _termTypes.Count > 0 ? _termTypes.FirstOrDefault(t => t.Code == typeCode)?.Name : string.Empty;
		}
	}
}