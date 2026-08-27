using LanguageWeaverProvider.Model;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the LW Cloud endpoints the plug-in talks to.
    /// <para>
    /// These values used to be written at each call site, which let them drift: a preprod client id was once
    /// left behind in the token-refresh path after everything else had gone back to production, so sign-in
    /// succeeded but the later refresh failed against the wrong tenant. Two things are guarded here - that
    /// the shipped default is Production, and that every value in a profile belongs to the same environment.
    /// </para>
    /// </summary>
    public class CloudEnvironmentTests
    {
        [Fact]
        public void ShippedDefault_IsProduction()
        {
            Assert.Same(CloudEnvironment.Production, CloudEnvironment.Current);
        }

        [Fact]
        public void Production_UsesTheProductionTenantAndItsMatchingCredentials()
        {
            var production = CloudEnvironment.Production;

            Assert.Equal("https://sdl-prod.eu.auth0.com/authorize", production.Auth0AuthorizeUrl);
            Assert.Equal("https://sdl-prod.eu.auth0.com/oauth/token", production.Auth0TokenUrl);
            Assert.Equal("https://api.sdl.com", production.Auth0Audience);
            Assert.Equal("F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff", production.Auth0ClientId);
            Assert.Equal("https://www.rws.com", production.Auth0RedirectUri);
            Assert.Equal("https://api.languageweaver.com/", production.CloudEUUrl);
            Assert.Equal("https://account-portal-api.sdl.com/", production.AccountPortalApiBaseUrl);
            Assert.Equal("https://eu.cloud.trados.com/lc-api/gw-account-web/accounts/", production.LanguageCloudAccountsUrl);
        }

        [Fact]
        public void Uat_UsesThePreprodTenantAndItsMatchingCredentials()
        {
            var uat = CloudEnvironment.Uat;

            Assert.Equal("https://sdl-preprod.eu.auth0.com/authorize", uat.Auth0AuthorizeUrl);
            Assert.Equal("https://sdl-preprod.eu.auth0.com/oauth/token", uat.Auth0TokenUrl);
            Assert.Equal("https://api-preprod.sdl.com", uat.Auth0Audience);
            Assert.Equal("OltQlVmK6N9Y04bNMmFxoXmGleLMmdxB", uat.Auth0ClientId);
            Assert.Equal("https://www.sdl.com", uat.Auth0RedirectUri);
            Assert.Equal("https://uat.api.languageweaver.com/", uat.CloudEUUrl);
            Assert.Equal("https://uat-account-portal-api.sdl.com/", uat.AccountPortalApiBaseUrl);
            Assert.Equal("https://eu.uat-cloud.trados.com/lc-api/gw-account-web/accounts/", uat.LanguageCloudAccountsUrl);
        }

        /// <summary>
        /// The authorize and token endpoints must belong to one tenant: only the tenant that issued an
        /// authorization code can exchange it. This is the invariant that the old per-call-site values broke.
        /// </summary>
        [Fact]
        public void AuthorizeAndTokenEndpoints_ShareTheSameTenant()
        {
            foreach (var environment in new[] { CloudEnvironment.Production, CloudEnvironment.Uat })
            {
                Assert.StartsWith(environment.Auth0BaseUrl + "/", environment.Auth0AuthorizeUrl);
                Assert.StartsWith(environment.Auth0BaseUrl + "/", environment.Auth0TokenUrl);
            }
        }

        /// <summary>
        /// Production must never carry a preprod host, and UAT must never carry a production Auth0 host.
        /// The US Cloud host is exempt: there is no US UAT host, so UAT deliberately points at production.
        /// </summary>
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
            Assert.Contains("preprod", uat.Auth0BaseUrl);
            Assert.Contains("preprod", uat.Auth0Audience);
            Assert.Contains("uat", uat.CloudEUUrl);
            Assert.Contains("uat", uat.AccountPortalApiBaseUrl);
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
                production.LanguageCloudAccountsUrl
            ];
        }

        /// <summary>
        /// The authorize URL is what Auth0 validates, so centralising the host must not have altered a single
        /// character of it. Everything except the random state and code challenge is pinned literally.
        /// </summary>
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
