using InterpretBank.CommonServices;
using InterpretBank.GlossaryService;
using InterpretBank.GlossaryService.Interface;
using InterpretBank.SettingsService.UI;
using InterpretBank.SettingsService.ViewModel;
using InterpretBank.Studio;
using InterpretBank.TermbaseViewer.UI;
using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using ExchangeService = InterpretBank.GlossaryExchangeService.GlossaryExchangeService;

namespace InterpretBank
{
    public static class ServiceManager
    {
        private static UserInteractionService _dialogService;
        private static ExchangeService _glossaryExchangeService;
        private static GlossarySetupViewModel _glossarySetupViewModel;
        private static TermbaseViewerControl _previouslyUsedTermbaseViewerControl;

        static ServiceManager()
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
            editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;
        }

        public static event Action<bool> TermbaseActiveEvent;

        public static UserInteractionService DialogService { get; } = _dialogService ??= new UserInteractionService();

        public static TermbaseViewerControl PreviouslyUsedTermbaseViewerControl
        {
            get => _previouslyUsedTermbaseViewerControl;
            set
            {
                _previouslyUsedTermbaseViewerControl = value;
                TermbaseActiveEvent?.Invoke(value != null);
            }
        }

        private static ExchangeService GlossaryExchangeService { get; } = _glossaryExchangeService ??= new ExchangeService();

        private static GlossarySetup GlossarySetup => new(GlossarySetupViewModel);

        private static GlossarySetupViewModel GlossarySetupViewModel { get; } = _glossarySetupViewModel ??=
                    new GlossarySetupViewModel(DialogService, GlossaryExchangeService, InterpretBankDataContext);

        private static IInterpretBankDataContext InterpretBankDataContext => new InterpretBankDataContext();

        public static TermbaseViewerControl GetTermbaseControl(InterpretBankProvider interpretBankProvider, Language sourceLanguage, Language targetLanguage)
        {
            var termbaseViewerViewModel = new TermbaseViewerViewModel(interpretBankProvider.TermSearchService, DialogService);
            termbaseViewerViewModel.LoadTerms(sourceLanguage, targetLanguage, interpretBankProvider.Settings.Glossaries);

            var termbaseViewer = new TermbaseViewer.UI.TermbaseViewer { DataContext = termbaseViewerViewModel };
            PreviouslyUsedTermbaseViewerControl = new TermbaseViewerControl(termbaseViewer);
            return PreviouslyUsedTermbaseViewerControl;
        }

        public static void ShowGlossarySetup()
        {
            GlossarySetupViewModel.Setup();
            GlossarySetup.ShowDialog();
            InterpretBankDataContext.Dispose();
        }

        private static void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e) =>
                                                            PreviouslyUsedTermbaseViewerControl = null;
    }
}