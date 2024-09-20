using Newtonsoft.Json;
using Sdl.LanguageCloud.IdentityApi;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace StandAloneConsoleApp_PretranslateUsingProvider.LC
{
    internal class LCService
    {

        public bool LoginToLC()
        {
            // Populate the `ClientId`, `ClientSecret`, and `TenantId` fields with actual values.
            // These can be retrieved from the Language Cloud web interface:
            //
            // Navigate to: Users -> Integrations -> Applications -> 
            // Select an application with API access -> API Access tab.
            // Copy the `ClientId` and `ClientSecret` from the selected application.
            //
            // For the TenantID, navigate to: Users -> Manage Account.
            // Copy the 'Trados Account ID' from the web UI.
            var result = LoginAsync(new AuthenticationRequestModel()
            {
                ClientId = "", // Paste the actual ClientId
                ClientSecret = "", // Paste the actual ClientSecret
                Audience = "https://api.sdl.com", // Do not change the value
                GrantType = "client_credentials", // Do not change the value
                TenantId = "" // Paste the actual TenantID
            });

            return result.Result;
        }

        private async Task<bool> LoginAsync(AuthenticationRequestModel model)
        {
            var lcInstance = LanguageCloudIdentityApi.Instance;

            var authData = await GetAuthToken(model);
            var loginData = new LoginData
            {
                AccessToken = authData.AccessToken,
                TenantId = model.TenantId
            };

            return lcInstance.LoginWithToken(loginData);
        }

        private async Task<AuthenticationResponseModel> GetAuthToken(AuthenticationRequestModel model)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://sdl-prod.eu.auth0.com/oauth/token");

            // Prepare form data as KeyValuePairs for the request content
            var collection = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id", model.ClientId),
                new KeyValuePair<string, string>("client_secret", model.ClientSecret),
                new KeyValuePair<string, string>("grant_type", model.GrantType),
                new KeyValuePair<string, string>("audience", model.Audience)
            };

            var content = new FormUrlEncodedContent(collection);
            request.Content = content;

            try
            {
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                // Use Newtonsoft.Json to deserialize the JSON response
                var authToken = JsonConvert.DeserializeObject<AuthenticationResponseModel>(jsonResponse);

                Console.WriteLine($"Access Token: {authToken.AccessToken}");
                return authToken;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request error: {httpEx.Message}");
                return null; // Handle as appropriate
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON Deserialization error: {jsonEx.Message}");
                return null; // Handle as appropriate
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return null; // Handle as appropriate
            }
        }
    }
}
