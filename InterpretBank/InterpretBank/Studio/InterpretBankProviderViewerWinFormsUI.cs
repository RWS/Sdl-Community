using Autofac;
using InterpretBank.TermbaseViewer.UI;
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
                SetupTermbaseControl();
                return _termbaseControl;
            }
        }

        public bool Initialized => true;

        public IEntry SelectedTerm { get; set; }

        private InterpretBankProvider InterpretBankProvider { get; set; }

        private Language SourceLanguage { get; set; }

        private Language TargetLanguage { get; set; }

        private ILifetimeScope TermbaseControlScope { get; } = ApplicationInitializer.Container.BeginLifetimeScope();
        private TermbaseViewerControl TermbaseViewerControl => (TermbaseViewerControl)Control;

        public void AddAndEditTerm(IEntry term, string source, string target)
        {
        }

        public void AddTerm(string source, string target) => TermbaseViewerControl.AddTerm(source, target);

        public void EditTerm(IEntry term)
        {
            TermbaseViewerControl.EditTerm(term);
            //TermChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
        {
            if (terminologyProvider is not InterpretBankProvider interpretBankProvider)
                return;

            InterpretBankProvider = interpretBankProvider;
            InterpretBankProvider.ProviderSettingsChanged += InterpretBankProvider_ProviderSettingsChanged;

            var currentProject = StudioContext.ProjectsController.CurrentProject;
            var targetLanguages = currentProject.GetTargetLanguageFiles().Select(p => p.Language);

            TargetLanguage = targetLanguages.FirstOrDefault(l => l.CultureInfo.Equals(target));
            SourceLanguage = currentProject.GetProjectInfo().SourceLanguage;
        }

        public void JumpToTerm(IEntry entry) => TermbaseViewerControl.JumpToTerm(entry);

        public void Release()
        {
            TermbaseControlScope.Dispose();
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
            => terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);

        private void InterpretBankProvider_ProviderSettingsChanged() => LoadTerms();

        private void LoadTerms() =>
            _termbaseControl
                .LoadTerms(SourceLanguage, TargetLanguage,
                    InterpretBankProvider.Settings.Glossaries,
                    InterpretBankProvider.TermSearchService);

        private void SetupTermbaseControl()
        {
            if (_termbaseControl is null)
            {
                _termbaseControl = TermbaseControlScope.Resolve<TermbaseViewerControl>();
                LoadTerms();
            }
            else _termbaseControl.ReloadTerms(SourceLanguage, TargetLanguage);
        }
    }
}