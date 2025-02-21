using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.Desktop.IntegrationApi.Notifications.Events;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ProjectTerms.Plugin
{
    [ApplicationInitializer]
    public sealed class ApplicationContext : IApplicationInitializer
    {
        private static ProjectsController _projectsController;
        private static FilesController _filesController;

        public static ProjectsController ProjectsController =>
            _projectsController ??= SdlTradosStudio.Application.GetController<ProjectsController>();

        public static FilesController FilesController =>
            _filesController ??= SdlTradosStudio.Application.GetController<FilesController>();

        public static bool ProjectsControllerIsActive { get; private set; }

        public static bool FilesControllerIsActive { get; private set; }

        public void Execute()
        {
            SdlTradosStudio.Application.GetService<IStudioEventAggregator>()
                .GetEvent<StudioWindowCreatedNotificationEvent>().Subscribe(OnStudioWindowCreated);
        }

        private void OnStudioWindowCreated(StudioWindowCreatedNotificationEvent obj)
        {
            ProjectsController.ActivationChanged += ProjectsController_ActivationChanged;
            FilesController.ActivationChanged += FilesController_ActivationChanged;
        }
        private void ProjectsController_ActivationChanged(object sender, ActivationChangedEventArgs e)
        {
            ProjectsControllerIsActive = e.Active;
        }

        private void FilesController_ActivationChanged(object sender, ActivationChangedEventArgs e)
        {
            FilesControllerIsActive = e.Active;
        }
    }
}
