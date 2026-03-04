using LanguageWeaverProvider.CohereSubscription.Settings.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions.Internal;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.CohereSubscription
{
    public class CohereStartupManager
    {
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
        }


        private async void OnViewActivated(object sender, EventArgs e)
        {
            try
            {
                if (_settings.GetDoNotShowAgain())
                {
                    RemoveSubscriber();
                    return;
                }

                bool wasShown = await _cohereOrchestrator.RunAsync();

                if (wasShown)
                {
                    RemoveSubscriber();
                }
            }
            catch (Exception) { }
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
