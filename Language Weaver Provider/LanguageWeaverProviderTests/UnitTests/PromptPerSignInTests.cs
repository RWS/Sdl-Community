using LanguageWeaverProvider.CohereSubscription;
using LanguageWeaverProvider.CohereSubscription.Decision.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.SubscriptionJourney.Model;
using LanguageWeaverProvider.SubscriptionJourney.ViewModel;
using System.Threading.Tasks;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    public class PromptPerSignInTests
    {
        private class TheSignedInTenant : ISignedInTenant
        {
            public string TenantId;

            public string GetActiveTenantId() => TenantId;
        }

        private class ACountingPrompt : ISubscriptionPrompt
        {
            public int TimesShown;

            public void Show(SubscriptionViewModel viewModel) => TimesShown++;
        }

        private class AnAccountNeedingAPrompt : ICohereSubscriptionWorkflowService
        {
            public Task<CohereSubscriptionData> ExecuteAsync()
                => Task.FromResult(new CohereSubscriptionData());
        }

        private class AlwaysPrompts : ICohereSubscriptionDecisionService
        {
            public SubscriptionViewModel BuildViewModel(CohereSubscriptionData data)
                => new SubscriptionViewModel("Cohere", new SubscriptionOptions(), new ADiscardedUriOpener());
        }

        private class ADiscardedUriOpener : IUriOpener
        {
            public void OpenUri(string uri) { }
        }

        private class NeverSuppressed : ICohereSubscriptionSettingsService
        {
            public bool GetDoNotShowAgain() => false;

            public void SetDoNotShowAgain(bool value) { }
        }

        private static CohereSubscriptionOrchestrator OrchestratorFor(
            TheSignedInTenant tenant, ACountingPrompt prompt)
            => new CohereSubscriptionOrchestrator(
                new NeverSuppressed(),
                new AnAccountNeedingAPrompt(),
                new AlwaysPrompts(),
                tenant,
                prompt);

        [Fact]
        public async Task TheSameSignIn_IsPromptedOnlyOnce()
        {
            var tenant = new TheSignedInTenant { TenantId = "tenant-a" };
            var prompt = new ACountingPrompt();
            var orchestrator = OrchestratorFor(tenant, prompt);

            await orchestrator.RunAsync();
            await orchestrator.RunAsync();

            Assert.Equal(1, prompt.TimesShown);
        }

        [Fact]
        public async Task SigningInAsSomeoneElse_IsPromptedAgain()
        {
            var tenant = new TheSignedInTenant { TenantId = "tenant-a" };
            var prompt = new ACountingPrompt();
            var orchestrator = OrchestratorFor(tenant, prompt);

            await orchestrator.RunAsync();
            tenant.TenantId = "tenant-b";
            await orchestrator.RunAsync();

            Assert.Equal(2, prompt.TimesShown);
        }

        [Fact]
        public async Task WithNoSessionYet_NothingIsShownAndNothingIsRemembered()
        {
            var tenant = new TheSignedInTenant { TenantId = null };
            var prompt = new ACountingPrompt();
            var orchestrator = OrchestratorFor(tenant, prompt);

            await orchestrator.RunAsync();
            Assert.Equal(0, prompt.TimesShown);

            tenant.TenantId = "tenant-a";
            await orchestrator.RunAsync();

            Assert.Equal(1, prompt.TimesShown);
        }
    }
}
