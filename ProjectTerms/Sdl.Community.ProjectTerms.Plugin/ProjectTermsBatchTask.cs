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
using System.Xml.Linq;

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
        private ProjectTermsBatchTaskSettingsControl control;
        private ProjectTermsBatchTaskSettings settings;
        private bool fileIncluded = false;

        protected override void OnInitializeTask()
        {
            if (ProjectTermsBatchTaskSettingsControl.ControlDisabled) return;

            base.OnInitializeTask();

            projectFiles = new HashSet<string>();
            control = new ProjectTermsBatchTaskSettingsControl();
            settings = GetSetting<ProjectTermsBatchTaskSettings>();
        }

        protected override void ConfigureConverter(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            if (ProjectTermsBatchTaskSettingsControl.ControlDisabled) return;

            var projectTermsFileName = Path.GetFileNameWithoutExtension(ProjectTermsCache.GetXMLFilePath(control.ProjectPath));
            var projectTermsStartFileName = projectTermsFileName.Split('_');
            if (projectFiles.Contains(projectFile.Name)) return;

            if (projectFile.Name.Contains(projectTermsStartFileName[0]))
            {
                fileIncluded = true;
                return;
            }

            control.ExtractProjectFileTerms(projectFile, multiFileConverter);
            projectFiles.Add(projectFile.Name);
        }

        public override void TaskComplete()
        {
            try
            {
                if (ProjectTermsBatchTaskSettingsControl.ControlDisabled) return;

                if (settings.BlackListSettings == null)
                {
                    MessageBox.Show(PluginResources.MessageContent_SkipSettings, PluginResources.MessageType_Info);
                    return;
                }
                
                if (fileIncluded)
                {
                    string path = ProjectTermsCache.GetXMLFilePath(SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().LocalProjectFolder);
                    Utils.Utils.CreateDirectory(Path.GetDirectoryName(path));
                    XDocument doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
                    doc.Add(new XElement("projectTerms", string.Empty));
                    doc.Save(path);
                } else
                {
                    control.ExtractProjectTerms(settings);
                    CreateReport("Project terms", "Project terms report", GenerateReport());
                }

                AddXMlToProject(Project, Path.GetDirectoryName(ProjectTermsCache.GetXMLFilePath(control.ProjectPath)), false);
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

                Utils.Utils.RemoveDirectory(xmlFolder);
            }
            catch (Exception e)
            {
                throw new ProjectTermsException(PluginResources.Error_AddXMlToProject + e.Message);
            }
        }
    }
}
