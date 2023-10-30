using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(AddTermAction), Icon = "AddTerm", Name = "Add new term",
        Description = "Add new term to InterpretBank db", ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class AddTermAction : AbstractAction
    {
        public AddTermAction()
        {
            ServiceManager.TermbaseActiveEvent += ServiceManager_TermbaseActiveEvent;
            Enabled = false;
        }

        protected override void Execute() => ServiceManager.PreviouslyUsedTermbaseViewerControl.AddNewTerm();

        private void ServiceManager_TermbaseActiveEvent(bool activeTermbase) => Enabled = activeTermbase;
    }
}