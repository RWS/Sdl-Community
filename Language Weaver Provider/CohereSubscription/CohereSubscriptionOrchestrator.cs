using LanguageWeaverProvider.CohereSubscription.Decision.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.Model.Interface;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription
{
    public class CohereSubscriptionOrchestrator
    {
        private readonly ICohereSubscriptionSettingsService _settings;
        private readonly ICohereSubscriptionWorkflowService _workflow;
        private readonly ICohereSubscriptionDecisionService _decisionService;
        private readonly ISignedInTenant _signedInTenant;
        private readonly ISubscriptionPrompt _prompt;

        private string _checkedTenantId;
        private bool _isRunning;

        public CohereSubscriptionOrchestrator(
            ICohereSubscriptionSettingsService settings,
            ICohereSubscriptionWorkflowService workflow,
            ICohereSubscriptionDecisionService decisionService,
            ISignedInTenant signedInTenant,
            ISubscriptionPrompt prompt)
        {
            _settings = settings;
            _workflow = workflow;
            _decisionService = decisionService;
            _signedInTenant = signedInTenant;
            _prompt = prompt;
        }

        public async Task RunAsync()
        {
            // A blank tenant means Studio has no session to check yet, not that the user changed.
            var tenantId = _signedInTenant.GetActiveTenantId();
            if (tenantId is null)
                return;

            if (tenantId == _checkedTenantId || _isRunning || _settings.GetDoNotShowAgain())
                return;

            _isRunning = true;
            try
            {
                var data = await _workflow.ExecuteAsync();

                // Recorded once the check has happened, not once a window is shown: deciding that this
                // tenant needs no prompt is still an answer, and re-asking would repeat the portal call
                // on every view activation.
                _checkedTenantId = tenantId;

                var viewModel = _decisionService.BuildViewModel(data);
                if (viewModel == null)
                    return;

                _prompt.Show(viewModel);

                if (viewModel.DoNotShowThisAgain)
                    _settings.SetDoNotShowAgain(true);
            }
            finally
            {
                _isRunning = false;
            }
        }
    }
}
