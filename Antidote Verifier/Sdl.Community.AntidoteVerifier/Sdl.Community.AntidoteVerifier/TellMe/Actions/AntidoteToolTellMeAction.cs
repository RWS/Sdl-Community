using Sdl.Community.AntidoteVerifier.TellMe.WarningWindow;
using Sdl.TellMe.ProviderApi;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    /// <summary>
    /// Shared behavior for the three Antidote tool actions (Corrector, Dictionaries, Guides):
    /// consistent naming and category, plus a guard that shows the TellMe warning window when no
    /// document is open in the editor before delegating to the corresponding ribbon action.
    /// </summary>
    public abstract class AntidoteToolTellMeAction : AbstractTellMeAction
    {
        protected AntidoteToolTellMeAction(string toolDisplayName)
        {
            Name = $"{PluginResources.Plugin_Name} {toolDisplayName}";
        }

        public override string Category =>
            string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

        public override bool IsAvailable => true;

        public override void Execute()
        {
            if (ApplicationContext.EditorController.ActiveDocument is null)
            {
                new SettingsActionWarning(PluginResources.CorrectorAction_NoFileOpened).ShowDialog();
                return;
            }

            ExecuteTool();
        }

        /// <summary>Invokes the ribbon action for the tool; a document is guaranteed to be open.</summary>
        protected abstract void ExecuteTool();
    }
}
