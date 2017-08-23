using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Reflection;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsBatchTaskSettingsPage : DefaultSettingsPage<ProjectTermsBatchTaskSettingsControl, ProjectTermsBatchTaskSettings>
    {
        public ProjectTermsBatchTaskSettingsPage()
        {
            var project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
            FieldInfo projectVal = typeof(FileBasedProject).GetField(PluginResources.Constant_ProjectType, BindingFlags.NonPublic | BindingFlags.Instance);

            dynamic projectValDynamic = projectVal.GetValue(project);
            dynamic projectType = projectValDynamic.ProjectType != null ? projectValDynamic.ProjectType : string.Empty;

            string projectTypeContent = Convert.ToString(projectType);

            if (projectTypeContent.Equals(PluginResources.Constant_ProjectTypeContent))
            {
                ProjectTermsBatchTaskSettingsControl.ControlDisabled = true;
            }
            else
            {
                ProjectTermsBatchTaskSettingsControl.ControlDisabled = false;
            }
        }
    }
}
