using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Trados.Transcreate
{
    [ApplicationInitializer]
    internal class ApplicationInstance : IApplicationInitializer
    {
        private static ProjectsController _projectsController;

        private static ProjectsController ProjectsController =>
            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public static Form GetActiveForm()
        {
            var allForms = System.Windows.Forms.Application.OpenForms;
            var activeForm = allForms[allForms.Count - 1];
            foreach (Form form in allForms)
            {
                if (form.GetType().Name == "StudioWindowForm")
                {
                    activeForm = form;
                    break;
                }
            }

            return activeForm;
        }

        public static FileBasedProject GetSelectedProject()
        {
            var currentProject = ProjectsController.CurrentProject;

            var projectsTotal = ProjectsController.SelectedProjects.Count();
            if (projectsTotal == 0 && currentProject != null) projectsTotal = 1;

            return projectsTotal is > 1 or 0
                ? null
                : ProjectsController.SelectedProjects.FirstOrDefault() ?? currentProject;
        }

        public static void InitializeTranscreateViewController() =>
                    SdlTradosStudio.Application.GetController<TranscreateViewController>().Initialize();

        public void Execute()
        {
            SetApplicationShutdownMode();

            Common.Log.Setup();
        }

        private static void SetApplicationShutdownMode()
        {
            if (Application.Current == null)
            {
                // initialize the environments application instance
                new Application();
            }

            if (Application.Current != null)
            {
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
        }
    }
}