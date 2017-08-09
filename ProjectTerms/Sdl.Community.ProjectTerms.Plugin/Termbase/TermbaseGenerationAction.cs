using Sdl.Desktop.IntegrationApi;
using Sdl.MultiTerm.TMO.Interop;
using System.Windows.Forms;
using System;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Community.ProjectTerms.Plugin.Termbase;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [Action("TermbaseCreator", Name = "Generate Termbase", Description = "TermbaseCreator_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class TermbaseGenerationAction : AbstractViewControllerAction<FilesController>
    {
        protected override void Execute()
        {
            TermbaseGeneration termbase = new TermbaseGeneration();

            string termbaseDefaultContent = TermbaseDefinitionFile.GetResourceTextFile("termbaseDefaultDefinitionFile.xdt");
            string termbaseDefinitionPath = TermbaseDefinitionFile.SaveTermbaseDefinitionToTempLocation(termbaseDefaultContent);
            TermbaseDefinitionFile.AddLanguages(termbaseDefinitionPath, termbase.GetProjectLanguages());

            ITermbase oTb = termbase.CreateTermbase(termbaseDefinitionPath);
            termbase.PopulateTermbase(oTb);

            MessageBox.Show("termbase creator!");
        }
    }
}
