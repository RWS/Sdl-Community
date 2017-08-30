using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using Sdl.Community.ProjectTerms.Plugin.ExportTermsToXML;
using System.IO;
using System;
using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using System.Windows.Forms;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [AutomaticTask("Coding.ProjectTerms.BatchTask",
        "Extract Project Terms",
        "Export project terms according the following criteria...",
        GeneratedFileType = AutomaticTaskFileType.BilingualTarget)]
    [AutomaticTaskSupportedFileType(AutomaticTaskFileType.BilingualTarget)]
    [RequiresSettings(typeof(ProjectTermsBatchTaskSettings), typeof(ProjectTermsBatchTaskSettingsPage))]
    public class ProjectTermsBatchTask : AbstractFileContentProcessingAutomaticTask
    {
        private HashSet<string> projectFiles;
        private FileBasedProject CurrentProject { get; set; }
        private ProjectTermsBatchTaskSettingsControl control = new ProjectTermsBatchTaskSettingsControl();
        private ProjectTermsBatchTaskSettings settings;
        private bool singleFileProject = false;

        private string GetProjectTermsName()
        {
            return CurrentProject.GetProjectInfo().Name;
        }

        protected override void OnInitializeTask()
        {
            CurrentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
            
            if (Utils.Utils.CheckSingleFileProject())
            {
                singleFileProject = true;
                return;
            }
            else
            {
                singleFileProject = false;
            }

            base.OnInitializeTask();

            projectFiles = new HashSet<string>();
            settings = GetSetting<ProjectTermsBatchTaskSettings>();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            if (singleFileProject) return;

            // skip repeated and project terms files
            if (projectFiles.Contains(projectFile.Name) || projectFile.Name.Contains(GetProjectTermsName())) return;

            control.ExtractProjectFileTerms(projectFile, multiFileConverter);
            projectFiles.Add(projectFile.Name);
        }

        public override void TaskComplete()
        {
            try
            {
                if (singleFileProject)
                {
                    MessageBox.Show(PluginResources.Error_SingleFileProject, PluginResources.MessageType_Error);
                    return;
                }

                if (!ProjectTermsBatchTaskSettingsControl.controlLoad)
                {
                    settings.ResetToDefaults();
                }

                control.ExtractProjectTerms(settings);

                AddXMlToProject(Project, Path.GetDirectoryName(ProjectTermsCache.GetXMLFilePath(control.ProjectPath)), false);
                CreateReport("Project terms", "Project terms report", GenerateReport());

                ProjectTermsBatchTaskSettingsControl.controlLoad = false;
            }
            catch (ProjectTermsException e)
            {
                MessageBox.Show(e.Message, PluginResources.MessageType_Error, MessageBoxButtons.OK);
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
                AutomaticTask preTran = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.PreTranslateFiles);
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_AddXMlToProject + e.Message);
            }
        }
    }
}
