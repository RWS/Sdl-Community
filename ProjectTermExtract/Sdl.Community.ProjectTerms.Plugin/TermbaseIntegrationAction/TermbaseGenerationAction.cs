using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Telemetry;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.Terminology.TerminologyProvider.Core.Termbase;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
{
	[Action("TermbaseGeneration", Name = "Generate Termbase", Description = "TermbaseGeneration_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class TermbaseGenerationAction : AbstractViewControllerAction<FilesController>
    {
        protected override void Execute()
        {
	        try
	        {
		        if (SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.Count() > 1)
		        {
			        MessageBox.Show(PluginResources.MessageContent_multipleFilesTermbase, PluginResources.MessageType_Info);
			        return;
		        }

		        var selectedFileName = Path.GetFileNameWithoutExtension(SdlTradosStudio.Application.GetController<FilesController>()?.SelectedFiles?.FirstOrDefault()?.Name);
		        var extractedXmlFileName = Utils.Utils.GetXmlFileName(Utils.Utils.GetProjectPath());
		        if (selectedFileName != null && !selectedFileName.Equals(extractedXmlFileName))
		        {
			        MessageBox.Show(PluginResources.MessageContent_GenerateTermbaseAction, PluginResources.MessageType_Info);
			        return;
		        }

				var termbaseCreator = new TermbaseGeneration();
				var termbase = GetTermbase(termbaseCreator);
				if (termbase == null)
				{
					DisplayMessage(PluginResources.Info_TermbaseExists, PluginResources.MessageTitle);
					return;
				}
				termbaseCreator.PopulateTermbase(termbase);

				AddStudioTermbase(termbaseCreator);
			}
			catch (ProjectTermsException e)
			{
				DisplayMessage(e.Message, PluginResources.MessageType_Error);
			}
			catch (TermbaseGenerationException e)
	        {
		        DisplayMessage($@"{e.Message}. {PluginResources.LocalTermbaseFilePath_Message}",
			        PluginResources.MessageType_Error);
	        }
	        catch (UploadTermbaseException e)
	        {
		        DisplayMessage(e.Message, PluginResources.MessageType_Error);
	        }
        }

		private void AddStudioTermbase(TermbaseGeneration termbaseCreator)
		{
			var termbasePath = termbaseCreator.TermbasePath;
			if (!string.IsNullOrEmpty(termbasePath) && File.Exists(termbasePath))
			{
				IncludeTermbaseInStudio(termbaseCreator, termbasePath);

				DisplayMessage(PluginResources.Info_SuccessfullyAdded, PluginResources.MessageTitle);
			}
			else
			{
				DisplayMessage(PluginResources.Info_NotSuccessfullyAdded, PluginResources.MessageTitle);
			}
		}

		private IStudioTermbase GetTermbase(TermbaseGeneration termbaseCreator)
		{
			return termbaseCreator.CreateTermbase();
		}

        private void DisplayMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK);
        }

        private void IncludeTermbaseInStudio(TermbaseGeneration termbaseCreator, string termbasePath)
        {
            ITelemetryTracker telemetryTracker = new TelemetryTracker();

            try
            {
                telemetryTracker.StartTrackRequest("Including the termbase into Trados Studio");
                telemetryTracker.TrackEvent("Including the termbase into Trados Studio");

                var project = StudioContext.ProjectsController.CurrentProject;
                var termbaseConfig = project.GetTermbaseConfiguration();

                var studioTermbase = new LocalTermbase(termbasePath);
                termbaseConfig.Termbases.Add(studioTermbase);

                var options = termbaseConfig.TermRecognitionOptions;
                options.MinimumMatchValue = 50;
                options.SearchDepth = 200;
                options.ShowWithNoAvailableTranslation = true;
                options.SearchOrder = TermbaseSearchOrder.Hierarchical;

                var targetLanguages = termbaseCreator.GetProjectTargetLanguages();
                termbaseConfig.LanguageIndexes.Clear();
                if (targetLanguages != null)
                {
	                foreach (var targetLanguage in targetLanguages.Keys)
	                {
		                termbaseConfig.LanguageIndexes.Add(new TermbaseLanguageIndex(LanguageRegistryApi.Instance.GetLanguage(CultureInfo.GetCultureInfo(targetLanguages[targetLanguage]).Name), targetLanguage));
	                }
                }
                project.UpdateTermbaseConfiguration(termbaseConfig);
            }
            catch (Exception e)
            {
                telemetryTracker.TrackException(new UploadTermbaseException(PluginResources.Error_IncludeTermbaseInStudio + e.Message));
                telemetryTracker.TrackTrace((new UploadTermbaseException(PluginResources.Error_IncludeTermbaseInStudio + e.Message)).StackTrace, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
                throw new UploadTermbaseException(PluginResources.Error_IncludeTermbaseInStudio + e.Message);
            }
        }
    }
}
