using InterpretBank.CommonServices;
using InterpretBank.Studio.Actions;
using InterpretBank.TermbaseViewer.UI;
using InterpretBank.TermbaseViewer.ViewModel;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace InterpretBank.Studio
{
    [TerminologyProviderViewerWinFormsUI]
    internal class InterpretBankProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
    {
        private TermbaseViewerControl _termbaseControl;

        public event EventHandler<EntryEventArgs> SelectedTermChanged;

        public event EventHandler TermChanged;

        public Control Control
        {
            get
            {
                ActionManager.CurrentlyUsedTermbaseViewerControl = _termbaseControl;
                if (_termbaseControl is not null) return _termbaseControl;

                var termbaseViewerViewModel = new TermbaseViewerViewModel(InterpretBankProvider.TermSearchService, UserInteractionService.Instance);
                termbaseViewerViewModel.LoadTerms(SourceLanguage, TargetLanguage, InterpretBankProvider.Settings.Glossaries);

                var termbaseViewer = new TermbaseViewer.UI.TermbaseViewer { DataContext = termbaseViewerViewModel };

                _termbaseControl = new TermbaseViewerControl(termbaseViewer);
                ActionManager.CurrentlyUsedTermbaseViewerControl = _termbaseControl;
                return _termbaseControl;
            }
        }

        public bool Initialized => true;
        public IEntry SelectedTerm { get; set; }
        private InterpretBankProvider InterpretBankProvider { get; set; }
        private Language SourceLanguage { get; set; }
        private Language TargetLanguage { get; set; }
        private TermbaseViewerControl TermbaseViewerControl => (TermbaseViewerControl)Control;

        public void AddAndEditTerm(IEntry term, string source, string target)
        {
        }

        public void AddTerm(string source, string target) => TermbaseViewerControl.AddTerm(source, target);

        public void EditTerm(IEntry term) => TermbaseViewerControl.EditTerm(term);

        public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
        {
            if (terminologyProvider is not InterpretBankProvider interpretBankProvider)
                return;

            InterpretBankProvider = interpretBankProvider;

            var currentProject = StudioContext.ProjectsController.CurrentProject;
            var targetLanguages = currentProject.GetTargetLanguageFiles().Select(p => p.Language);

            TargetLanguage = targetLanguages.FirstOrDefault(l => l.CultureInfo.Equals(target));
            SourceLanguage = currentProject.GetProjectInfo().SourceLanguage;
        }

        public void JumpToTerm(IEntry entry) => TermbaseViewerControl.JumpToTerm(entry);

        public void Release()
        {
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
            => terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);
    }
}