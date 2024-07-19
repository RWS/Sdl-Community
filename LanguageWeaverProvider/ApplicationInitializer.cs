using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
        private const string BatchProcessingWizardAutomationId = "Wizard_Window_BatchProcessingWizard";
        private const string ProjectWizardAutomationId = "Wizard_Window_ProjectWizard";

		public static string CurrentAppVersion { get; private set; }

		public static IList<RatedSegment> RatedSegments { get; set; }

		public static ITranslationProviderCredentialStore CredentialStore { get; set; }

		public static IDictionary<string, ITranslationOptions> TranslationOptions { get; set; }

		public void Execute()
		{
			RatedSegments = new List<RatedSegment>();
			TranslationOptions = new Dictionary<string, ITranslationOptions>();
			CurrentAppVersion = GetAssemblyFileVersion();
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