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
                    DisplayErrorMessage("You already have a termbase for this file! Please remove it and generate it again!", "Termbase information");
                    return;
                }

                termbaseCreator.PopulateTermbase(termbase);

                IncludeTermbaseInStudio(termbase, termbaseCreator);
                DisplayErrorMessage("Your termbase was successfully added to the project!", "Termbase information");
            }
            catch(ProjectTermsException e)
            {
                DisplayErrorMessage(e.Message, "Error");
            }
            catch(TermbaseDefinitionException e)
            {
                DisplayErrorMessage(e.Message, "Error");
            }
            catch (TermbaseGenerationException e)
            {
                DisplayErrorMessage(e.Message, "Error");
            }
            catch(UploadTermbaseException e)
            {
                DisplayErrorMessage(e.Message, "Error");
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
                var project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
                var studioTermbase = new LocalTermbase(termbase._Path);
                TermbaseConfiguration termbaseConfig = project.GetTermbaseConfiguration();
                termbaseConfig.Termbases.Add(studioTermbase);

                var langs = termbaseCreator.GetProjectLanguages();
                termbaseConfig.LanguageIndexes.Clear();
                foreach (var lang in langs.Keys)
                {
                    termbaseConfig.LanguageIndexes.Add(new TermbaseLanguageIndex(new Language(CultureInfo.GetCultureInfo(langs[lang])), lang));
                }

                project.UpdateTermbaseConfiguration(termbaseConfig);
            } catch(Exception e)
            {
                throw new UploadTermbaseException("Including termbase to Studio error!\n" + e.Message);
            }
        }
    }
}
