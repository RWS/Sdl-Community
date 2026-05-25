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
    /// Explicit, user-triggered migration of Multilingual Excel FileType identifiers
    /// inside the selected project's .sdlproj and .sdlxliff files: any legacy versioned
    /// id (e.g. "Multilingual Excel FileType v 1.0.0.0") or unversioned id
    /// ("Multilingual Excel FileType") is rewritten to the currently installed plugin's
    /// versioned id (e.g. "Multilingual Excel FileType v 3.0.2.0").
    /// </summary>
    [Action(
        "Multilingual.Excel.FileType.MigrateLegacyFileTypeIdAction",
        Name = "Migrate Legacy File Type Id",
        Icon = "MLExcel",
        Description = "Use this on a project that was created with an earlier version of the Multilingual Excel FileType plugin. It rewrites references to the Multilingual Excel FileType in the selected project's .sdlproj and .sdlxliff files so they match the currently installed version of the plugin (e.g. ' v 1.0.0.0' becomes ' v 3.0.0.0').")]
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
                    "This will rewrite references to the Multilingual Excel FileType inside the .sdlproj and all .sdlxliff files of the selected project so they match the currently installed plugin version:\r\n\r\n"
                        + projectName
                        + "\r\n\r\nLegacy versioned ids (e.g. ' v 1.0.0.0') and unversioned ids will be replaced with '"
                        + Constants.FiletypeConstants.FileTypeDefinitionId
                        + "'.\r\n\r\nThe project will be closed in Studio, the files will be rewritten in place (original encoding/BOM preserved), and the project will then be re-opened.\r\n\r\nIt is strongly recommended that you back up the project (or have it under source control) before proceeding, as this operation cannot be undone.\r\n\r\nProceed?",
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
                    Trace.TraceError("MultilingualExcel: failed to close project '{0}' before migration: {1}", projectName, ex.Message);
                    MessageBox.Show(
                        "Migration aborted: the project could not be closed in Studio, which is required before its files can be rewritten safely.\r\n\r\n"
                            + ex.Message,
                        "Multilingual Excel FileType",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                FileBasedProject reopened = null;
                string reopenError = null;
                var previousCursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    var service = new FileTypeIdMigrationService();
                    service.MigrateProject(projectFilePath);

                    if (!string.IsNullOrEmpty(projectFilePath) && File.Exists(projectFilePath))
                    {
                        try
                        {
                            reopened = projectsController.Add(projectFilePath);
                        }
                        catch (Exception ex)
                        {
                            reopenError = ex.Message;
                            Trace.TraceWarning("MultilingualExcel: failed to re-add project '{0}' after migration: {1}", projectName, ex.Message);
                        }
                    }
                }
                finally
                {
                    Cursor.Current = previousCursor;
                }

                MessageBox.Show(
                    "Migration completed for project:\r\n\r\n"
                        + projectName
                        + (reopened == null
                            ? "\r\n\r\nThe project could not be re-opened automatically; please open it manually from:\r\n"
                                + projectFilePath
                                + (reopenError != null ? "\r\n\r\nReason: " + reopenError : string.Empty)
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
