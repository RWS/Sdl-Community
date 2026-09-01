using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.CohereSubscription.Workflow.Services;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// Pins the mapping from the account-portal details response onto the pop-up decision inputs.
    /// The <c>trialStatus</c> values used here are the four the service confirmed it returns
    /// (NOT_STARTED / IN_PROGRESS / CANCELLED / ENDED), not invented labels.
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

        [Fact]
        public void TrialThatRanItsCourse_IsMappedAsEnded()
        {
            // ENDED is the only terminal status that is not a deliberate stop: the service confirmed the
            // vocabulary is NOT_STARTED, IN_PROGRESS, CANCELLED, ENDED.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "ENDED"
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
            //
            // This also guards why the suppression is an explicit early return rather than simply dropping
            // CANCELLED from the terminal set: that would read as "never had Cohere" and offer a 14-day free
            // trial to someone who just cancelled one.
            var data = CohereSubscriptionWorkflow.MapDetails(new LanguageWeaverDetails
            {
                TrialStatus = "CANCELLED",
                IsProActive = false // what JSON null deserialises to
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
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: false);

            Assert.False(data.IsCohereDetected);
            Assert.False(data.IsPaid);
            Assert.False(data.IsTrial);
            Assert.False(data.IsTrialExpired);
        }

        [Fact]
        public void NeverTrialed_WithProPairs_IsPaid()
        {
            // An account can hold Pro without ever trialing, so Pro pairs still mean paid here.
            var data = CohereSubscriptionWorkflow.MapEntitlement(hasProLanguagePairs: true);

            Assert.True(data.IsCohereDetected);
            Assert.True(data.IsPaid);
            Assert.False(data.IsTrial);
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
