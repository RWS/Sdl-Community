using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using Newtonsoft.Json;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the shape of the account-web response. The account object is nested under an "account" property;
    /// deserializing the body straight into <see cref="LanguageCloudAccount"/> yields an empty object whose
    /// null BusinessAccountId reads as "not provisioned through Account Portal" for every account, which
    /// silently disables the whole trial branch.
    /// </summary>
    public class LanguageCloudAccountResponseTests
    {
        // Trimmed from a live GET /lc-api/gw-account-web/accounts/{id} response.
        private const string LiveResponse =
            "{\"account\":{\"id\":\"53c38504df93bc162367bd54\",\"companyName\":\"SDL\"," +
            "\"oosAccountId\":\"84458\",\"businessAccountId\":\"6a69bcd2c2e2163a8f392e5f\"," +
            "\"businessSubscriptionId\":null,\"accountType\":\"ENTERPRISE\",\"status\":\"ACTIVE\"}}";

        [Fact]
        public void BusinessAccountId_IsReadFromInsideTheAccountEnvelope()
        {
            var response = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(LiveResponse);

            Assert.NotNull(response.Account);
            Assert.Equal("6a69bcd2c2e2163a8f392e5f", response.Account.BusinessAccountId);
        }

        [Fact]
        public void NullBusinessAccountId_StillDeserializesTheAccount()
        {
            // An account that has never started a trial: the envelope and account are present, the id is not.
            var response = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(
                "{\"account\":{\"id\":\"abc\",\"businessAccountId\":null}}");

            Assert.NotNull(response.Account);
            Assert.Null(response.Account.BusinessAccountId);
        }

        [Fact]
        public void FlatDeserialization_LosesTheField()
        {
            // Guards the regression directly: mapping the body onto the account type finds nothing.
            var flat = JsonConvert.DeserializeObject<LanguageCloudAccount>(LiveResponse);

            Assert.Null(flat.BusinessAccountId);
        }
    }
}
