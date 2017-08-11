using Sdl.Desktop.IntegrationApi;
using Sdl.MultiTerm.TMO.Interop;
using System;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Community.ProjectTerms.Plugin.Termbase;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [Action("TermbaseGeneration", Name = "Generate Termbase", Description = "TermbaseGeneration_Description")]
    [ActionLayout(typeof(TranslationStudioDefaultContextMenus.FilesContextMenuLocation), 2, DisplayType.Large)]
    public class TermbaseGenerationAction : AbstractViewControllerAction<FilesController>
    {
        protected override void Execute()
        {
            var termbase = new TermbaseGeneration();

            var termbaseDefaultContent = TermbaseDefinitionFile.GetResourceTextFile("termbaseDefaultDefinitionFile.xdt");
            var termbaseDefinitionPath = TermbaseDefinitionFile.SaveTermbaseDefinitionToTempLocation(termbaseDefaultContent);
            TermbaseDefinitionFile.AddLanguages(termbaseDefinitionPath, termbase.GetProjectLanguages());

            var oTb = termbase.CreateTermbase(termbaseDefinitionPath);
            if (oTb == null)
            {
                MessageBox.Show("You already have a termbase for this file! Please remove it and generate it again!", "Generate termbase information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            termbase.PopulateTermbase(oTb);
        }
    }
}
