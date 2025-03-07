using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.IATETerminologyProvider
{
    [TerminologyProviderViewerWinFormsUI]
    internal class IATETerminologyProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
    {
        private IATETerminologyProvider _iateTerminologyProvider;
        private IATETermsControl _control;
        private DocumentStateService _documentStateService;

        public event EventHandler TermChanged;
        public event EventHandler<EntryEventArgs> SelectedTermChanged;
        public event Action<Entry> JumpToTermAction;
        public event Action<string, string> AddTermAction;

        public Control Control
        {
            get
            {
                _control = new IATETermsControl(_iateTerminologyProvider)
                {
                    Text = @"IATETerminologyProviderViewerWinFormsUI",
                    BackColor = Color.White
                };

                JumpToTermAction += _control.JumpToTerm;

                if (_documentStateService == null)
                {
                    _documentStateService = new DocumentStateService();
                }

                _documentStateService.UpdateDocumentEntriesState(_control);

                return _control;
            }
        }

        public bool Initialized => true;

        public Entry SelectedTerm { get; set; }

        bool ITerminologyProviderViewerWinFormsUI.CanAddTerm => false;

        public bool IsEditing => false;

        public void AddAndEditTerm(Entry term, string source, string target)
        {
        }

        public void AddTerm(string source, string target)
        {
            AddTermAction?.Invoke(source, target);
        }

        public void EditTerm(Entry term)
        {
        }

        public void Initialize(ITerminologyProvider terminologyProvider, CultureCode source, CultureCode target)
        {
            _iateTerminologyProvider = (IATETerminologyProvider)terminologyProvider;
        }

        public void JumpToTerm(Entry entry)
        {
            JumpToTermAction?.Invoke(entry);
        }

        public void Release()
        {
            if (_control == null)
            {
                return;
            }

            if (JumpToTermAction != null)
            {
                JumpToTermAction -= _control.JumpToTerm;
            }

            _documentStateService.SaveDocumentEntriesState(_control);

            _control?.ReleaseSubscribers();
        }

        public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
        {
            return terminologyProviderUri.Scheme == Constants.IATEGlossary;
        }

        public bool CanAddTerm() => true;

        public void CancelTerm()
        {

        }

        public void SaveTerm()
        {

        }
    }
}