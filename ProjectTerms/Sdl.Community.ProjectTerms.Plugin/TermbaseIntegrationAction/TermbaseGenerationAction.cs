using Sdl.Desktop.IntegrationApi;
using Sdl.MultiTerm.TMO.Interop;
using System;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction;
using System.Windows.Forms;
using Sdl.ProjectAutomation.Core;
using Sdl.Core.Globalization;
using System.Globalization;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Community.ProjectTerms.Plugin;

namespace Sdl.Community.ProjectTerms.TermbaseIntegrationAction
{
    [Action("TermbaseGeneration", Name = "Generate Termbase", Description = "TermbaseGeneration_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class TermbaseGenerationAction : AbstractViewControllerAction<FilesController>
    {
        protected override void Execute()
        {
            try
            {
                var termbaseCreator = new TermbaseGeneration();

                var termbaseDefaultContent = TermbaseDefinitionFile.GetResourceTextFile("termbaseDefaultDefinitionFile.xdt");
                var termbaseDefinitionPath = TermbaseDefinitionFile.SaveTermbaseDefinitionToTempLocation(termbaseDefaultContent);
                TermbaseDefinitionFile.AddLanguages(termbaseDefinitionPath, termbaseCreator.GetProjectLanguages());

                var termbase = termbaseCreator.CreateTermbase(termbaseDefinitionPath);
                if (termbase == null)
                {
                    DisplayErrorMessage(PluginResources.Info_TermbaseExists, PluginResources.MessageTitle);
                    return;
                }

                termbaseCreator.PopulateTermbase(termbase);

                IncludeTermbaseInStudio(termbase, termbaseCreator);
                DisplayErrorMessage(PluginResources.Info_SuccessfullyAdded, PluginResources.MessageTitle);
            }
            catch(ProjectTermsException e)
            {
                DisplayErrorMessage(e.Message, PluginResources.MessageType_Error);
            }
            catch(TermbaseDefinitionException e)
            {
                DisplayErrorMessage(e.Message, PluginResources.MessageType_Error);
            }
            catch (TermbaseGenerationException e)
            {
                DisplayErrorMessage(e.Message, PluginResources.MessageType_Error);
            }
            catch(UploadTermbaseException e)
            {
                DisplayErrorMessage(e.Message, PluginResources.MessageType_Error);
            }
        }

        private void DisplayErrorMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK);
        }

        private void IncludeTermbaseInStudio(ITermbase termbase, TermbaseGeneration termbaseCreator)
        {
            try
            {
                #region TbConfig
                var project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
                TermbaseConfiguration termbaseConfig = project.GetTermbaseConfiguration();
                #endregion

                #region AddTb
                var studioTermbase = new LocalTermbase(termbase._Path);
                termbaseConfig.Termbases.Add(studioTermbase);
                #endregion

                #region TermRecOptions
                TermRecognitionOptions options = termbaseConfig.TermRecognitionOptions;
                options.MinimumMatchValue = 50;
                options.SearchDepth = 200;
                options.ShowWithNoAvailableTranslation = false;
                options.SearchOrder = TermbaseSearchOrder.Hierarchical;
                #endregion

                #region TermbaseLanguageIndex
                var langs = termbaseCreator.GetProjectLanguages();
                termbaseConfig.LanguageIndexes.Clear();
                foreach (var lang in langs.Keys)
                {
                    termbaseConfig.LanguageIndexes.Add(new TermbaseLanguageIndex(new Language(CultureInfo.GetCultureInfo(langs[lang])), lang));
                }
                #endregion

                #region UpdateTermbaseConfiguration
                project.UpdateTermbaseConfiguration(termbaseConfig);
                #endregion
            }
            catch (Exception e)
            {
                throw new UploadTermbaseException(PluginResources.Error_IncludeTermbaseInStudio + e.Message);
            }
        }
    }
}
