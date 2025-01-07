using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using SdlXliff.Toolkit.Integration.Helpers;
using System.Reflection;

namespace SdlXliffToolkit
{
    [ApplicationInitializer]
    public class ApplicationInitializer : IApplicationInitializer
    {
        public static TradosView GetCurrentView()
        {
            var projectController = SdlTradosStudio.Application.GetController<ProjectsController>();

            var propertyInfo = projectController.GetType().BaseType.GetField("_studioWindow", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase);
            var studioWindow = propertyInfo.GetValue(projectController);

            var propertyInfo2 = studioWindow.GetType()
                .GetProperty("ActiveView", BindingFlags.Instance | BindingFlags.Public);

            var activeView = propertyInfo2.GetValue(studioWindow);

            return
                activeView.ToString().Contains("ProjectsView") ? TradosView.ProjectsView :
                activeView.ToString().Contains("FilesView") ? TradosView.FilesView :
                activeView.ToString().Contains("FilesView") ? TradosView.EditorView : 
                TradosView.OtherView;
        }

        public void Execute()
        {
            Log.Setup();
        }
    }
}