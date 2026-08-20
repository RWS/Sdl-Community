using LanguageWeaverProvider.CohereSubscription.Decision.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.SubscriptionJourney.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription
{
    public class CohereSubscriptionOrchestrator
    {
        private readonly ICohereSubscriptionSettingsService _settings;
        private readonly ICohereSubscriptionWorkflowService _workflow;
        private readonly ICohereSubscriptionDecisionService _decisionService;

        private bool _hasShownThisSession;
        private bool _isRunning;

        public CohereSubscriptionOrchestrator(
            ICohereSubscriptionSettingsService settings,
            ICohereSubscriptionWorkflowService workflow,
            ICohereSubscriptionDecisionService decisionService)
        {
            _settings = settings;
            _workflow = workflow;
            _decisionService = decisionService;
        }

        public async Task<bool> RunAsync()
        {
            if (_hasShownThisSession || _isRunning || _settings.GetDoNotShowAgain())
                return false;

            _isRunning = true;
            try
            {
                var data = await _workflow.ExecuteAsync();
                var viewModel = _decisionService.BuildViewModel(data);
                if (viewModel == null)
                    return false;

                var view = new CohereSubscriptionWindow
                {
                    DataContext = viewModel
                };

                _hasShownThisSession = true;
                _ = view.ShowDialog();

                if (viewModel.DoNotShowThisAgain)
                    _settings.SetDoNotShowAgain(true);

                return true;
            }
            finally
            {
                _isRunning = false;
            }
        }
    }
}
