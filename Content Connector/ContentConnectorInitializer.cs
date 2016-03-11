using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.ContentConnector
{
    [ApplicationInitializer]
    class ContentConnectorInitializer : IApplicationInitializer
    {
        public void Execute()
        {
            ContentConnectorViewController contentConnector = SdlTradosStudio.Application.GetController<ContentConnectorViewController>();
            contentConnector.CheckForProjects();

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
