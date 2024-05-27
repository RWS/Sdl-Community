using Newtonsoft.Json;
using Sdl.LanguageCloud.IdentityApi;
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
            var configXMLFile = XDocument.Load("SDLTradosStudio.exe.config");
            var audience = configXMLFile.XPathSelectElement("//auth0/setting[@name='audience']").Attribute("value").Value.ToString();

            return LoginAsync(new AuthenticationRequestModel
            {
                ClientSecret = "",
                ClientId = "",
                GrantType = "http://auth0.com/oauth/grant-type/password-realm",
                Scope = "openid email profile offline_access",
                Realm = "oos",
                Audience = audience,
                Password = "",
                Username = "",
                TenantId = ""
            }).Result;
        }

        private async Task<bool> LoginAsync(AuthenticationRequestModel model)
        {
            var lcInstance = LanguageCloudIdentityApi.Instance;
            var authData = await GetAuthToken(model);
            var loginData = new LoginData
            {
                AccessToken = authData.Access_token,
                RefreshToken = authData.Refresh_token,
                IdToken = authData.Id_token,
                TenantId = model.TenantId
            };

            return lcInstance.LoginWithToken(loginData);
        }

        private async Task<AuthenticationResponseModel> GetAuthToken(AuthenticationRequestModel model)
        {
            var payload = new AuthenticationRequestModel
            {
                Audience = model.Audience,
                GrantType = "http://auth0.com/oauth/grant-type/password-realm",
                Scope = "openid email profile offline_access",
                Realm = "oos",
                ClientSecret = model.ClientSecret,
                ClientId = model.ClientId,
                Username = model.Username,
                Password = model.Password
            };

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage bearerResponse;
                if (payload.Audience.ToLower().Contains("preprod"))
                {
                    bearerResponse = await client.PostAsync("https://sdl-preprod.eu.auth0.com:443/oauth/token", content);
                }
                else
                {
                    bearerResponse = await client.PostAsync("https://sdl-preprod.eu.auth0.com", content);
                }

                var responseContent = await bearerResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<AuthenticationResponseModel>(responseContent);
            }
        }
    }
}
