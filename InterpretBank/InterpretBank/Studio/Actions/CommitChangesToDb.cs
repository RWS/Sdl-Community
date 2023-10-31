using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;

namespace InterpretBank.Studio.Actions
{
    [Action(nameof(CommitChangeToDb), Icon = "CommitToDb", Name = "Commit changes",
        Description = "Commit changes to InterpretBank db", ContextByType = typeof(EditorController))]
    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
    public class CommitChangeToDb : AbstractAction
    {
        protected override void Execute() => ActionManager.CurrentlyUsedTermbaseViewerControl.CommitToDatabase();

        public CommitChangeToDb()
        {
            ActionManager.CommitChangesActive += enabled => Enabled = enabled;
        }
    }
}