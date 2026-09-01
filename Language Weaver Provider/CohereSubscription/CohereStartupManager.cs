using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using NLog;
using Sdl.Desktop.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription
{
    public class CohereStartupManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CohereSubscriptionOrchestrator _cohereOrchestrator;
        private readonly ICohereSubscriptionSettingsService _settings;
        private readonly List<AbstractViewController> _controllers;


        public CohereStartupManager(
            CohereSubscriptionOrchestrator cohereOrchestrator,
            ICohereSubscriptionSettingsService settings,
            List<AbstractViewController> controllers)
        {
            _cohereOrchestrator = cohereOrchestrator;
            _settings = settings;
            _controllers = controllers;
        }

        public void Initialize()
        {
            AddSubscriber();
            _ = RunWorkflow();
        }


        private async void OnViewActivated(object sender, EventArgs e)
        {
            await RunWorkflow();
        }

        private async Task RunWorkflow()
        {
            try
            {
                if (_settings.GetDoNotShowAgain())
                {
                    RemoveSubscriber();
                    return;
                }

                // Deliberately stays subscribed after showing: the orchestrator prompts once per signed-in
                // tenant, so remaining subscribed is what lets a re-sign-in be noticed.
                await _cohereOrchestrator.RunAsync();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "[Cohere] Subscription check failed during view activation.");
            }
        }

        private void AddSubscriber()
        {
            foreach (var controller in _controllers)
            {
                controller.ActivationChanged += OnViewActivated;
            }
        }

        private void RemoveSubscriber()
        {
            foreach (var controller in _controllers)
            {
                controller.ActivationChanged -= OnViewActivated;
            }
        }
    }
}
