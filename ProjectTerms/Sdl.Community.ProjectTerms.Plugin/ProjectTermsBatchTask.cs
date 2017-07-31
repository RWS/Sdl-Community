using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML;
using System.IO;

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
        string reportContent = "<report><projectTerms>";

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
            control.ExtractProjectTerms(settings);
            if (CheckedXmlExists(Project, ProjectTermsCache.GetXMLFilePath(control.ProjectPath)))
            {
                reportContent = "<report><projectTerms>";
                reportContent += "<infoTag>The project terms file was already generated!</infoTag>";
                GenerateReport();
                CreateReport("Project terms", "Project terms report", reportContent);
                return;
            }
            
            AddXMlToProject(Project, Path.GetDirectoryName(ProjectTermsCache.GetXMLFilePath(control.ProjectPath)), false);
            GenerateReport();
            CreateReport("Project terms", "Project terms report", reportContent);
        }

        private void GenerateReport()
        {
            reportContent += "<header>The project contains:</header>";
            reportContent += "<noTerms>" + control.GetNumbersOfExtractedTerms().ToString() + "</noTerms>";
            reportContent += "<occurrences>" + settings.TermsOccurrencesSettings.ToString() + "</occurrences>";
            reportContent += "<length>" + settings.TermsLengthSettings.ToString() + "</length>";
            reportContent += "</projectTerms>";
            reportContent += "</report>";
        }

        private void AddXMlToProject(IProject project, string xmlFolder, bool recursion)
        {
            project.AddFolderWithFiles(xmlFolder, recursion);
            ProjectFile[] projectFiles = project.GetSourceLanguageFiles();
            AutomaticTask scan = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            AutomaticTask convertTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.ConvertToTranslatableFormat);
            AutomaticTask copyTask = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.CopyToTargetLanguages);
        }

        private bool CheckedXmlExists(IProject project, string xmlFilePath)
        {
            string xmlfileName = Path.GetFileName(xmlFilePath) + ".sdlxliff";
            foreach (ProjectFile file in project.GetSourceLanguageFiles())
            {
                if (file.Name == xmlfileName) return true;
            }

            return false;
        }
    }
}
