using LanguageWeaverProvider.Extensions;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.Services.Model;
using LanguageWeaverProvider.XliffConverter.Converter;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Services
{
    public static class EdgeService
    {
        public static async Task<bool> AuthenticateUser(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
        {
            try
            {
                var userName = Uri.EscapeDataString(edgeCredentials.UserName);
                var password = Uri.EscapeDataString(edgeCredentials.Password);
                var uri = new UriBuilder(edgeCredentials.Uri)
                {
                    Path = "/api/v2/auth",
                    Query = $"username={userName}&password={password}"
                };

                var response = await new HttpClient().PostAsync(uri.Uri, null);

                var isSuccessStatusCode = response.IsSuccessStatusCode;
                if (isSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    SetAccessToken(translationOptions, token, "Bearer", edgeCredentials.Uri);
                    return true;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonConvert.DeserializeObject<EdgeFeedbackError>(responseContent).Error;
                ErrorHandling.ShowDialog(null, $"Authentication failed: {errorResponse.Code}", errorResponse.Message, false);
                return false;
            }
            catch (Exception ex)
            {
                ex.ShowDialog("Authentication failed", ex.Message, true);
                return false;
            }
        }

        public static async Task<bool> CreateDictionaryTerm(AccessToken accessToken, PairDictionary pairDictionary, DictionaryTerm newDictionaryTerm)
        {
            var requestUri = $"{accessToken.BaseUri}api/v2/dictionaries/{pairDictionary.DictionaryId}/term";
            var content = newDictionaryTerm.ToKeyValuePairDictionary();
            var encodedContent = new FormUrlEncodedContent(content);

            var response = await Service.SendRequest(HttpMethod.Post, requestUri, accessToken, encodedContent);
            if (response.IsSuccessStatusCode) return true;

            var result = await response.Content.ReadAsStringAsync();
            ErrorHandling.ShowDialog(null, PluginResources.Dictionary_NewTerm_Unsuccessfully, result);
            return false;
        }

        public static async Task<IEnumerable<PairDictionary>> GetDictionaries(AccessToken accessToken)
        {
            try
            {
                var requestUri = $"{accessToken.BaseUri}api/v2/dictionaries";
                var response = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);

                var dictionaries = await response.DeserializeResponse<EdgeDictionariesResponse>();

                var pairDictionaries = dictionaries.Dictionaries.Select(dictionary => new PairDictionary()
                {
                    Name = dictionary.DictionaryId,
                    DictionaryId = dictionary.DictionaryId,
                    Source = dictionary.SourceLanguageId,
                    Target = dictionary.TargetLanguageId
                });

                return pairDictionaries;
            }
            catch (Exception ex)
            {
                ex.ShowDialog("Dictionaries", ex.Message, true);
                return null;
            }
        }

        public static async Task<IEnumerable<PairModel>> GetLanguagePairs(AccessToken accessToken)
        {
            try
            {
                var requestUri = $"{accessToken.BaseUri}api/v2/language-pairs";

                var response = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);
                var languagePairs = await response.DeserializeResponse<EdgeLanguagePairResult>();

                var pairModels = languagePairs.LanguagePairs.Select(lp => new PairModel()
                {
                    DisplayName = $"{lp.Model} {lp.Version}",
                    SourceLanguageId = lp.SourceLanguageId,
                    TargetLanguageId = lp.TargetLanguageId,
                    LinguisticOptions = lp.LinguisticOptions,
                    Name = lp.LanguagePairId,
                    Model = lp.LanguagePairId
                });

                foreach (var linguisticOption in pairModels.Where(pair => pair.LinguisticOptions is not null).SelectMany(pair => pair.LinguisticOptions))
                {
                    linguisticOption.Name = linguisticOption.Id;
                }

                return pairModels;
            }
            catch (Exception ex)
            {
                ex.ShowDialog("Language pairs", ex.Message, true);
                return null;
            }
        }

        public static async Task<string> SendFeedback(AccessToken accessToken, EdgeFeedbackItem feedbackItem)
        {
            var requestUri = $"{accessToken.BaseUri}api/v2/feedback";

            var feedback = feedbackItem.ToKeyValuePairDictionary();
            var content = new FormUrlEncodedContent(feedback);

            var response = await Service.SendRequest(HttpMethod.Post, requestUri, accessToken, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return JObject.Parse(await response.Content.ReadAsStringAsync())["feedbackId"]?.ToString();

            await HandleError(response);
            return null;
        }

        public static async Task<bool> SignInAuthAsync(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions, WebView2 browser)
        {
            try
            {
                var baseUrl = edgeCredentials.Uri;
                var uri2 = $"{baseUrl}/api/v2/auth/saml/init";
                var uri3 = $"{baseUrl}/api/v2/auth/saml/wait";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, new Uri($"{baseUrl}/api/v2/auth/saml/gen-secret"));
                using var httpClient = new HttpClient();
                var response = await httpClient.SendAsync(httpRequest);

                var secret = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    secret = await response.Content.ReadAsStringAsync();
                }

                var url = $"{uri2}?secret={secret}";
                browser.CoreWebView2.Navigate(url);

                httpRequest = new HttpRequestMessage(HttpMethod.Get, $"{uri3}?secret={secret}");
                response = await httpClient.SendAsync(httpRequest);

                if (response.IsSuccessStatusCode)
                {
                    var token = await response.Content.ReadAsStringAsync();
                    SetAccessToken(translationOptions, token, "Bearer", edgeCredentials.Uri);
                }

                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (Exception ex)
            {
                ex.ShowDialog("Authentication failed", ex.Message, true);
                return false;
            }
        }

        public static async Task<Xliff> Translate(AccessToken accessToken, PairMapping pairMapping, Xliff sourceXliff)
        {
            try
            {
                var translationRequest = await SendTranslationRequest(accessToken, pairMapping, sourceXliff);
                var translationStatus = await GetTranslationStatus(accessToken, translationRequest.TranslationId);
                if (translationStatus.Error is not null)
                {
                    ErrorHandling.ShowDialog(null, translationStatus.Error.Code, translationStatus.Error.Message);
                    return null;
                }

                await WaitForTranslationCompletion(accessToken, translationRequest.TranslationId);
                var translationResponse = await GetTranslation(accessToken, translationRequest.TranslationId);
                var decodedtTranslationResponse = Base64Decode(translationResponse);
                var translatedXliff = Converter.ParseXliffString(decodedtTranslationResponse);

                return translatedXliff;
            }
            catch
            {
                return null;
            }
        }

        public static async Task UpdateFeedback(AccessToken accessToken, string feedbackId, EdgeFeedbackItem feedbackItem)
        {
            var requestUri = $"{accessToken.BaseUri}api/v2/feedback/{feedbackId}";
            var feedback = feedbackItem.ToKeyValuePairDictionary();
            var content = new FormUrlEncodedContent(feedback);

            var response = await Service.SendRequest(HttpMethod.Put, requestUri, accessToken, content).ConfigureAwait(false);
            if (response.IsSuccessStatusCode) return;

            await HandleError(response);
        }

        public static async Task<bool> VerifyAPI(EdgeCredentials edgeCredentials, ITranslationOptions translationOptions)
        {
            try
            {
                var encodedApiKey = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{edgeCredentials.ApiKey}:"));
                SetAccessToken(translationOptions, encodedApiKey, "Basic", edgeCredentials.Uri);

                var temporaryAccessToken = translationOptions.AccessToken;
                var requestUri = $"{temporaryAccessToken.BaseUri}api/v2/language-pairs";

                var response = await Service.SendRequest(HttpMethod.Get, requestUri, temporaryAccessToken);
                var isSuccessStatusCode = response.IsSuccessStatusCode;
                if (isSuccessStatusCode) return true;

                var responseContent = await response.Content.ReadAsStringAsync();
                ErrorHandling.ShowDialog(null, "Authentication failed", responseContent, true);
                return false;
            }
            catch (Exception ex)
            {
                translationOptions.AccessToken = null;
                ex.ShowDialog("Authentication failed", ex.Message, true);
                return false;
            }
        }

        private static string Base64Decode(this string encodedText)
            => Encoding.UTF8.GetString(Convert.FromBase64String(encodedText));

        private static string Base64Encode(this string text)
            => Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

        private static async Task<string> GetTranslation(AccessToken accessToken, string translationId)
        {
            var requestUri = $"{accessToken.BaseUri}api/v2/translations/{translationId}/download";

            var response = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private static async Task<EdgeTranslationStatus> GetTranslationStatus(AccessToken accessToken, string requestId)
        {
            var requestUri = $"{accessToken.BaseUri}api/v2/translations/{requestId}";

            var response = await Service.SendRequest(HttpMethod.Get, requestUri, accessToken);
            response.EnsureSuccessStatusCode();

            var translationStatus = await response.DeserializeResponse<EdgeTranslationStatus>();
            return translationStatus;
        }

        private static async Task HandleError(HttpResponseMessage response)
        {
            EdgeFeedbackError error;
            try
            {
                error = await response.DeserializeResponse<EdgeFeedbackError>();
            }
            catch
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                throw new Exception($"Code {(int)response.StatusCode}: {response.ReasonPhrase}. Details: {errorDetails}");
            }

            throw new Exception($"Code {error.Error.Code}: {error.Error.Message}. Details: {error.Error.Details}.");
        }

        private static async Task<EdgeTranslationRequestResponse> SendTranslationRequest(AccessToken accessToken, PairMapping pairMapping, Xliff sourceXliff)
        {
            var requestUri = $"{accessToken.BaseUri}api/v2/translations";

            var input = Base64Encode(sourceXliff.ToString());
            var edgeTranslationRequestContent = new EdgeTranslationRequestContent(pairMapping, input);
            var content = new FormUrlEncodedContent(edgeTranslationRequestContent.ToKeyValuePairDictionary());

            var response = await Service.SendRequest(HttpMethod.Post, requestUri, accessToken, content);
            var translationRequestResponse = await response.DeserializeResponse<EdgeTranslationRequestResponse>();

            return translationRequestResponse;
        }

        private static void SetAccessToken(ITranslationOptions translationOptions, string token, string tokenType, Uri edgeUri)
                                                            => translationOptions.AccessToken = new() { Token = token, TokenType = tokenType, BaseUri = edgeUri };

        private static async Task WaitForTranslationCompletion(AccessToken accessToken, string translationId)
        {
            bool isWaiting;
            do
            {
                var translationStatus = await GetTranslationStatus(accessToken, translationId);
                isWaiting = translationStatus.State != "done";
                if (isWaiting)
                {
                    await Task.Delay(1000);
                }
            } while (isWaiting);
        }
    }
}