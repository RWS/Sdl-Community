using LanguageWeaverProvider;
using LanguageWeaverProvider.Model;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins where the prompt's call-to-action sends an administrator.
    /// <para>
    /// Both actions are performed from the account's own dashboard, so both links point there: Account Portal
    /// for a provisioned account, the Language Weaver portal otherwise. Deeper links were tried and dropped -
    /// opening one externally makes Account Portal re-run its auth bootstrap, which discards the requested
    /// path and lands on the dashboard anyway, even with a live session. Verified against the add-on page in
    /// UAT, where the URL handed to the browser was confirmed correct yet still redirected to the dashboard.
    /// </para>
    /// </summary>
    public class SubscriptionCallToActionUriTests
    {
        private const string BusinessAccountId = "6a8d8867e3ba8a1312c63ea7";

        private static string TenantUri => Constants.AccountPortalTenantUrl + BusinessAccountId;

        [Fact]
        public void AccountPortalAccount_IsLinkedToItsOwnTenantPage()
        {
            Assert.StartsWith(CloudEnvironment.Current.AccountPortalUrl, TenantUri);
            Assert.EndsWith("/t/" + BusinessAccountId, TenantUri);
        }

        [Fact]
        public void TheTenantLink_CarriesNoDeepPath()
        {
            // A deep link cannot survive external entry, so the tenant page is the whole destination.
            Assert.DoesNotContain("/subscriptions/", TenantUri);
            Assert.DoesNotContain("/addons", TenantUri);
        }

        [Fact]
        public void AccountWithoutAnAccountPortalRecord_IsLinkedToTheLanguageWeaverPortal()
        {
            // Nothing to link to in Account Portal, so the Language Weaver portal account page is used.
            Assert.StartsWith(
                CloudEnvironment.Current.LanguageWeaverEUPortalUrl,
                Constants.LanguageWeaverProStartTrialUrl);
            Assert.DoesNotContain("account.rws.com", Constants.LanguageWeaverProStartTrialUrl);
        }

        [Fact]
        public void TheTwoDestinations_AreDistinct()
        {
            Assert.NotEqual(TenantUri, Constants.LanguageWeaverProStartTrialUrl);
        }
    }
}
