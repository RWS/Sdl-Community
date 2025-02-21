using Autofac;
using InterpretBank.CommonServices;
using InterpretBank.Events;
using InterpretBank.TermbaseViewer.UI;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Windows.Forms;

namespace InterpretBank.Studio
{
    [TerminologyProviderViewerWinFormsUI]
    internal class InterpretBankProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
    {
        private TermbaseViewerControl _termbaseControl;

        public event EventHandler<EntryEventArgs> SelectedTermChanged;

        public event EventHandler TermChanged;

        public bool CanAddTerm => true;

        public Control Control
        {
            get
            {
                if (!Initialized) return null;
                    //new UserInteractionService().WarnUser("Not all project languages have been mapped. Please go to termbase settings and map them.");
                SetupTermbaseControl();
                return _termbaseControl;
            }
        }

        public bool Initialized
        {
            get;
            set;
        }

        public bool IsEditing => false;
        public Entry SelectedTerm { get; set; }

        private InterpretBankProvider InterpretBankProvider { get; set; }

        private Language SourceLanguage { get; set; }

        private Language TargetLanguage { get; set; }

        private ILifetimeScope TermbaseControlScope { get; } = ApplicationInitializer.ApplicationLifetimeScope.BeginLifetimeScope();
        private TermbaseViewerControl TermbaseViewerControl => (TermbaseViewerControl)Control;

        public void AddAndEditTerm(Entry term, string source, string target)
        {
        }

        public void AddTerm(string source, string target)
        {
            TermbaseViewerControl.AddTerm(source, target);
            TermChanged?.Invoke(this, EventArgs.Empty);
        }

        public void CancelTerm()
        {
        }

        public void EditTerm(Entry term)
        {
            //TermbaseViewerControl.EditTerm(term);
            TermChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Initialize(ITerminologyProvider terminologyProvider, CultureCode source, CultureCode target)
        {
            if (terminologyProvider is not InterpretBankProvider interpretBankProvider)
                return;

            InterpretBankProvider = interpretBankProvider;
            InterpretBankProvider.ProviderSettingsChanged -= InterpretBankProvider_ProviderSettingsChanged;
            InterpretBankProvider.ProviderSettingsChanged += InterpretBankProvider_ProviderSettingsChanged;

            TargetLanguage = LanguageRegistryApi.Instance.GetLanguage(target);
            SourceLanguage = LanguageRegistryApi.Instance.GetLanguage(source);

            StudioContext.EventAggregator.GetEvent<DbChangedEvent>().Subscribe(OnDbChanged);
            Initialized = true;
        }

        public void JumpToTerm(Entry entry)
        {
            TermbaseViewerControl.JumpToTerm(entry);
        }

        public void Release()
        {
        }

        public void SaveTerm()
        {
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
            => terminologyProviderUri.ToString().Contains(Constants.InterpretBankUri);

        private void InterpretBankProvider_ProviderSettingsChanged() => LoadTerms();

        private void LoadTerms()
        {
            if (!Initialized) return;
            _termbaseControl
                .LoadTerms(
                    SourceLanguage,
                    TargetLanguage,
                    InterpretBankProvider.Settings.Glossaries,
                    InterpretBankProvider.Settings.Tags,
                    InterpretBankProvider.Settings.UseTags,
                    InterpretBankProvider.Settings.DatabaseFilepath);
        }

        private void OnDbChanged(DbChangedEvent dbChangedEvent)
        {
            _termbaseControl.ReloadDb(dbChangedEvent.Filepath);
        }

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