using System;
using LanguageWeaverProvider.Model;
using NLog;
using Sdl.LanguageCloud.IdentityApi;

namespace LanguageWeaverProvider.Services
{
    /// <summary>
    /// Wraps the RWS ID session that Trados Studio already holds, so the plug-in can authenticate against
    /// Language Weaver without running a second interactive sign-in.
    /// <para>
    /// Studio owns this token and its renewal: there is no refresh token the plug-in may use, so an expired
    /// token is handled by re-reading the current one from Studio rather than by refreshing it.
    /// </para>
    /// </summary>
    public static class StudioIdentityService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns the token of the current Trados sign-in, or <c>null</c> when Studio has no usable session.
        /// </summary>
        public static string GetCurrentToken()
        {
            try
            {
                var identity = LanguageCloudIdentityApi.Instance;
                var token = identity?.AccessToken;
                return string.IsNullOrWhiteSpace(token) ? null : token;
            }
            catch (Exception ex)
            {
                // The identity assembly ships with Studio, so a version mismatch surfaces here as a load or
                // binding failure. Treat it as "no session" so the caller degrades instead of crashing.
                Logger.Log(LogLevel.Warn, $"Could not read the Trados sign-in session: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Returns the display name of the signed-in user, preferring the account name over the email address.
        /// Returns <c>null</c> when unavailable.
        /// </summary>
        public static string GetDisplayName()
        {
            try
            {
                var credential = LanguageCloudIdentityApi.Instance?.LanguageCloudCredential;
                if (credential is null)
                {
                    return null;
                }

                return !string.IsNullOrWhiteSpace(credential.Email) ? credential.Email
                     : !string.IsNullOrWhiteSpace(credential.AccountName) ? credential.AccountName
                     : null;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, $"Could not read the Trados sign-in details: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Builds an <see cref="AccessToken"/> for <paramref name="region"/> from the current Trados sign-in,
        /// or returns <c>null</c> when there is no session to reuse. The account id is not populated here.
        /// </summary>
        public static AccessToken CreateAccessToken(string region)
        {
            var token = GetCurrentToken();
            if (token is null)
            {
                return null;
            }

            return new AccessToken
            {
                Token = token,
                TokenType = "Bearer",
                // Deliberately no RefreshToken: renewal belongs to Studio, and presenting one here would let
                // the plug-in try to refresh a session it does not own.
                ExpiresAt = GetExpiryFromToken(token),
                BaseUri = new Uri(region)
            };
        }

        /// <summary>
        /// Reads the <c>exp</c> claim of a JWT and converts it to the Unix epoch milliseconds used by
        /// <see cref="AccessToken.ExpiresAt"/>. Returns <c>0</c> when the claim cannot be read, which the
        /// token validation treats as expired and therefore forces a re-read from Studio.
        /// </summary>
        public static long GetExpiryFromToken(string token)
        {
            // "exp" is expressed in seconds; AccessToken.ExpiresAt is in milliseconds.
            return EdgeService.TryGetJwtExpirySeconds(token) * 1000L;
        }
    }
}
