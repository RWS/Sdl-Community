using Sdl.Community.AntidoteVerifier.TellMe.WarningWindow;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    public class CorrectorAction : AbstractTellMeAction
    {
        public CorrectorAction()
        {
            Name = $"{PluginResources.Plugin_Name} Corrector";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

        public override Icon Icon => PluginResources.boutonCorrecteur;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            var activeDocument = ApplicationContext.EditorController.ActiveDocument;
            if (activeDocument is null)
            {
                ShowInfo();
                return;
            }

            SdlTradosStudio.Application.ExecuteAction<AntidoteCorrectorAction>();
        }

        private void ShowInfo()
        {
            var infoWindow = new SettingsActionWarning(PluginResources.CorrectorAction_NoFileOpened);
            infoWindow.ShowDialog();
        }
    }
}