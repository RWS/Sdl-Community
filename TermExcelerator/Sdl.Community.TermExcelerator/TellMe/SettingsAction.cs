using System;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Services;
using Sdl.Community.TermExcelerator.TellMe.WarningWindow;
using Sdl.Community.TermExcelerator.Ui;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TermExcelerator.TellMe
{
	public class SettingsAction : AbstractTellMeAction
	{
		public const string ExcelUriTemplate = "excelglossary://";
		private static readonly string TerminologyProviderPathSeparator = "\\%\\";
		private PersistenceService _persistenceService;

		public SettingsAction()
		{
			Name = $"{PluginResources.Plugin_Name} Settings";
		}

		public override string Category => $"{PluginResources.Plugin_Name} results";
		public override Icon Icon => PluginResources.Settings;
		public override bool IsAvailable => true;

		private PersistenceService PersistenceService => _persistenceService ??= new PersistenceService();

		public override void Execute()
		{
			var project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			ProviderSettings newSettings;
			try
			{
				var currentSettings = GetCurrentTermExceleratorSettings(project);
				newSettings = GetSettingsFromUser(currentSettings);
				newSettings.Uri = new Uri((ExcelUriTemplate + Path.GetFileName(newSettings.TermFilePath)).RemoveUriForbiddenCharacters());


			}
			catch (Exception)
			{
				ShowWarningMessage();
				return;
			}

			PersistenceService.AddSettings(newSettings);
			ApplicationContext.RaiseSettingsChangedFromTellMeAction();
		}

		private static ProviderSettings GetSettingsFromUser(ProviderSettings settings)
		{
			var settingsDialog = new Settings(true);

			settingsDialog.SetSettings(settings);
			settingsDialog.ShowDialog();
			return settingsDialog.GetSettings();
		}

		private ProviderSettings GetCurrentTermExceleratorSettings(FileBasedProject project)
		{
			var termbaseConfiguration = project.GetTermbaseConfiguration();
			var termExceleratorTermbaseConfiguration = termbaseConfiguration.Termbases
				.Find(t => t.Name.Contains(PluginResources.Plugin_Name));

			var serializer = new XmlSerializer(typeof(TermbaseSettings));
			using var reader = new StringReader(termExceleratorTermbaseConfiguration.SettingsXML);
			var termbaseSettings = (TermbaseSettings)serializer.Deserialize(reader);

			var providerUri = termbaseSettings.Path.Substring(0,
				termbaseSettings.Path.LastIndexOf(TerminologyProviderPathSeparator, StringComparison.Ordinal));

			var settings = PersistenceService.Load(new Uri(providerUri));
			return settings;
		}

		private void ShowWarningMessage()
		{
			var settingsWarningWindow =
				new SettingsActionWarning("https://appstore.rws.com/Plugin/59?tab=documentation");
			settingsWarningWindow.ShowDialog();
		}
	}
}