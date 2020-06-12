using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using System;
using System.Globalization;
using System.Windows.Forms;

namespace AddTmToProject
{
    [RibbonGroup("AddTmToProject", Name = "AddTmToProject")]
    [RibbonGroupLayout(LocationByType = typeof(StudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
    public class ApplyTMTemplateRibbonGroup : AbstractRibbonGroup
    {
    }

    [Action("AddTmToProject")]
    [ActionLayout(typeof(ApplyTMTemplateRibbonGroup), 10, DisplayType.Large)]
    public class MyCustomTradosStudio : AbstractAction
    {
        protected override void Execute()
        {
            var dlg = new OpenFileDialog();
            dlg.ShowDialog();

            var project = new FileBasedProject(dlg.FileName);
            project.Save();

            dlg.ShowDialog();
            AddTm(project, dlg.FileName);
        }

        private void AddTm(FileBasedProject project, string tmFilePath)
        {
            var config = project.GetTranslationProviderConfiguration(new Language(CultureInfo.GetCultureInfo("aa-DJ")));

            var tm = new TranslationProviderCascadeEntry(
                tmFilePath,
                true,
                true,
                false);
            config.OverrideParent = true;
            config.Entries.Add(tm);

            project.UpdateTranslationProviderConfiguration(new Language(CultureInfo.GetCultureInfo("aa-DJ")), config);
        }
    }
}