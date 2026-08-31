using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using Newtonsoft.Json;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the licence vocabulary that decides eligibility for Language Weaver Pro.
    ///
    /// Both literals were read off live UAT accounts under the debugger rather than derived from a naming
    /// convention, because there is no convention to derive: Go reports <c>trados_go</c> while Freelance
    /// reports <c>trados_live_freelance</c>. Guessing <c>trados_live_go</c> would have been wrong.
    /// </summary>
    public class ProductOfferingEligibilityTests
    {
        [Fact]
        public void TheGoLicence_IsReadFromTheAccountResponse()
        {
            // Captured from a live Trados Go account.
            const string json = "{\"account\":{\"businessAccountId\":\"6a91742ae3ba8a1312c63ec8\","
                              + "\"productOffering\":\"trados_go\",\"accountType\":\"ENTERPRISE\"}}";

            var account = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(json);

            Assert.Equal("trados_go", account.Account.ProductOffering);
        }

        [Fact]
        public void TheFreelanceLicence_IsReadFromTheAccountResponse()
        {
            // Captured from a live Trados Freelance account. Note the extra "live" segment, absent from Go.
            const string json = "{\"account\":{\"businessAccountId\":\"6a91742ae3ba8a1312c63ec8\","
                              + "\"productOffering\":\"trados_live_freelance\",\"accountType\":\"ENTERPRISE\"}}";

            var account = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(json);

            Assert.Equal("trados_live_freelance", account.Account.ProductOffering);
        }

        [Fact]
        public void AccountType_CannotIdentifyTheLicence()
        {
            // Go, Freelance and Enterprise accounts all report accountType ENTERPRISE, which is why
            // eligibility reads productOffering instead. This test exists to stop anyone reaching for
            // accountType again: it would silently exclude every Go and Freelance user.
            const string go = "{\"account\":{\"productOffering\":\"trados_go\",\"accountType\":\"ENTERPRISE\"}}";
            const string freelance = "{\"account\":{\"productOffering\":\"trados_live_freelance\",\"accountType\":\"ENTERPRISE\"}}";

            var goAccount = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(go);
            var freelanceAccount = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(freelance);

            Assert.NotEqual(goAccount.Account.ProductOffering, freelanceAccount.Account.ProductOffering);
        }

        [Fact]
        public void AnAbsentLicence_IsNull()
        {
            // An unreadable account yields no licence, which the workflow treats as ineligible: eligibility
            // must be positively established rather than assumed.
            var account = JsonConvert.DeserializeObject<LanguageCloudAccountResponse>(
                "{\"account\":{\"businessAccountId\":\"abc\"}}");

            Assert.Null(account.Account.ProductOffering);
        }
    }
}
