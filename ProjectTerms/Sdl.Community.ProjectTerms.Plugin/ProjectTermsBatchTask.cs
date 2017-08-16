using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML;
using System.IO;
using System;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using System.Windows.Forms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [AutomaticTask("Coding.ProjectTerms.BatchTask",
        "Project Terms",
        "Export project terms according the following criteria...",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof(ProjectTermsBatchTaskSettings), typeof(ProjectTermsBatchTaskSettingsPage))]
    public class ProjectTermsBatchTask : AbstractFileContentProcessingAutomaticTask
    {
        private HashSet<string> projectFiles;
        private ProjectTermsBatchTaskSettingsControl control;
        private ProjectTermsBatchTaskSettings settings;

        protected override void OnInitializeTask()
        {
            base.OnInitializeTask();

            projectFiles = new HashSet<string>();
            control = new ProjectTermsBatchTaskSettingsControl();
            settings = GetSetting<ProjectTermsBatchTaskSettings>();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            if (projectFiles.Contains(projectFile.Name)) return;

            control.ExtractProjectFileTerms(projectFile, multiFileConverter);
            projectFiles.Add(projectFile.Name);
        }

        public override void TaskComplete()
        {
            try
            {
                control.ExtractProjectTerms(settings);
                AddXMlToProject(Project, Path.GetDirectoryName(ProjectTermsCache.GetXMLFilePath(control.ProjectPath)), false);
                CreateReport("Project terms", "Project terms report", GenerateReport());
            }
            catch (ProjectTermsException e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK);
            }
        }

        private string GenerateReport()
        {
            string reportContent = "<report><projectTerms>";
            reportContent += "<header>The project contains:</header>";
            reportContent += "<noTerms>" + control.GetNumbersOfExtractedTerms().ToString() + "</noTerms>";
            reportContent += "<occurrences>" + settings.TermsOccurrencesSettings.ToString() + "</occurrences>";
            reportContent += "<length>" + settings.TermsLengthSettings.ToString() + "</length>";
            reportContent += "</projectTerms>";
            reportContent += "</report>";

            return reportContent;
        }

        private void AddXMlToProject(IProject project, string xmlFolder, bool recursion)
        {
            try
            {
                project.AddFolderWithFiles(xmlFolder, recursion);
                ProjectFile[] projectFiles = project.GetSourceLanguageFiles();
                AutomaticTask scan = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
                AutomaticTask convertTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.ConvertToTranslatableFormat);
                AutomaticTask copyTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.CopyToTargetLanguages);

                Utils.Utils.RemoveDirectory(xmlFolder);
            }
            catch (Exception e)
            {
                throw new ProjectTermsException("Add xml file in the Studio project failed!\n" + e.Message);
            }
        }
    }
}
