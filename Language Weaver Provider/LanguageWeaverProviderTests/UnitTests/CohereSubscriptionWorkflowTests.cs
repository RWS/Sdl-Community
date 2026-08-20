using LanguageWeaverProvider.CohereSubscription.Workflow.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the mapping from the two observable facts — whether the account holds Pro (GENERICPLUS) language
    /// pairs, and the signed-in user's role — onto the prompt's decision inputs.
    /// </summary>
    public class CohereSubscriptionWorkflowTests
    {
        [Fact]
        public void ProLanguagePairs_AreMappedAsPaid()
        {
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: true, userRole: "ADMIN");

            Assert.True(data.IsPaid);
            Assert.True(data.IsCohereDetected);
        }

        [Fact]
        public void NoProLanguagePairs_AreMappedAsNotDetected()
        {
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: false, userRole: "ADMIN");

            Assert.False(data.IsPaid);
            Assert.False(data.IsCohereDetected);
        }

        [Theory]
        [InlineData("ADMIN")]
        [InlineData("admin")]
        public void AdminRole_IsRecognisedRegardlessOfCasing(string userRole)
        {
            Assert.True(CohereSubscriptionWorkflow.MapEntitlement(false, userRole).IsAdmin);
        }

        [Theory]
        [InlineData("USER")]
        [InlineData("")]
        [InlineData(null)]
        public void NonAdminOrMissingRole_IsNotAdmin(string userRole)
        {
            // A missing role must never read as admin: API-credential logins identify an application rather
            // than a person and carry no role, and those users must not be offered account-management actions.
            Assert.False(CohereSubscriptionWorkflow.MapEntitlement(true, userRole).IsAdmin);
        }

        [Fact]
        public void TrialState_IsNeverAsserted()
        {
            // Documents a known gap rather than desired behaviour: no source currently exposes trial state,
            // so both flags stay false and the two trial cases collapse into the neighbouring ones.
            var entitled = CohereSubscriptionWorkflow.MapEntitlement(true, "ADMIN");
            var notEntitled = CohereSubscriptionWorkflow.MapEntitlement(false, "ADMIN");

            Assert.False(entitled.IsTrial);
            Assert.False(entitled.IsTrialExpired);
            Assert.False(notEntitled.IsTrial);
            Assert.False(notEntitled.IsTrialExpired);
        }
    }
}
