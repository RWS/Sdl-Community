using LanguageWeaverProvider;
using LanguageWeaverProvider.CohereSubscription.Decision.Services;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    public class LearnMoreLinkTests
    {
        private static CohereSubscriptionData NotDetectedFor(bool isAdmin) => new CohereSubscriptionData
        {
            IsCohereDetected = false,
            IsPaid = false,
            IsAdmin = isAdmin
        };

        [Fact]
        public void TheAdminPrompt_CarriesALearnMoreLink()
        {
            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(NotDetectedFor(isAdmin: true));

            Assert.Equal(Constants.LanguageWeaverProLearnMoreUrl, viewModel.LearnMoreUri);
        }

        [Fact]
        public void TheNonAdminPrompt_HasNoLinkBecauseItAlreadyHasTheButton()
        {
            var viewModel = new CohereSubscriptionDecisionService().BuildViewModel(NotDetectedFor(isAdmin: false));

            Assert.Null(viewModel.LearnMoreUri);
        }
    }
}
