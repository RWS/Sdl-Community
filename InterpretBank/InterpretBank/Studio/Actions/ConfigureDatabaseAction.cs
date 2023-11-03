using Autofac;
using InterpretBank.Interface;
using InterpretBank.SettingsService.UI;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Linq;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(ConfigureDatabaseAction), Icon = "logo", Name = "Configure database",
        Description = "InterpretBank database configuration")]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class ConfigureDatabaseAction : AbstractAction
    {
        protected override void Execute()
        {
            if (CloseOpenDocuments())
            {
                ShowGlossarySetupDialog();
            }
        }

        private bool CloseOpenDocuments()
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            var studioDocuments = editorController.GetDocuments();

            if (studioDocuments.Any() && ShouldCloseDocuments())
            {
                foreach (var document in studioDocuments)
                {
                    editorController.Save(document);
                    editorController.Close(document);
                }
                return true;
            }

            return !studioDocuments.Any(); // No documents open or user chose not to close them.
        }

        private bool ShouldCloseDocuments()
        {
            return ApplicationInitializer.ApplicationLifetimeScope
                .Resolve<IUserInteractionService>()
                .Confirm("For this action, all opened documents will be closed.\r\nDo you wish to continue?");
        }

        private void ShowGlossarySetupDialog()
        {
            ApplicationInitializer.ApplicationLifetimeScope.Resolve<GlossarySetup>().ShowDialog();
        }
    }
}