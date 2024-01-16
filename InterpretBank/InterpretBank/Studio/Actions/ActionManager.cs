//using InterpretBank.TermbaseViewer.UI;
//using Sdl.TranslationStudioAutomation.IntegrationApi;
//using System;

//namespace InterpretBank.Studio.Actions
//{
//    public static class ActionManager
//    {
//        private static TermbaseViewerControl _previouslyUsedTermbaseViewerControl;

//        //public static event Action<bool> AddTermActive;
//        public static event Action<bool> CommitChangesActive;

//        static ActionManager()
//        {
//            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
//            editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
//        }

//        public static TermbaseViewerControl CurrentlyUsedTermbaseViewerControl
//        {
//            get => _previouslyUsedTermbaseViewerControl;
//            set
//            {
//                _previouslyUsedTermbaseViewerControl = value;

//                //AddTermActive?.Invoke(value != null);

//                if (value != null)
//                {
//                    //CommitChangesActive?.Invoke(value.AnyEditedTerms);
//                    value.AnyEditedTermsChanged += CurrentlyUsedTermbaseViewer_AnyEditedTermsChanged;
//                }
//                else
//                {
//                    //CommitChangesActive?.Invoke(false);
//                }
//            }
//        }

//        private static void CurrentlyUsedTermbaseViewer_AnyEditedTermsChanged(bool anyEditedTerms) =>
//            //CommitChangesActive?.Invoke(anyEditedTerms);

//        private static void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e) =>
//            CurrentlyUsedTermbaseViewerControl = null;
//    }
//}