namespace LanguageWeaverProvider.Model
{
    /// <summary>
    /// Single point of control for the LW Cloud endpoints the plug-in talks to: the Auth0 tenant used for
    /// Cloud sign-in, the Cloud API region hosts, and the Account Portal / Language Cloud hosts behind the
    /// Cohere entitlement lookup. Language Weaver Edge is unaffected - it is reached through a
    /// user-supplied host, so it has no environment to switch.
    /// <para>
    /// These endpoints have to move together: the Auth0 tenant that issues an authorization code is the only
    /// one that can exchange it for a token, and the audience, client id and callback all have to belong to
    /// that same tenant. Previously each value was written at its own call site, which let them drift out of
    /// step - a preprod client id was once left behind in the token-refresh path after everything else had
    /// been put back to production, so sign-in worked but refreshing the token later did not.
    /// </para>
    /// <para>
    /// To switch environments, change <see cref="Current"/> and nothing else.
    /// </para>
    /// </summary>
    public class CloudEnvironment
    {
        public static CloudEnvironment Production { get; } = new CloudEnvironment
        {
            Name = "Production",
            Auth0BaseUrl = "https://sdl-prod.eu.auth0.com",
            Auth0Audience = "https://api.sdl.com",
            Auth0ClientId = "F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff",
            Auth0RedirectUri = "https://www.rws.com",
            CloudEUUrl = "https://api.languageweaver.com/",
            CloudUSUrl = "https://us.api.languageweaver.com/",
            AccountPortalApiBaseUrl = "https://account-portal-api.sdl.com/",
            LanguageCloudAccountsUrl = "https://eu.cloud.trados.com/lc-api/gw-account-web/accounts/"
        };

        /// <summary>
        /// UAT. The Auth0 values are Studio's own preprod ones taken from SDLTradosStudio.exe.config;
        /// https://www.sdl.com is the callback registered for that client. There is no US UAT host, so
        /// <see cref="CloudUSUrl"/> stays on production and only the EU region can be exercised here.
        /// </summary>
        public static CloudEnvironment Uat { get; } = new CloudEnvironment
        {
            Name = "UAT",
            Auth0BaseUrl = "https://sdl-preprod.eu.auth0.com",
            Auth0Audience = "https://api-preprod.sdl.com",
            Auth0ClientId = "OltQlVmK6N9Y04bNMmFxoXmGleLMmdxB",
            Auth0RedirectUri = "https://www.sdl.com",
            CloudEUUrl = "https://uat.api.languageweaver.com/",
            CloudUSUrl = "https://us.api.languageweaver.com/",
            AccountPortalApiBaseUrl = "https://uat-account-portal-api.sdl.com/",
            LanguageCloudAccountsUrl = "https://eu.uat-cloud.trados.com/lc-api/gw-account-web/accounts/"
        };

        private CloudEnvironment()
        {
        }

        /// <summary>
        /// The Cloud environment the plug-in runs against. This is the only line to edit when switching.
        /// <para>
        /// Declared after the profiles on purpose: static field initializers run in textual order, so putting
        /// this first would read Production before it was assigned and leave every endpoint null.
        /// </para>
        /// </summary>
        public static CloudEnvironment Current { get; set; } = Production;

        public string Name { get; private set; }

        public string Auth0BaseUrl { get; private set; }

        public string Auth0Audience { get; private set; }

        public string Auth0ClientId { get; private set; }

        public string Auth0RedirectUri { get; private set; }

        public string Auth0TokenUrl => $"{Auth0BaseUrl}/oauth/token";

        public string Auth0AuthorizeUrl => $"{Auth0BaseUrl}/authorize";

        public string CloudEUUrl { get; private set; }

        public string CloudUSUrl { get; private set; }

        public string AccountPortalApiBaseUrl { get; private set; }

        public string LanguageCloudAccountsUrl { get; private set; }
    }
}
