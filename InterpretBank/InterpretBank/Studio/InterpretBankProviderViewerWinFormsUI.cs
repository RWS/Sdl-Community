using Autofac;
using InterpretBank.Events;
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

        public bool IsEditing => false;

        public Control Control
        {
            get
            {
                SetupTermbaseControl();
                return _termbaseControl;
            }
        }

        public bool Initialized => true;

        public Entry SelectedTerm { get; set; }

        private InterpretBankProvider InterpretBankProvider { get; set; }

        private Language SourceLanguage { get; set; }

        private Language TargetLanguage { get; set; }

        private ILifetimeScope TermbaseControlScope { get; } = ApplicationInitializer.ApplicationLifetimeScope.BeginLifetimeScope();
        private TermbaseViewerControl TermbaseViewerControl => (TermbaseViewerControl)Control;

        public void AddAndEditTerm(Entry term, string source, string target)
        {
        }

        public void CancelTerm()
        {
            
        }

        public void SaveTerm()
        {
            
        }

        public void Initialize(ITerminologyProvider terminologyProvider, CultureCode source, CultureCode target)
        {
            if (terminologyProvider is not InterpretBankProvider interpretBankProvider)
                return;

            InterpretBankProvider = interpretBankProvider;
            InterpretBankProvider.ProviderSettingsChanged -= InterpretBankProvider_ProviderSettingsChanged;
            InterpretBankProvider.ProviderSettingsChanged += InterpretBankProvider_ProviderSettingsChanged;

            var currentProject = StudioContext.ProjectsController.CurrentProject;
            var targetLanguages = currentProject.GetTargetLanguageFiles().Select(p => p.Language);


            TargetLanguage = targetLanguages.FirstOrDefault(l => l.CultureInfo.IetfLanguageTag.Equals(target.Name));
            SourceLanguage = currentProject.GetProjectInfo().SourceLanguage;

            StudioContext.EventAggregator.GetEvent<DbChangedEvent>().Subscribe(OnDbChanged);
        }

        public void AddTerm(string source, string target)
        {
            TermbaseViewerControl.AddTerm(source, target);
            TermChanged?.Invoke(this, EventArgs.Empty);
        }

        public void EditTerm(Entry term)
        {
            //TermbaseViewerControl.EditTerm(term);
            TermChanged?.Invoke(this, EventArgs.Empty);
        }

       

        private void OnDbChanged(DbChangedEvent dbChangedEvent)
        {
            _termbaseControl.ReloadDb(dbChangedEvent.Filepath);
        }

        public void JumpToTerm(Entry entry)
        {
            // TermbaseViewerControl.JumpToTerm(entry);
        }

        public void Release()
        {
        }

        public bool CanAddTerm => true;

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
            => terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);

        private void InterpretBankProvider_ProviderSettingsChanged() => LoadTerms();

        private void LoadTerms() =>
            _termbaseControl
                .LoadTerms(
                    SourceLanguage,
                    TargetLanguage,
                    InterpretBankProvider.Settings.Glossaries,
                    InterpretBankProvider.Settings.DatabaseFilepath);

        private void SetupTermbaseControl()
        {
            if (_termbaseControl is null)
            {
                _termbaseControl = TermbaseControlScope.Resolve<TermbaseViewerControl>();
                LoadTerms();
            }
            else
            {
                _termbaseControl.ReloadTerms(SourceLanguage, TargetLanguage);
            }
        }
    }
}