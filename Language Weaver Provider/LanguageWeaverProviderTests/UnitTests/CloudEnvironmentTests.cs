using LanguageWeaverProvider.Model;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    public class CloudEnvironmentTests
    {
        [Fact]
        public void ShippedDefault_IsProduction()
        {
            Assert.Same(CloudEnvironment.Production, CloudEnvironment.Current);
        }

        // A stale preprod client id once survived in the token-refresh path: sign-in succeeded, refresh failed.
        [Fact]
        public void AuthorizeAndTokenEndpoints_ShareTheSameTenant()
        {
            foreach (var environment in new[] { CloudEnvironment.Production, CloudEnvironment.Uat })
            {
                Assert.StartsWith(environment.Auth0BaseUrl + "/", environment.Auth0AuthorizeUrl);
                Assert.StartsWith(environment.Auth0BaseUrl + "/", environment.Auth0TokenUrl);
            }
        }

        [Fact]
        public void Profiles_DoNotMixHostsFromTheOtherEnvironment()
        {
            var production = CloudEnvironment.Production;
            foreach (var url in ProductionSensitiveUrls(production))
            {
                Assert.DoesNotContain("preprod", url);
                Assert.DoesNotContain("uat", url);
            }

            var uat = CloudEnvironment.Uat;
            // CloudUSUrl is absent by design: there is no US UAT host, so UAT points at the production one.
            Assert.Contains("preprod", uat.Auth0BaseUrl);
            Assert.Contains("preprod", uat.Auth0Audience);
            Assert.Contains("uat", uat.CloudEUUrl);
            Assert.Contains("uat", uat.AccountPortalApiBaseUrl);
            Assert.Contains("uat", uat.AccountPortalUrl);
            Assert.Contains("uat", uat.LanguageWeaverEUPortalUrl);
            Assert.Contains("uat", uat.LanguageCloudAccountsUrl);
            Assert.NotEqual(production.Auth0ClientId, uat.Auth0ClientId);
        }

        private static string[] ProductionSensitiveUrls(CloudEnvironment production)
        {
            return
            [
                production.Auth0BaseUrl,
                production.Auth0Audience,
                production.Auth0RedirectUri,
                production.CloudEUUrl,
                production.CloudUSUrl,
                production.AccountPortalApiBaseUrl,
                production.AccountPortalUrl,
                production.LanguageWeaverEUPortalUrl,
                production.LanguageWeaverUSPortalUrl,
                production.LanguageCloudAccountsUrl
            ];
        }

        [Fact]
        public void LoginUri_ForProduction_IsUnchangedByCentralisingTheHost()
        {
            var config = new CloudAuth0Config(null, "eu");
            var loginUri = config.LoginUri.OriginalString;

            Assert.StartsWith(
                "https://sdl-prod.eu.auth0.com/authorize?audience=https%3A%2F%2Fapi.sdl.com" +
                "&response_type=code&scope=openid%20email%20profile%20offline_access" +
                "&redirect_uri=https%3A%2F%2Fwww.rws.com" +
                "&client_id=F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff&allowsignup=false&state=",
                loginUri);
            Assert.Contains("&code_challenge=", loginUri);
            Assert.EndsWith("&code_challenge_method=S256", loginUri);
            Assert.DoesNotContain("&connection=", loginUri);
        }

        [Fact]
        public void LoginUri_AppendsTheConnectionCode_WhenOneIsSupplied()
        {
            var config = new CloudAuth0Config("some-connection", "eu");

            Assert.EndsWith("&connection=some-connection", config.LoginUri.OriginalString);
        }
    }
}
