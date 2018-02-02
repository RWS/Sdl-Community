using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
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
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdaptiveMT
{
	[Action("AdaptiveMt",
		Name = "Adaptive MT Training",
		Description = "Adaptive MT Training",
		Icon = "icon"
	)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "",
		true)]
	public class AdaptiveMtRibbon : AbstractAction
	{
		public ProjectsController GetProjectsController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}


		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		protected override async void Execute()
		{
			var projects = GetProjectsController().SelectedProjects;

			var userCredentials = Helpers.Credentials.GetCredentials();
			if (userCredentials != null)
			{
				var userDetails = await ApiClient.Login(userCredentials.Email, userCredentials.Password);
				await ApiClient.OosSession(userCredentials, userDetails.Sid);
				var providerUrl = string.Empty;

				foreach (var project in projects)
				{
					var providerExist = false;
					var provider = project.GetTranslationProviderConfiguration();

					foreach (var entry in provider.Entries)
					{
						if (entry.MainTranslationProvider.Enabled &&
						    entry.MainTranslationProvider.Uri.AbsoluteUri.Contains("bmslanguagecloud"))
						{
							providerExist = true;
							providerUrl = HttpUtility.UrlDecode(HttpUtility.UrlDecode(entry.MainTranslationProvider.Uri.AbsoluteUri));
							break; 
						}
					}
					if (providerExist)
					{
						var files = project.GetTargetLanguageFiles();
						var providersDetails = EngineDetails.GetDetailsFromEngineUrl(providerUrl);
						using (var waitForm = new WaitForm())
						{
							waitForm.Show();
							await ProcessFiles(files, providersDetails, userDetails, project);
							waitForm.Close();
						}
					}
				}
			}
		}

		private async System.Threading.Tasks.Task ProcessFiles(ProjectFile[]files, List<EngineMappingDetails>
			providersDetails, UserResponse userDetails, FileBasedProject project)
		{
			var editorController = GetEditorController();
			foreach (var file in files)
			{
				var targetLanguage = file.Language.IsoAbbreviation;
				var document = editorController.Open(file, EditingMode.Translation);
				var segmentPairs = document.SegmentPairs.ToList();
				var providerDetails = providersDetails.FirstOrDefault(t => t.TargetLang.Equals(targetLanguage));
				if (providerDetails != null)
				{
					providerDetails.SourceLang = file.SourceFile.Language.IsoAbbreviation;

					//Confirm each segment
					foreach (var segmentPair in segmentPairs)
					{
						if (segmentPair.Target.ToString() != string.Empty)
						{
							var translateRequest = Helpers.Api.CreateTranslateRequest(segmentPair, providerDetails);
							var translateResponse = await ApiClient.Translate(userDetails.Sid, translateRequest);

							var feedbackRequest =
								Helpers.Api.CreateFeedbackRequest(translateResponse.Translation, segmentPair, providerDetails);
							var feedbackReaponse = await ApiClient.Feedback(userDetails.Sid, feedbackRequest);
							if (feedbackReaponse.Success)
							{
								editorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPair.Properties);
							}
						}
					}
				}
				project.Save();
			}
		}
	}
}

