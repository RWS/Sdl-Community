using System;
using System.Text;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Offline characterization tests for <see cref="EdgeService.EnsureExpiryPopulated"/> — the self-heal that
    /// repairs an Edge Bearer token whose separately-persisted <see cref="AccessToken.ExpiresAt"/> deserialized as
    /// <c>0</c> (e.g. a token saved before expiry parsing existed, or assigned without running <c>SetAccessToken</c>).
    ///
    /// The authoritative expiry is the JWT "exp" claim that travels inside the token, not the cached scalar, so a
    /// live session must NOT be reported as expired just because the cached value is zero. These tests pin that.
    /// </summary>
    public class EdgeTokenExpiryTests
    {
        // The exact EdgeSSO Bearer JWT observed live in the debugger when the false-positive
        // EdgeSessionExpiredException fired: ExpiresAt deserialized as 0 even though the payload's
        // "exp" claim (1782420347 = 2026-06-25T20:45:47Z) was still ~4 hours in the future.
        private const string PinnedEdgeSsoToken =
            "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3ODI0MjAzNDcsInRva2VuIjoiNjQ0ZmMwZTEtOTgyZi00MjAwLWIwNDgtMzBjNGE5MzQwMDZmIn0.VY4uo5ztaN0VGomroxInaE7E5qI0NcVluQjwLNUsPwQ4k7Jxa9gVoJ00BfXJ57viI0ve8Op8BgwSv9oICggjX7VpICJ7jdI-ATCP8Oy5XaVzdq3JvLkSleUziJHncf0PgYEXlhUg96LwnFzM5zYwMIOvzdMlmNZWhBOTzZd3UDIuSZGWydiiOr1szTt1Cct0pZ08B2QHGu1a9MCxvKTgrVBRF1s14ADPjDiUZF-jqT6-ahD-tVEytStEFuQmgQf9bym0ZRJ-z5PeELA5koNUjgqDGd1cR7cio_OKEZOxUMX4zg5Vnp90Ngy4ytdBrNnghjIlTTrE7mvO1_1cqD-e4Q";

        private const long PinnedExpSeconds = 1782420347L;

        // Builds an unsigned 3-part JWT carrying the given "exp". TryGetJwtExpirySeconds only reads the payload and
        // never validates the signature, so this is sufficient to exercise expiry extraction deterministically.
        private static string MakeJwt(long expSeconds)
        {
            string Base64Url(string json)
                => Convert.ToBase64String(Encoding.UTF8.GetBytes(json))
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');

            var header = Base64Url("{\"alg\":\"none\",\"typ\":\"JWT\"}");
            var payload = Base64Url("{\"exp\":" + expSeconds + "}");
            return header + "." + payload + ".sig";
        }

        [Fact]
        public void EnsureExpiryPopulated_PinnedLiveEdgeSsoToken_RepairsZeroExpiryFromJwtClaim()
        {
            // Reproduces the live debugger state: Bearer JWT, but ExpiresAt rehydrated as 0.
            var token = new AccessToken
            {
                Token = PinnedEdgeSsoToken,
                TokenType = "Bearer",
                ExpiresAt = 0,
                ValidityInSeconds = 0
            };

            EdgeService.EnsureExpiryPopulated(token);

            // ExpiresAt is re-derived from the JWT's own "exp" claim (milliseconds), independent of the wall clock.
            // (ValidityInSeconds is wall-clock-relative on a fixed-date token, so it can't be asserted meaningfully
            // here without flaking after the token's exp date — the future-dated test covers that computation instead.)
            Assert.Equal(PinnedExpSeconds * 1000L, token.ExpiresAt);
        }

        [Fact]
        public void EnsureExpiryPopulated_BearerTokenExpiringInFuture_PopulatesPositiveValidity()
        {
            var expSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600;
            var token = new AccessToken
            {
                Token = MakeJwt(expSeconds),
                TokenType = "Bearer",
                ExpiresAt = 0,
                ValidityInSeconds = 0
            };

            EdgeService.EnsureExpiryPopulated(token);

            Assert.Equal(expSeconds * 1000L, token.ExpiresAt);
            Assert.True(token.ValidityInSeconds > 0);
        }

        [Fact]
        public void EnsureExpiryPopulated_AlreadyPopulatedExpiry_IsNotOverwritten()
        {
            var token = new AccessToken
            {
                Token = MakeJwt(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600),
                TokenType = "Bearer",
                ExpiresAt = 12345,
                ValidityInSeconds = 67
            };

            EdgeService.EnsureExpiryPopulated(token);

            Assert.Equal(12345, token.ExpiresAt);
            Assert.Equal(67, token.ValidityInSeconds);
        }

        [Fact]
        public void EnsureExpiryPopulated_BasicApiKeyToken_LeavesExpiryZero()
        {
            // Use a parseable JWT payload but TokenType = "Basic": this proves it is the Bearer guard (not a parse
            // failure) that keeps ExpiresAt at 0. Edge Basic/API-key tokens are opaque and must never be JWT-parsed.
            var token = new AccessToken
            {
                Token = MakeJwt(DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 3600),
                TokenType = "Basic",
                ExpiresAt = 0,
                ValidityInSeconds = 0
            };

            EdgeService.EnsureExpiryPopulated(token);

            Assert.Equal(0, token.ExpiresAt);
            Assert.Equal(0, token.ValidityInSeconds);
        }

        [Fact]
        public void EnsureExpiryPopulated_MalformedBearerToken_LeavesExpiryZero()
        {
            var token = new AccessToken
            {
                Token = "not-a-valid-jwt",
                TokenType = "Bearer",
                ExpiresAt = 0,
                ValidityInSeconds = 0
            };

            EdgeService.EnsureExpiryPopulated(token);

            Assert.Equal(0, token.ExpiresAt);
        }

        [Fact]
        public void EnsureExpiryPopulated_NullToken_DoesNotThrow()
        {
            var exception = Record.Exception(() => EdgeService.EnsureExpiryPopulated(null));
            Assert.Null(exception);
        }
    }
}
