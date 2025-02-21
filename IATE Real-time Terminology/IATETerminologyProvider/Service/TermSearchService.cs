using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.Model.ResponseModels;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class TermSearchService
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly ConnectionProvider _connectionProvider;
		private readonly InventoriesProvider _inventoriesProvider;
		private readonly List<string> _subdomains;
		private readonly List<TermTypeModel> _termTypes;
		private int _termIndexId;

		public TermSearchService(ConnectionProvider connectionProvider,
			InventoriesProvider inventoriesProvider)
		{
			_connectionProvider = connectionProvider;
			_inventoriesProvider = inventoriesProvider;

			_subdomains = new List<string>();
			_termTypes = new List<TermTypeModel>();

			SetTermTypes(_inventoriesProvider.TermTypes);
		}

		/// <summary>
		/// Get terms from IATE database.
		/// </summary>
		/// <param name="jsonBody">Values in the jsonBody of the requests</param>
		/// <param name="searchDepth"></param>
		/// <returns>terms</returns>
		public List<SearchResultModel> GetTerms(string jsonBody, int searchDepth)
		{
			if (!_connectionProvider.EnsureConnection())
			{
				throw new Exception("Failed login!");
			}

			var mediaType = new ContentType("application/json").MediaType;
			var content = new StringContent(jsonBody, Encoding.UTF8, mediaType);

			// we need to remove the charset otherwise we'll receive Unsupported Media Type error from IATE
			content.Headers.ContentType.CharSet = string.Empty;

			var httpRequest = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(ApiUrls.SearchUri("true", searchDepth)),
				Content = content
			};

			_logger.Info("--> Search call to iate");
			var httpResponse = _connectionProvider.HttpClient.SendAsync(httpRequest)?.Result;

			httpResponse?.EnsureSuccessStatusCode();
			try
			{
				var httpResponseString = httpResponse?.Content?.ReadAsStringAsync().Result;
				var domainsJsonResponse = JsonConvert.DeserializeObject<JsonDomainResponseModel>(httpResponseString);

				var results = MapResponseValues(httpResponseString, domainsJsonResponse);
				_logger.Info("--> Response received from IATE");

				return results;
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n{ex.StackTrace}");
				throw;
			}
			finally
			{
				httpResponse?.Dispose();
				content.Dispose();
			}
		}

		/// <summary>
		/// Map the terms values returned from the IATE API response with the SearchResultModel
		/// </summary>
		/// <param name="response">IATE API response</param>
		/// <param name="domainResponseModel">domains response model</param>
		/// <returns>list of terms</returns>
		private List<SearchResultModel> MapResponseValues(string response, JsonDomainResponseModel domainResponseModel)
		{
			var termsList = new List<SearchResultModel>();
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

						// get language children (source + target languages)
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
										_logger.Error($"{e.Message}\n{e.StackTrace}");
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
				var result = _inventoriesProvider.Domains?.FirstOrDefault(d => d.Code.Equals(itemDomain.Code));
				if (result != null)
				{
					domain = $"{result.Name}, ";
				}
			}

			return domain.TrimEnd(' ').TrimEnd(',');
		}

		// Set term subdomains
		private void SetTermSubdomains(ItemsResponseModel mainDomains)
		{
			// clear _subdomains list for each term
			_subdomains.Clear();
			if (_inventoriesProvider.Domains?.Count > 0)
			{
				foreach (var mainDomain in mainDomains.Domains)
				{
					foreach (var domain in _inventoriesProvider.Domains)
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
				result += $"{subdomainNo}.{subdomain}  ";
			}

			return result.TrimEnd(' ');
		}

		// Return the term type name based on the term type code.
		private string GetTermTypeByCode(string termTypeCode)
		{
			var typeCode = int.TryParse(termTypeCode, out _)
				? int.Parse(termTypeCode) : 0;

			return _termTypes?.Count > 0
				? _termTypes?.FirstOrDefault(t => t.Code == typeCode)?.Name
				: string.Empty;
		}

		private void SetTermTypes(IEnumerable<ItemsResponseModel> termTypesResponse)
		{
			foreach (var item in termTypesResponse)
			{
				var selectedTermTypeName = Utils.UppercaseFirstLetter(item.Name.ToLower());

				var termType = new TermTypeModel
				{
					Code = int.TryParse(item.Code, out _) ? int.Parse(item.Code) : 0,
					Name = selectedTermTypeName
				};

				_termTypes.Add(termType);
			}
		}
	}
}