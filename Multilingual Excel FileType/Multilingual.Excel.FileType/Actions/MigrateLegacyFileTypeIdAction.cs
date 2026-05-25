using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Multilingual.Excel.FileType.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Multilingual.Excel.FileType.Actions
{
    /// <summary>
    /// Ribbon group that hosts Multilingual Excel FileType maintenance actions
    /// in the Projects view.
    /// </summary>
    [RibbonGroup(
        "Multilingual.Excel.FileType.RibbonGroup",
        Name = "Multilingual Excel",
        ContextByType = typeof(ProjectsController))]
    [RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
    public class MultilingualExcelRibbonGroup : AbstractRibbonGroup
    {
    }

    /// <summary>
    /// Explicit, user-triggered migration of legacy versioned file type identifiers
    /// (e.g. "Multilingual Excel FileType v 3.0.2.0") to the stable, unversioned
    /// "Multilingual Excel FileType" identifier inside the selected project's
    /// .sdlproj and .sdlxliff files.
    /// </summary>
    [Action(
        "Multilingual.Excel.FileType.MigrateLegacyFileTypeIdAction",
        Name = "Migrate Legacy File Type Id",
        Icon = "MLExcel",
        Description = "Use this on a project that was created with an earlier version of the Multilingual Excel FileType plugin. It removes the legacy version suffix (e.g. ' v 1.0.0.0') from references to the Multilingual Excel FileType in the selected project's .sdlproj and .sdlxliff files so the project is compatible with the currently installed version of the plugin.")]
    [ActionLayout(typeof(MultilingualExcelRibbonGroup), 10, DisplayType.Large)]
    public class MigrateLegacyFileTypeIdAction : AbstractAction
    {
        protected override void Execute()
        {
            FileBasedProject project = null;
            try
            {
                var projectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
                if (projectsController == null)
                {
                    MessageBox.Show(
                        "Trados Studio Projects view is not available.",
                        "Multilingual Excel FileType",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                project = projectsController.SelectedProjects?.FirstOrDefault()
                          ?? projectsController.CurrentProject;

                if (project == null)
                {
                    MessageBox.Show(
                        "Please select a project in the Projects view first.",
                        "Multilingual Excel FileType",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                var projectName = project.GetProjectInfo()?.Name ?? "(unknown)";
                var projectFilePath = project.FilePath;

                var confirm = MessageBox.Show(
                    "This will remove the legacy version suffix (e.g. ' v 3.0.2.0') from references to the Multilingual Excel FileType inside the .sdlproj and all .sdlxliff files of the selected project:\r\n\r\n"
                        + projectName
                        + "\r\n\r\nThe project will be closed in Studio, the files will be rewritten in place (original encoding/BOM preserved, only the exact suffix removed), and the project will then be re-opened.\r\n\r\nProceed?",
                    "Multilingual Excel FileType",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (confirm != DialogResult.OK)
                {
                    return;
                }

                // Studio keeps the .sdlproj open and re-writes it on close,
                // which undoes our migration. We must therefore:
                //   1. Close the project in Studio so it releases its handles
                //      AND stops tracking pending changes for it.
                //   2. Rewrite the files on disk.
                //   3. Re-add the project so the user sees the updated copy.
                try
                {
                    projectsController.Close(project);
                }
                catch (Exception ex)
                {
                    Trace.TraceWarning("MultilingualExcel: failed to close project '{0}' before migration: {1}", projectName, ex.Message);
                }

                var service = new FileTypeIdMigrationService();
                service.MigrateProject(project);

                FileBasedProject reopened = null;
                if (!string.IsNullOrEmpty(projectFilePath) && File.Exists(projectFilePath))
                {
                    try
                    {
                        reopened = projectsController.Add(projectFilePath);
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning("MultilingualExcel: failed to re-add project '{0}' after migration: {1}", projectName, ex.Message);
                    }
                }

                MessageBox.Show(
                    "Migration completed for project:\r\n\r\n"
                        + projectName
                        + (reopened == null
                            ? "\r\n\r\nThe project could not be re-opened automatically; please open it manually from:\r\n" + projectFilePath
                            : "\r\n\r\nThe project has been re-opened.")
                        + "\r\n\r\nSee the Trace output for per-file details.",
                    "Multilingual Excel FileType",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Migration failed: " + ex.Message,
                    "Multilingual Excel FileType",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
