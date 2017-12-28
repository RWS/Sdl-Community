using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Sdl.Community.AdaptiveMT.Service;
using Sdl.Community.AdaptiveMT.Service.Clients;
using Sdl.Community.AdaptiveMT.Service.Helpers;
using Sdl.Community.AdaptiveMT.Service.Model;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdaptiveMT
{
	[Action("AdaptiveMt",
		Name = "Adaptive MT Training",
		Description = "Adaptive MT Training"
		)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class AdaptiveMtRibbon: AbstractAction
	{
		public ProjectsController GetProjectsController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}
		private Document ActiveDocument { get; set; }

		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		protected  override async void Execute()
		{
			var editorController = GetEditorController();
			var projects = GetProjectsController().SelectedProjects;

			Helpers.Credentials.GetCredentials();
			

			//var userDerails = await ApiClient.Login("fcaputa@sdl.com", "101qwe101!!@");

			//await ApiClient.Feedback(userDerails.Sid);

			var providerUrl = string.Empty;

			foreach (var project in projects)
			{
				var providerExist = false;
				var provider = project.GetTranslationProviderConfiguration();

				//from gere get credentioal with reflection
				var factory = TranslationProviderManager.GetTranslationProviderWinFormsUIs();

				foreach (var entry in provider.Entries)
				{
					if (entry.MainTranslationProvider.Enabled && entry.MainTranslationProvider.Uri.AbsoluteUri.Contains("bmslanguagecloud"))
					{
						providerExist = true;
						providerUrl = HttpUtility.UrlDecode(entry.MainTranslationProvider.Uri.AbsoluteUri);
						break;//for the moment we take only the first cloud provider
						
					}
				}
				
				var files = project.GetTargetLanguageFiles();
				var providerDetails = EngineDetails.GetDetailsFromEngineUrl(providerUrl);

				//Confirm all segments in file needs to be uncommented later
				//foreach (var file in files)
				//{					
				//	var document = editorController.Open(file, EditingMode.Translation);
				//	var segmentPairs = document.SegmentPairs.ToList();
				//	//Confirm each segment
				//	foreach (var segmentPair in segmentPairs)
				//	{
				//		if (segmentPair.Target.ToString() != string.Empty)
				//		{
				//			segmentPair.Properties.ConfirmationLevel = ConfirmationLevel.Translated;
				//			editorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPair.Properties);

				//		}
				//	}
				//}
			}
		}
	}
}
