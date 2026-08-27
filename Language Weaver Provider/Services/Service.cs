using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LanguageWeaverProvider.Services
{
    internal static class Service
    {
        public static async Task<HttpResponseMessage> SendRequest(HttpMethod httpMethod, string requestUri, AccessToken accessToken = null, HttpContent content = null)
        {
            var request = new HttpRequestMessage(httpMethod, requestUri) { Content = content };
            if (accessToken is not null)
            {
                request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");
            }

            var httpClient = GetHttpClient();
            var response = await httpClient.SendAsync(request).ConfigureAwait(false);
            return response;
        }

        public static async Task<T> DeserializeResponse<T>(this HttpResponseMessage httpResponseMessage,
            string property = null, int index = -1)
        {
            var content = await httpResponseMessage.Content.ReadAsStringAsync();
            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                // Surface HTTP failures (e.g., 401 "Not Authorized") as a clear error instead of letting the
                // non-JSON body fall through to the deserializer, which throws a misleading "Error parsing NaN value".
                throw new HttpRequestException($"{(int)httpResponseMessage.StatusCode} {httpResponseMessage.ReasonPhrase}: {content}");
            }

            if (!string.IsNullOrEmpty(property))
            {
                content = index != -1
                    ? JObject.Parse(content)[property][index].ToString()
                    : JObject.Parse(content)[property].ToString();
            }

            var deserializedObject = JsonConvert.DeserializeObject<T>(content);
            return deserializedObject;
        }

        public static async Task ValidateAndUpdateTokenAsync(ITranslationOptions translationOptions, Action? onValidated, bool showErrors = false)
        {
            try
            {
                var result = await ValidateTokenAsync(translationOptions, showErrors);
                if (result)
                    onValidated?.Invoke();
            }
            catch (EdgeSessionExpiredException)
            {
                // Background validation (e.g. during provider creation) cannot prompt an interactive sign-in.
                // Swallow here so the expired EdgeSSO session is reported at the point of use (translation),
                // not as an unobserved task exception from this fire-and-forget call.
            }
        }

        public static async Task<bool> ValidateTokenAsync(ITranslationOptions translationOptions, bool showErrors = true)
        {
            if (translationOptions.AccessToken is null)
            {
                // A persisted EdgeSSO provider with no token has never completed (or has lost) its interactive
                // sign-in. There is no token to refresh and no silent renewal path, so treat it exactly like an
                // expired session: signal the batch path to prompt re-sign-in and abort, instead of returning
                // false and letting translation proceed with a null token (a confusing NRE deep in EdgeService).
                if (translationOptions.PluginVersion == PluginVersion.LanguageWeaverEdge
                 && translationOptions.AuthenticationType == AuthenticationType.EdgeSSO)
                {
                    throw new EdgeSessionExpiredException("Your Language Weaver Edge session has expired. Please sign in again to continue translating.");
                }

                return false;
            }

            if (
                translationOptions.AuthenticationType == AuthenticationType.CloudSSO
             && IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
            {
                return await CloudService.RefreshAuth0Token(translationOptions);
            }

            // The Trados sign-in token is owned and renewed by Studio, so there is no refresh token the plug-in
            // may present. Re-read the current token instead: Studio has usually already renewed it in the
            // background, and only when the fresh token is also expired must the user be prompted.
            if (translationOptions.AuthenticationType == AuthenticationType.CloudStudio
             && IsTimestampExpired(translationOptions.AccessToken.ExpiresAt))
            {
                var refreshedToken = StudioIdentityService.GetCurrentToken();
                if (refreshedToken is null || IsTimestampExpired(StudioIdentityService.GetExpiryFromToken(refreshedToken)))
                {
                    return false;
                }

                translationOptions.AccessToken.Token = refreshedToken;
                translationOptions.AccessToken.ExpiresAt = StudioIdentityService.GetExpiryFromToken(refreshedToken);
                return true;
            }

            if (translationOptions.PluginVersion == PluginVersion.LanguageWeaverCloud
             && translationOptions.AuthenticationType != AuthenticationType.CloudSSO
             && translationOptions.AuthenticationType != AuthenticationType.CloudStudio
             && IsTimestampExpired(translationOptions.AccessToken?.ExpiresAt))
            {
                return await CloudService.AuthenticateUser(translationOptions, translationOptions.AuthenticationType, showErrors);
            }

            // EdgeApiKey is excluded: it uses HTTP Basic with the API key itself as the credential, so there is no expiry to refresh.
            if (translationOptions.PluginVersion == PluginVersion.LanguageWeaverEdge
             && translationOptions.AuthenticationType == AuthenticationType.EdgeCredentials
             && translationOptions.EdgeCredentials is not null
             && IsTimestampExpired(translationOptions.AccessToken?.ExpiresAt))
            {
                return await EdgeService.AuthenticateUser(translationOptions.EdgeCredentials, translationOptions);
            }

            // EdgeSSO cannot be refreshed silently: its token is issued through an interactive WebView2 SAML
            // flow and carries no refresh token, so renewal requires a UI context that ValidateTokenAsync cannot supply.
            // Fail fast with an actionable message instead of sending an expired Bearer token that the server rejects with 401.
            if (translationOptions.PluginVersion == PluginVersion.LanguageWeaverEdge
             && translationOptions.AuthenticationType == AuthenticationType.EdgeSSO
             && IsTimestampExpired(translationOptions.AccessToken?.ExpiresAt))
            {
                throw new EdgeSessionExpiredException("Your Language Weaver Edge session has expired. Please sign in again to continue translating.");
            }

            return false;
        }

        private static bool IsTimestampExpired(double? unixTimeStamp)
        {
            // Treat missing or non-positive timestamps as expired so that a token with an unset ExpiresAt
            // forces a refresh rather than silently bypassing validation.
            if (!unixTimeStamp.HasValue || unixTimeStamp.Value <= 0)
            {
                return true;
            }

            var expirationTime = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).AddMilliseconds(unixTimeStamp.Value);
            var currentTime = DateTimeOffset.UtcNow.AddHours(-1);

            return expirationTime <= currentTime;
        }

        public static HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(Constants.TraceAppKey, Constants.TraceAppValue);
            httpClient.DefaultRequestHeaders.Add(Constants.TraceAppVersionKey, ApplicationInitializer.CurrentAppVersion);
            return httpClient;
        }
    }
}