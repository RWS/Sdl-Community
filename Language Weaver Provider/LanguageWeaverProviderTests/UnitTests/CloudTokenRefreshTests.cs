using System;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// CRQ-46950: every Cloud request is built as <c>v4/accounts/{AccountId}/…</c> from the access token, so a token
    /// renewed through Auth0 must keep the account it was issued for. The Auth0 refresh response carries no account.
    /// </summary>
    public class CloudTokenRefreshTests
    {
        // Shape of the Auth0 /oauth/token response to a refresh_token grant without rotation: no refresh_token.
        private const string Auth0RefreshResponse =
            "{\"access_token\":\"renewed\",\"id_token\":\"id\",\"scope\":\"openid profile offline_access\",\"expires_in\":86400,\"token_type\":\"Bearer\"}";

        [Fact]
        public void MapRefreshedToken_KeepsTheAccountOfTheExpiredToken()
        {
            var expired = new AccessToken
            {
                Token = "expired",
                TokenType = "Bearer",
                RefreshToken = "refresh",
                AccountId = "account-42",
                BaseUri = new Uri("https://api.languageweaver.com/")
            };

            var renewed = CloudService.MapRefreshedToken(Auth0RefreshResponse, expired);

            Assert.Equal("renewed", renewed.Token);
            Assert.Equal("account-42", renewed.AccountId);
        }
    }
}
