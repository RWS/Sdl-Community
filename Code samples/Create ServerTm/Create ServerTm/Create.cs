using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Create_ServerTm
{
	[ApplicationInitializer]
	public class Create : IApplicationInitializer
	{
		public void Execute()
		{
			var uri = new Uri(@"Add your url");
			var translationProviderServer = new TranslationProviderServer(uri, false, "user name", "password");

			var serverTm = new ServerBasedTranslationMemory(translationProviderServer)
			{
				Name = "Tm from API"
			};
			var resourceTemplate =
				translationProviderServer.GetLanguageResourcesTemplates(LanguageResourcesTemplateProperties.All);
			var containters = translationProviderServer.GetContainers(ContainerProperties.All);
			var container = containters.FirstOrDefault(c => c.Name.Equals("APSIC_TM_Container"));
			if (container != null)
			{
				serverTm.Container = container;
				serverTm.ParentResourceGroupPath = container.ParentResourceGroupPath;
				serverTm.LanguageResourcesTemplate = resourceTemplate?[0];
				CreateLanguageDirections(serverTm.LanguageDirections);

				serverTm.Save();
			}
		}

		private void CreateLanguageDirections(ServerBasedTranslationMemoryLanguageDirectionCollection directionsCollection)
		{
			var direction = new ServerBasedTranslationMemoryLanguageDirection
			{
				SourceLanguage = CultureInfo.GetCultureInfo("en-US"),
				TargetLanguage = CultureInfo.GetCultureInfo("de-DE")
			};

			directionsCollection.Add(direction);
		}

		private void ImportXliffToExistingTm()
		{
			var uri = new Uri(@"uri");

			var translationProviderServer = new TranslationProviderServer(uri, false, "user name", "password");

			//Import to existing tm
			var guid = new Guid("Existing TM Guid");
			var tm = translationProviderServer.GetTranslationMemory(guid, TranslationMemoryProperties.None);
			var langDirection = tm.LanguageDirections;

			var import = new ScheduledServerTranslationMemoryImport(langDirection[0])
			{
				Source = new FileInfo(@"path to xliff")
			};
			import.Queue();
		}
	}
}
