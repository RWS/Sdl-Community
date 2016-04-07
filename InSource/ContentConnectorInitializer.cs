using System.Windows.Forms;
using Sdl.Community.InSource.Insights;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InSource
{
    [ApplicationInitializer]
    class ContentConnectorInitializer : IApplicationInitializer
    {
        public void Execute()
        {
            ContentConnectorViewController contentConnector = SdlTradosStudio.Application.GetController<ContentConnectorViewController>();
            contentConnector.CheckForProjects();
            TelemetryService.Instance.Init();
            if (contentConnector.ProjectRequests.Count > 0)
            {
                if (MessageBox.Show(
                        Form.ActiveForm,
                        "You have new project requests. Would you like to create projects now?",
                        "New Project Requests",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    contentConnector.Activate();
                }
            }
            
        }
    }
}
