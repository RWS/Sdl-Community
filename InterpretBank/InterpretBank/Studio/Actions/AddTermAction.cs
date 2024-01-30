//using Sdl.Desktop.IntegrationApi;
//using Sdl.Desktop.IntegrationApi.Extensions;
//using Sdl.TranslationStudioAutomation.IntegrationApi;
//using System;

//namespace InterpretBank.Studio.Actions
//{
//    [Action(nameof(AddTermAction), Icon = "AddTerm", Name = "Add new term",
//        Description = "Add new term to InterpretBank db", ContextByType = typeof(EditorController))]
//    [ActionLayout(typeof(InterpretBankRibbonGroup), 10, DisplayType.Large)]
//    public class AddTermAction : AbstractAction
//    {
//        protected override void Execute() => ActionManager.CurrentlyUsedTermbaseViewerControl.AddNewTerm();

//        public AddTermAction()
//        {
//            ActionManager.AddTermActive += enabled => Enabled = enabled;
//        }
//    }
//}