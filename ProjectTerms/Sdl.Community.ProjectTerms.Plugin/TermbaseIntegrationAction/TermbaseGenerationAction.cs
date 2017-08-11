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
                    MessageBox.Show("You already have a termbase for this file! Please remove it and generate it again!", "Generate termbase information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                termbaseCreator.PopulateTermbase(termbase);

                IncludeTermbaseInStudio(termbase, termbaseCreator);
            }
            catch (TermbaseGenerationException e)
            {

                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(UploadTermbaseException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
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
                throw new UploadTermbaseException("Including termbase to Studio error!");
            }
        }
    }
}
