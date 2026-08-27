using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.CohereSubscription.Workflow.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the mapping from the account-portal details response onto the pop-up decision inputs.
    /// The <c>trialStatus</c> values used here are the ones the details endpoint documents
    /// (NOT_STARTED / IN_PROGRESS / CANCELLED / EXPIRED / ENDED), not invented labels.
    /// </summary>
    public class CohereSubscriptionWorkflowTests
    {
        [Fact]
        public void ActiveProSubscription_IsMappedAsPaid()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = true,
                TrialStatus = "NOT_STARTED"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsPaid);
            Assert.False(data.IsTrial);
            // No role was supplied, so it is unknown rather than known-to-be-non-admin.
            Assert.True(data.IsAdmin);
        }

        [Fact]
        public void PaidAfterTrialConversion_IsStillPaid()
        {
            // Converting to paid cancels the trial, so this is the ordinary state of a paying customer.
            // Reading it as an ended trial would prompt someone who has already bought the add-on, and the
            // CANCELLED suppression must not swallow them either - hence its !IsProActive guard.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = true,
                TrialStatus = "CANCELLED"
            });

            Assert.NotNull(data);
            Assert.True(data.IsPaid);
            Assert.True(data.IsCohereDetected);
        }

        [Fact]
        public void TrialInProgress_IsMappedAsAnActiveTrial()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "IN_PROGRESS"
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.False(data.IsTrialExpired);
            Assert.False(data.IsPaid);
        }

        [Theory]
        [InlineData("EXPIRED")]
        [InlineData("ENDED")]
        public void TrialThatRanItsCourse_IsMappedAsEnded(string trialStatus)
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = trialStatus
            });

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsTrial);
            Assert.True(data.IsTrialExpired);
            Assert.False(data.IsPaid);
        }

        [Fact]
        public void DeliberateCancellation_ShowsNoPromptAtAll()
        {
            // Pinned from a live UAT response for an account that cancelled:
            // {"accountId":1227,"trialStatus":"CANCELLED","groupId":"...","isProActive":null}
            // CANCELLED means somebody chose to stop - a trial or a paid subscription, and the endpoint gives
            // nothing to tell those apart. Either way the decision was deliberate, so we do not sell back to
            // them. A null result renders no prompt.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "CANCELLED",
                IsProActive = false // what JSON null deserialises to
            });

            Assert.Null(data);
        }

        [Fact]
        public void ACancelledAccount_IsNeverOfferedAFreeTrial()
        {
            // Guards the reason the suppression is an explicit early return rather than simply dropping
            // CANCELLED from the terminal set: that would read as "never had Cohere" and offer a 14-day free
            // trial to someone who just cancelled one.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "CANCELLED"
            });

            Assert.Null(data);
        }

        [Fact]
        public void NoTrialAndNoPro_IsMappedAsNotDetected()
        {
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                IsProActive = false,
                TrialStatus = "NOT_STARTED"
            });

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsPaid);
            Assert.False(data.IsTrial);
        }

        [Fact]
        public void NeverTrialed_WithoutProPairs_IsOfferedTheTrial()
        {
            // No Account Portal record means no trial has ever been started, so this is the case the prompt
            // exists for. The trial flags are false by knowledge, not by ignorance.
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: false, userRole: "ADMIN");

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsPaid);
            Assert.False(data.IsTrial);
            Assert.False(data.IsTrialExpired);
        }

        [Fact]
        public void NeverTrialed_WithProPairs_IsPaid()
        {
            // An account can hold Pro without ever trialing, so Pro pairs still mean paid here.
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: true, userRole: "ADMIN");

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsPaid);
            Assert.False(data.IsTrial);
        }

        [Theory]
        [InlineData("ADMIN")]
        [InlineData("admin")]
        public void AdminRole_IsRecognisedOnTheNeverTrialedPath(string userRole)
        {
            Assert.True(CohereSubscriptionWorkflow.MapEntitlement(false, userRole).IsAdmin);
        }

        [Theory]
        [InlineData("USER")]
        public void NonAdminRole_IsNotAdminOnTheNeverTrialedPath(string userRole)
        {
            Assert.False(CohereSubscriptionWorkflow.MapEntitlement(false, userRole).IsAdmin);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void UnreadableRole_IsAdminOnTheNeverTrialedPath(string userRole)
        {
            Assert.True(CohereSubscriptionWorkflow.MapEntitlement(false, userRole).IsAdmin);
        }

        [Theory]
        [InlineData("ADMIN")]
        [InlineData("admin")]
        public void AdminRole_IsRecognisedRegardlessOfCasing(string userRole)
        {
            var data = CohereSubscriptionWorkflow.MapDetails(
                new LanguageWeaverDetails { TrialStatus = "NOT_STARTED" }, userRole);

            Assert.True(data.IsAdmin);
        }

        [Theory]
        [InlineData("USER")]
        public void NonAdminRole_IsNotAdmin(string userRole)
        {
            // A role that was actually read and is not ADMIN must never read as admin: those users would
            // otherwise be offered account-management actions they cannot perform.
            var data = CohereSubscriptionWorkflow.MapDetails(
                new LanguageWeaverDetails { TrialStatus = "NOT_STARTED" }, userRole);

            Assert.False(data.IsAdmin);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void UnreadableRole_IsTreatedAsAdmin(string userRole)
        {
            // An account never provisioned into Language Weaver has no readable role from any source:
            // v4/accounts/users/self answers 403 "user ... does not exist", and neither the account-web body
            // nor the sign-in JWT carries a role. Reading that absence as non-admin made the admin
            // "start a trial" prompt unreachable for the never-trialed accounts it is written for, and told
            // account owners to ask an administrator who does not exist.
            var data = CohereSubscriptionWorkflow.MapDetails(
                new LanguageWeaverDetails { TrialStatus = "NOT_STARTED" }, userRole);

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsPaid);
            Assert.True(data.IsAdmin);
        }

        [Fact]
        public void MissingTrialStatus_IsMappedAsNotDetected()
        {
            // The endpoint returns NOT_STARTED defaults when no details exist; a null status must not be
            // read as a trial in any state.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails());

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsTrial);
            Assert.False(data.IsTrialExpired);
        }
    }
}
