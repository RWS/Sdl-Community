using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Forms;
using Sdl.Community.AdaptiveMT.Service.Clients;
using Sdl.Community.AdaptiveMT.Service.Helpers;
using Sdl.Community.AdaptiveMT.Service.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
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
		private bool _shouldExit;
		private List<ProcessedFileDetails>_processedFilesList = new List<ProcessedFileDetails>();

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
			_shouldExit = false;
			var projects = GetProjectsController().SelectedProjects;
			var editorController = GetEditorController();
			editorController.Closing += EditorController_Closing;
			
			var userCredentials = Helpers.Credentials.GetCredentials();
			if (userCredentials != null)
			{
				var userDetails = await ApiClient.Login(userCredentials.Email, userCredentials.Password);
				await ApiClient.OosSession(userCredentials, userDetails.Sid);
				var providerUrl = string.Empty;

				foreach (var project in projects)
				{
					_processedFilesList.Clear();
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
							await ProcessFiles(files, providersDetails, userDetails);
							waitForm.Close();
						}
					}
				}
			}
		}
		
		private void EditorController_Closing(object sender, CancelDocumentEventArgs e)
		{
			_processedFilesList.Clear();
			_shouldExit = true;
		}

		private async System.Threading.Tasks.Task ProcessFiles(ProjectFile[]files, List<EngineMappingDetails>
			providersDetails, UserResponse userDetails)
		{
			var editorController = GetEditorController();
			
			foreach (var file in files)
			{
				
				if (_shouldExit)
				{
					break;
				}
				var targetLanguage = file.Language.IsoAbbreviation;
				var document = editorController.Open(file, EditingMode.Translation);
				var segmentPairs = document.SegmentPairs.ToList();
				var segmentsNumber = segmentPairs.Count;
				var currentSegmentIndex = 0;
				var providerDetails = providersDetails.FirstOrDefault(t => t.TargetLang.Equals(targetLanguage));
				if (providerDetails != null)
				{
					providerDetails.SourceLang = file.SourceFile.Language.IsoAbbreviation;
					var processedFile = new ProcessedFileDetails
					{
						FileId = editorController.ActiveDocument.ActiveFile.Id.ToString(),
						ProcessCompleted = false
					};
					_processedFilesList.Add(processedFile);
					//Confirm each segment
					foreach (var segmentPair in segmentPairs)
					{
						if (_shouldExit)
						{
							break;
						}
						if (segmentPair.Target.ToString() != string.Empty)
						{
							var translateRequest = Helpers.Api.CreateTranslateRequest(segmentPair, providerDetails);
							var translateResponse = await ApiClient.Translate(userDetails.Sid, translateRequest);

							var feedbackRequest =
								Helpers.Api.CreateFeedbackRequest(translateResponse.Translation, segmentPair, providerDetails);
							 await ApiClient.Feedback(userDetails.Sid, feedbackRequest);
							if (currentSegmentIndex== segmentsNumber-1)
							{
								processedFile.ProcessCompleted = true;
							}

						}
						currentSegmentIndex++;
					}
				}
			}
		}
	}
}

