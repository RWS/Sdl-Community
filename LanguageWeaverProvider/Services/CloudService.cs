using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.Studio.FeedbackController.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;

namespace LanguageWeaverProvider.Services
{
    public static class CloudService
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static async Task<bool> AuthenticateSSOUser(ITranslationOptions translationOptions, CloudAuth0Config auth0Config, Uri uri, string selectedRegion)
        {
            try
            {
                var uriParams = uri.PathAndQuery.TrimStart('/');
                var parameters = HttpUtility.ParseQueryString(uriParams);
                var param = HttpUtility.ParseQueryString(uriParams).AllKeys.ToDictionary(x => x, x => parameters[x]);
                param["client_id"] = auth0Config.ClientId;
                param["redirect_uri"] = auth0Config.RedirectUri;
                param["code_verifier"] = auth0Config.CodeVerifier;
                param["grant_type"] = "authorization_code";

                var requestUri = new Uri("https://sdl-prod.eu.auth0.com/oauth/token");
                var formUrlEncodedContent = new FormUrlEncodedContent(param);
                using var httpRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Post,
                    RequestUri = requestUri,
                    Content = formUrlEncodedContent
                };

                var result = await GetHttpClient().SendAsync(httpRequest);
                var content = result.Content.ReadAsStringAsync().Result;
                //var x = await Service.SendRequest(HttpMethod.Post, formUrlEncodedContent, content: content);
                if (!result.IsSuccessStatusCode)
                {
                    var errorResponse = JsonConvert.DeserializeObject<CloudAuth0Error>(content);
                    ErrorHandling.ShowDialog(null, $"{result.StatusCode} {(int)result.StatusCode}", errorResponse.ToString());
                    return false;
                }

                var ssoToken = JsonConvert.DeserializeObject<CloudAuth0Response>(content);
                translationOptions.AccessToken = new AccessToken
                {
                    Token = ssoToken.AccessToken,
                    TokenType = ssoToken.TokenType,
                    RefreshToken = ssoToken.RefreshToken,
                    ExpiresAt = (long)(DateTime.UtcNow.AddSeconds(ssoToken.ExpiresIn) - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalMilliseconds,
                    BaseUri = new Uri(selectedRegion)
                };

                await SetAccountId(translationOptions, selectedRegion);
                return true;
            }
            catch (Exception ex)
            {
                ex.ShowDialog("Authentication failed", ex.Message, true);
                return false;
            }
        }

        public static async Task<bool> AuthenticateUser(ITranslationOptions translationOptions, AuthenticationType authenticationType)
        {
            try
            {
                var cloudCredentials = translationOptions.CloudCredentials;
                var response = await Authenticate(cloudCredentials, authenticationType);
                if (response.IsSuccessStatusCode)
                {
                    _logger.Log(LogLevel.Info, "Authentication successful.");

                    translationOptions.AccessToken = await response.DeserializeResponse<AccessToken>();
                    translationOptions.AccessToken.BaseUri = new Uri(cloudCredentials.AccountRegion);
                    await SetAccountId(translationOptions, cloudCredentials.AccountRegion, cloudCredentials);
                    return true;
                }

                _logger.Log(LogLevel.Info, "Authentication unsuccessful.");
                var cloudAccountError = await response.DeserializeResponse<CloudAccountError>("status");
                ErrorHandling.ShowDialog(null, $"{response.StatusCode} {(int)response.StatusCode}",
                    $"Code: {cloudAccountError.Code}\nMessage: {cloudAccountError.Description}");
                return false;
            }
            catch (Exception ex)
            {
                var message = $"{ex.Message}. {Environment.StackTrace}.";
                _logger.Log(LogLevel.Error, message);
                ex.ShowDialog("Authentication failed", message, true);
                return false;
            }
        }

        private static async Task SetAccountId(ITranslationOptions translationOptions, string uri, CloudCredentials cloudCredentials = null)
        {
            var requesturi = translationOptions.AuthenticationType switch
            {
                AuthenticationType.CloudCredentials => $"{uri}v4/accounts/users/self",
                AuthenticationType.CloudAPI => $"{uri}v4/accounts/api-credentials/self",
                AuthenticationType.CloudSSO => $"{uri}v4/accounts/users/self",
            };

            var accountId = await GetUserInfo(translationOptions.AccessToken, requesturi, "accountId");
            translationOptions.AccessToken.AccountId = accountId;
            if (cloudCredentials is not null)
            {
                cloudCredentials.AccountId = accountId;
            }
        }

        private static async Task<HttpResponseMessage> Authenticate(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
        {
            _logger.Log(LogLevel.Info,
                $"[Authenticate] start - CloudCredentials:{cloudCredentials}, AuthenticationType: {authenticationType}");

            var requestUri = authenticationType switch
            {
                AuthenticationType.CloudCredentials => $"{cloudCredentials.AccountRegion}v4/token/user",
                AuthenticationType.CloudAPI => $"{cloudCredentials.AccountRegion}v4/token",
                _ => throw new ArgumentOutOfRangeException(nameof(authenticationType), authenticationType, "Unsupported authentication type.")
            };
                                                         
            var content = GetAuthenticationContent(cloudCredentials, authenticationType);
            var stringContent = new StringContent(content, null, "application/json");

            var response = await Service.SendRequest(HttpMethod.Post, requestUri, content: stringContent);
            return response;
        }

        private static string GetAuthenticationContent(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
        {
            var (idKey, idValue, secretKey, secretValue) = authenticationType switch
            {
                AuthenticationType.CloudCredentials => ("username", cloudCredentials.UserName, "password", cloudCredentials.UserPassword),
                AuthenticationType.CloudAPI => ("clientId", cloudCredentials.ClientID, "clientSecret", cloudCredentials.ClientSecret)
            };

            return $"\r\n{{\r\n    \"{idKey}\": \"{idValue}\",\r\n    \"{secretKey}\": \"{secretValue}\"\r\n}}";
        }

        private static async Task<string> GetUserInfo(AccessToken accessToken, string requestUri, string property)
        {
            var response = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);
            var accountId = await Service.DeserializeResponse<string>(response, property);
            return accountId;
        }

        public static async Task<List<T>> GetResources<T>(AccessToken accessToken, CloudResources resource)
        {
            const string LanguagePairsEndPoint = "subscriptions/language-pairs";
            const string LanguagePairsProperty = "languagePairs";
            const string DictionariesEndPoint = "dictionaries";
            const string DictionariesProperty = "dictionaries";

            var (resourceRequested, property) = resource switch
            {
                CloudResources.LanguagePairs => (LanguagePairsEndPoint, LanguagePairsProperty),
                CloudResources.Dictionaries => (DictionariesEndPoint, DictionariesProperty),
                _ => throw new ArgumentException("Unsupported Resource value")
            };

            var requestUri = $"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/{resourceRequested}";
            var response = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);
            var languagePairs = await Service.DeserializeResponse<List<T>>(response, property);
            return languagePairs;
        }

        public static async Task<Xliff> Translate(AccessToken accessToken, PairMapping mappedPair, Xliff sourceXliff)
        {
            try
            {
                var translationRequest = await SendTranslationRequest(accessToken, mappedPair, sourceXliff);
                await WaitForTranslationCompletion(accessToken, translationRequest.RequestId);
                var translation = await GetTranslationInfo<CloudTranslationResponse>(accessToken, translationRequest.RequestId, "content");
                var translatedSegment = translation.Translation.First();
                var translatedXliffSegment = Converter.ParseXliffString(translatedSegment);
                return translatedXliffSegment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static async Task<CloudTranslationRequestResponse> SendTranslationRequest(AccessToken accessToken, PairMapping mappedPair, Xliff sourceXliff)
        {
            var requestUri = $"{accessToken.BaseUri}v4/mt/translations/async";
            var translationRequestModel = CreateTranslationRequest(mappedPair, sourceXliff);
            var translationRequestModelJson = JsonConvert.SerializeObject(translationRequestModel);
            var content = new StringContent(translationRequestModelJson, Encoding.UTF8, "application/json");
            var response = await Service.SendRequest(HttpMethod.Post, requestUri, accessToken, content);
            var translationRequestResponse = await Service.DeserializeResponse<CloudTranslationRequestResponse>(response);
            return translationRequestResponse;
        }

        private static CloudTranslationRequest CreateTranslationRequest(PairMapping mappedPair, Xliff sourceXliff)
        {
            const string InputFormat = "xliff";

            var linguisticOptionsDictionary = mappedPair.LinguisticOptions?.ToDictionary(lo => lo.Id, lo => lo.SelectedValue);
            var dictionaries = mappedPair.Dictionaries.Where(d => d.IsSelected).Select(d => d.DictionaryId).ToArray();

            var translationRequestModel = new CloudTranslationRequest
            {
                SourceLanguageId = mappedPair.SourceCode,
                TargetLanguageId = mappedPair.TargetCode,
                Input = [sourceXliff.ToString()],
                Model = mappedPair.SelectedModel.Model,
                InputFormat = InputFormat,
                Dictionaries = dictionaries,
                LinguisticOptions = linguisticOptionsDictionary,
                QualityEstimation = mappedPair.SelectedModel.QeSupport ? 1 : 0
            };

            return translationRequestModel;
        }

        private static async Task WaitForTranslationCompletion(AccessToken accessToken, string RequestId)
        {
            CloudTranslationStatus translationStatus;
            bool isWaiting;
            do
            {
                translationStatus = await GetTranslationInfo<CloudTranslationStatus>(accessToken, RequestId);
                isWaiting = translationStatus.Status.Equals("init", StringComparison.OrdinalIgnoreCase)
                         || translationStatus.Status.Equals("translating", StringComparison.OrdinalIgnoreCase);
                if (isWaiting)
                {
                    await Task.Delay(1000);
                }
            } while (isWaiting);
        }

        private static async Task<T> GetTranslationInfo<T>(AccessToken accessToken, string requestId, string endpoint = null)
        {
            var requestUri = $"{accessToken.BaseUri}v4/mt/translations/async/{requestId}/{endpoint}";
            var translationStatusReponse = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);
            var x = await translationStatusReponse.Content.ReadAsStringAsync();
            var translationStatus = await Service.DeserializeResponse<T>(translationStatusReponse);
            return translationStatus;
        }

        public static async Task<bool> CreateFeedback(AccessToken accessToken, FeedbackRequest feedbackRequest)
        {
            try
            {
                var requestUri = $"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/feedback/translations";
                var feedbackRequestJson = JsonConvert.SerializeObject(feedbackRequest);
                var content = new StringContent(feedbackRequestJson, new UTF8Encoding(), "application/json");
                var response = await Service.SendRequest(HttpMethod.Post, requestUri, accessToken, content);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                ErrorHandling.ShowDialog(ex, "Feedback", ex.Message, true);
                return false;
            }
        }

        public static async Task<bool> CreateDictionaryTerm(AccessToken accessToken, PairDictionary pairDictionary, DictionaryTerm newDictionaryTerm)
        {
            var requestUri = $"https://api.languageweaver.com/v4/accounts/{accessToken.AccountId}/dictionaries/{pairDictionary.DictionaryId}/terms";
            var content = JsonConvert.SerializeObject(newDictionaryTerm);
            var stringContent = new StringContent(content, new UTF8Encoding(), "application/json");

            var response = await Service.SendRequest(HttpMethod.Post, requestUri, accessToken, stringContent);
            var isSuccessStatusCode = response.IsSuccessStatusCode;
            if (isSuccessStatusCode)
            {
                return isSuccessStatusCode;
            }

            var errors = await Service.DeserializeResponse<CloudAccountErrors>(response);
            var error = errors.Errors.FirstOrDefault();
            ErrorHandling.ShowDialog(null, $"Code {error?.Code}", error?.Description);
            return isSuccessStatusCode;
        }

        public static HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(Constants.TraceAppKey, Constants.TraceAppValue);
            httpClient.DefaultRequestHeaders.Add(Constants.TraceAppVersionKey, ApplicationInitializer.CurrentAppVersion);
            return httpClient;
        }

        #region Account subscription - ON HOLD
        public static async Task<CloudAccount> GetAccountDetails(AccessToken accessToken)
        {
            var uri = new Uri($"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/subscriptions");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
            var response = await GetHttpClient().SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            var output = JsonConvert.DeserializeObject<CloudAccount>(responseContent);
            return output;
        }

        public static async Task<List<AccountCategoryFeature>> GetSubscriptionDetails(AccessToken accessToken)
        {
            var uri = new Uri($"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
            var response = await GetHttpClient().SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            var accountCategoryFeatures = JObject.Parse(responseContent)["accountSetting"]["accountCategoryFeatures"].ToObject<List<AccountCategoryFeature>>();
            return accountCategoryFeatures;
        }

        public static async Task<CloudUsageReport> GetUsageReport(AccessToken accessToken, IEnumerable<CloudAccountSubscription> subscriptions)
        {
            var usageReport = new CloudUsageReport();
            foreach (var subscription in subscriptions.Where(sub => sub.IsActive))
            {
                var startDate = DateTime.ParseExact(subscription.StartDate, "yyyy/MM/dd", null);
                var endDate = DateTime.ParseExact(subscription.EndDate, "yyyy/MM/dd", null);

                for (var currentMonthStart = startDate; currentMonthStart < endDate; currentMonthStart = currentMonthStart.AddMonths(3))
                {
                    var currentMonthEnd = currentMonthStart.AddMonths(2);
                    if (currentMonthEnd > endDate)
                    {
                        currentMonthEnd = endDate;
                    }

                    var uri = $"{accessToken.BaseUri}v4/accounts/{accessToken.AccountId}/reports/usage/translations";
                    var request = new HttpRequestMessage(HttpMethod.Get, uri);
                    request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

                    var period = new CloudSubscriptionPeriod
                    {
                        StartDate = currentMonthStart.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture),
                        EndDate = currentMonthEnd.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)
                    };

                    var feedbackRequestJson = JsonConvert.SerializeObject(period);
                    var content = new StringContent(feedbackRequestJson, new UTF8Encoding(), "application/json");

                    var response = await Service.SendRequest(HttpMethod.Post, uri, accessToken, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    var currentPeriodUsageReports = JsonConvert.DeserializeObject<CloudUsageReports>(responseContent);

                    foreach (var currentPeriodUsageReport in currentPeriodUsageReports.Reports)
                    {
                        UpdateUsageReport(usageReport, currentPeriodUsageReport);
                    }
                }
            }

            return usageReport;
        }

        private static void UpdateUsageReport(CloudUsageReport totalUsageReport, CloudUsageReport currentPeriodReport)
        {
            totalUsageReport.OutputWordCount += currentPeriodReport.OutputWordCount;
            totalUsageReport.OutputCharCount += currentPeriodReport.OutputCharCount;
            totalUsageReport.Count += currentPeriodReport.Count;
            totalUsageReport.InputWordCount += currentPeriodReport.InputWordCount;
            totalUsageReport.InputCharCount += currentPeriodReport.InputCharCount;
            // Add more properties if needed
        }
        #endregion
    }
}