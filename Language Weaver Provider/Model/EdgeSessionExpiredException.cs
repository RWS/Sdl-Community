using System;

namespace LanguageWeaverProvider.Model
{
    /// <summary>
    /// Thrown when a Language Weaver Edge SSO session token has expired.
    /// Edge SSO tokens are issued through an interactive WebView2 SAML flow and carry no refresh token,
    /// so they cannot be renewed silently. Callers should surface a clear, actionable message asking the
    /// user to sign in again instead of sending a request the server will reject with 401 Unauthorized.
    /// </summary>
    public class EdgeSessionExpiredException : Exception
    {
        public EdgeSessionExpiredException(string message) : base(message)
        {
        }
    }
}
