using Newtonsoft.Json;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Model
{
    /// <summary>
    /// Subset of the Language Cloud account-web response
    /// (<c>GET {lcHost}/lc-api/gw-account-web/accounts/{tradosAccountId}</c>) that this feature needs.
    /// </summary>
    public class LanguageCloudAccount
    {
        /// <summary>
        /// Account Portal identifier for the Trados account. Null when the account was not provisioned
        /// through Account Portal, in which case its Language Weaver entitlement cannot be looked up.
        /// </summary>
        [JsonProperty("businessAccountId")]
        public string BusinessAccountId { get; set; }
    }
}
