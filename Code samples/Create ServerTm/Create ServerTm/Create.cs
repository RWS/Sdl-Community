using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	}
}
