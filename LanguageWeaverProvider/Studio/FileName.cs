using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace LanguageWeaverProvider.Studio
{
	[ViewPart(
		Id = "LanguageWeaverEditorController",
		Name = "LanguageWeaverEditorController_Name",
		Description = "LanguageWeaverEditorController_Description",
		Icon = "lwLogoIco"
	)]

	[ViewPartLayout(typeof(EditorController), Dock = DockType.Left)]
	public class LanguageWeaverEditorController : AbstractViewPartController
	{
		private EditorController _editorController;
		private ProjectsController _projectsController;

		protected override void Initialize()
		{
			 var methodName = "Initialize";

			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
			_projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			var translationProviderConfiguration = _projectsController.CurrentProject.GetTranslationProviderConfiguration();
		}

		protected override IUIControl GetContentControl()
		{
			return default;
		}

		private void GetConfiguration()
		{
			var _pathInfo = _projectsController.CurrentProject.FilePath;
			var assembly = Assembly.GetExecutingAssembly().Location;
			var defaultSettings = GetConfiguration(ConfigurationManager.OpenExeConfiguration(assembly));
			var json = File.ReadAllText(_pathInfo);
		}

		private dynamic GetConfiguration(Configuration config)
		{
			var baseDomain = GetConfigValue(config, "OpenAI_ApiBaseDomain");
			var apiEndpoint = GetConfigValue(config, "OpenAI_ApiEndpoint");
			var apiModel = GetConfigValue(config, "OpenAI_ApiModel");
			var apiTemperatureString = GetConfigValue(config, "OpenAI_ApiTemperature");
			var apiTemperatoreSuccess = int.TryParse(apiTemperatureString, out var apiTemperatore);

			dynamic options = new
			{
				ApiBaseDomain = baseDomain,
				ApiEndpoint = apiEndpoint,
				ApiModel = apiModel,
				ApiTemperature = apiTemperatoreSuccess ? apiTemperatore : 75,
			};

			return options;
		}

		private static string GetConfigValue(Configuration config, string propertyName)
		{
			if (config.AppSettings.Settings.AllKeys.Any(a => a == propertyName))
			{
				var value = config.AppSettings.Settings[propertyName].Value;

				return value;

			}

			return null;
		}
	}
}
