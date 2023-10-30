using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(CommitChangeToDb), Icon = "CommitChanges", Name = "Commit changes",
        Description = "Commit changes to InterpretBank db", ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class CommitChangeToDb : AbstractAction
    {
        public CommitChangeToDb()
        {
            Enabled = false;
            ServiceManager.TermbaseActiveEvent += ServiceManager_TermbaseActiveEvent;
        }

        private void ServiceManager_TermbaseActiveEvent(bool termbaseActive)
        {
            if (termbaseActive)
            {
                Enabled = ServiceManager.PreviouslyUsedTermbaseViewerControl.AnyEditedTerms;
                ServiceManager.PreviouslyUsedTermbaseViewerControl.AnyEditedTermsChanged += PreviouslyUsedTermbaseViewerControl_AnyEditedTermsChanged;
            }
            else
            {
                Enabled = false;
            }
        }

        private void PreviouslyUsedTermbaseViewerControl_AnyEditedTermsChanged(bool anyEditedTerms) =>
            Enabled = anyEditedTerms;

        protected override void Execute() => ServiceManager.PreviouslyUsedTermbaseViewerControl.CommitToDatabase();
    }
}