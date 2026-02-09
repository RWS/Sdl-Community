using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using LanguageWeaverProvider.CohereSubscription;
using LanguageWeaverProvider.CohereSubscription.Decision;
using LanguageWeaverProvider.CohereSubscription.Settings;
using LanguageWeaverProvider.CohereSubscription.Workflow;
using LanguageWeaverProvider.CohereSubscription.Workflow.Interfaces;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.View;
using Newtonsoft.Json;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace LanguageWeaverProvider
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
        private const string BatchProcessingWizardAutomationId = "Wizard_Window_BatchProcessingWizard";
        private const string ProjectWizardAutomationId = "Wizard_Window_ProjectWizard";

		public static string CurrentAppVersion { get; private set; }

		public static IList<RatedSegment> RatedSegments { get; set; } = new List<RatedSegment>();

        public static ITranslationProviderCredentialStore CredentialStore { get; set; }

        public static PluginVersion PluginVersion { get; set; } = PluginVersion.None;

		public static IDictionary<string, ITranslationOptions> TranslationOptions { get; set; } = new Dictionary<string, ITranslationOptions>();
        public static bool IsStandalone { get; set; }

        public void Execute()
        {
			CurrentAppVersion = GetAssemblyFileVersion();
            Log.Setup();
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };

            SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
              .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj)
        {
            var cohereSettingsService = CohereSubscriptionSettingsServiceFactory.Create();
            var cohereWorkflowService = CohereWorkflowServiceFactory.Create();
            var cohereDecisionService = CohereSubscriptionDecisionServiceFactory.Create();

            var cohereOrchestrator = new CohereSubscriptionOrchestrator(
                cohereSettingsService, cohereWorkflowService, cohereDecisionService);

            List<AbstractViewController> viewControllers = new List<AbstractViewController>()
            {
                SdlTradosStudio.Application.GetController<WelcomeViewController>(),
                SdlTradosStudio.Application.GetController<ProjectsController>(),
                SdlTradosStudio.Application.GetController<FilesController>(),
                SdlTradosStudio.Application.GetController<EditorController>(),
                SdlTradosStudio.Application.GetController<ReportsViewController>(),
                SdlTradosStudio.Application.GetController<TranslationMemoriesViewController>()
            };

            var cohereStartupManager = new CohereStartupManager(
                cohereOrchestrator, cohereSettingsService, viewControllers);

            cohereStartupManager.Initialize();
        }



        public static Window GetBatchTaskWindow()
        {
            // Get the list of current windows
            var windows = Application.Current.Windows.Cast<Window>().ToList();

            // Find the first window with the specified automation IDs
            var targetWindow = windows.FirstOrDefault(window =>
            {
                var automationId = window.GetValue(System.Windows.Automation.AutomationProperties.AutomationIdProperty) as string;
                return automationId == BatchProcessingWizardAutomationId ||
                       automationId == ProjectWizardAutomationId;
            });

            return targetWindow;
        }


        private static string GetAssemblyFileVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyFileVersionAttribute = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            return assemblyFileVersionAttribute?.Version;
        }
    }
}