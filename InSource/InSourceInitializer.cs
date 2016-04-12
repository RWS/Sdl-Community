using System.Windows.Forms;
using Sdl.Community.InSource.Insights;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.InSource
{
    [ApplicationInitializer]
    class InSourceInitializer : IApplicationInitializer
    {
        public void Execute()
        {
            InSourceViewController contentConnector = SdlTradosStudio.Application.GetController<InSourceViewController>();
            contentConnector.CheckForProjects();
            TelemetryService.Instance.Init();
            if (contentConnector.ProjectRequests.Count > 0)
            {
                if (MessageBox.Show(
                        Form.ActiveForm,
                        @"You have new project requests. Would you like to check the project requests now?",
                        @"New Project Requests",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    contentConnector.Activate();
                }
            }
            
        }
    }
}
