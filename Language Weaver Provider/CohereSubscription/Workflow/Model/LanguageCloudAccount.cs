using Newtonsoft.Json;

namespace LanguageWeaverProvider.CohereSubscription.Workflow.Model
{
    /// <summary>
    /// Envelope for the account-web response. The account object is nested under an <c>account</c> property:
    /// <c>{"account":{"id":"…","businessAccountId":"…"}}</c>. Deserializing the body straight into
    /// <see cref="LanguageCloudAccount"/> silently yields an empty object, which reads as "not provisioned
    /// through Account Portal" for every account.
    /// </summary>
    public class LanguageCloudAccountResponse
    {
        [JsonProperty("account")]
        public LanguageCloudAccount Account { get; set; }
    }

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

        /// <summary>
        /// Account Portal identifier for the account's subscription. Can be null even on an active enterprise
        /// account, so callers must cope with its absence rather than assume a provisioned account always has
        /// one.
        /// </summary>
        [JsonProperty("businessSubscriptionId")]
        public string BusinessSubscriptionId { get; set; }
    }
}
