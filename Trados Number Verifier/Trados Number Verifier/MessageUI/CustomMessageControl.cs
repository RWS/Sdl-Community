using System;
using System.Windows.Forms;
using Sdl.DesktopEditor.BasicControls;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using System.Collections.Generic;

namespace Sdl.Verification.Sdk.EditAndApplyChanges.MessageUI
{
    /// <summary>
    /// CustomMessageControl class is responsible for displaying custom messages.
    /// </summary>
    public partial class CustomMessageControl : UserControl, ISuggestionProvider, ISegmentChangedAware
    {
        /// <summary>
        /// Source segment edit control
        /// </summary>
        private readonly BasicSegmentEditControl _sourceSegmentControl = new BasicSegmentEditControl();

        /// <summary>
        /// Target segment edit control
        /// </summary>
        private readonly BasicSegmentEditControl _targetSegmentControl = new BasicSegmentEditControl();

        /// <summary>
        /// true if target segment was manually edited
        /// </summary>
        private bool _hasSegmentChanged = false;

        /// <summary>
        /// Constructor that takes the given message event args, bilingual document, source segment and target segment.
        /// </summary>
        /// <param name="messageEventArgs">message event arguments</param>
        /// <param name="bilingualDocument">bilingual document</param>
        /// <param name="sourceSegment">source segment</param>
        /// <param name="targetSegment">target segment</param>
        public CustomMessageControl(MessageEventArgs messageEventArgs, IBilingualDocument bilingualDocument, ISegment sourceSegment, ISegment targetSegment)
        {
            MessageEventArgs = messageEventArgs;
            BilingualDocument = bilingualDocument;
            SourceSegment = sourceSegment;
            TargetSegment = targetSegment;
            InitializeComponent();
            
            _sourceSegmentControl.Dock = DockStyle.Fill;
            _sourceSegmentPanel.Controls.Add(_sourceSegmentControl);

            _targetSegmentControl.Dock = DockStyle.Fill;
            _targetSegmentPanel.Controls.Add(_targetSegmentControl);

            UpdateMessage(messageEventArgs);
            UpdateSourceSegment(sourceSegment);
            UpdateTargetSegment((ISegment)targetSegment.Clone());
            UpdateProblemDescription(messageEventArgs);
            UpdateSuggestions(messageEventArgs);

            // make the target segment editable
            _targetSegmentControl.IsReadOnly = false;

            _suggestionsList.SelectedIndexChanged += _suggestionsList_SelectedIndexChanged;
        }

        /// <summary>
        /// MessageEventArgs property represents the message event arguments.
        /// </summary>
        public MessageEventArgs MessageEventArgs
        {
            get;
            private set;
        }

        /// <summary>
        /// BilingualDocument property represents the bilingual document.
        /// </summary>
        public IBilingualDocument BilingualDocument
        {
            get;
            private set;
        }

        /// <summary>
        /// SourceSegment property represents the source segment.
        /// </summary>
        public ISegment SourceSegment
        {
            get;
            private set;
        }

        /// <summary>
        /// TargetSegment property represents the target segment.
        /// </summary>
        public ISegment TargetSegment
        {
            get;
            private set;
        }

        #region ISegmentChangedAware implementation
        /// <summary>
        /// Returns true if object was manually edited
        /// </summary>
        public bool HasSegmentChanged
        {
            get { return _hasSegmentChanged; }
        }

        /// <summary>
        /// Returns edited segment
        /// </summary>
        public ISegment EditedSegment
        {
            get { return _targetSegmentControl.GetDocumentSegment(); }
        }

        /// <summary>
        /// The paragraph unit ID for edited segment in the original document.
        /// Note: The segment may not reference the original document so the paragraph ID may be null.
        /// </summary>
        public ParagraphUnitId? TargetParagraphId
        {
            get { return MessageEventArgs.FromLocation.ParagrahUnitId; }
        }

        /// <summary>
        /// The segment ID for the edited segment in the original document.
        /// </summary>
        public SegmentId? TargetSegmentId
        {
            get { return MessageEventArgs.FromLocation.SegmentId;  } 
        }

        /// <summary>
        /// Reset target segment content to original value and
        /// re-enable suggestions list
        /// </summary>
        public void ResetSegment()
        {
            UpdateTargetSegment(TargetSegment);
        }

        public event EventHandler<EventArgs> SegmentChanged;

        #endregion

        #region Private members
        /// <summary>
        /// Updates the message from the given message event arguments.
        /// </summary>
        /// <param name="messageEventArgs">message event arguments</param>
        private void UpdateMessage(MessageEventArgs messageEventArgs)
        {
            _messageTextBox.Text = messageEventArgs.Message;
            switch (messageEventArgs.Level)
            {
                case Sdl.FileTypeSupport.Framework.NativeApi.ErrorLevel.Error:
                    //l_Severity.Image = Resources.error;
                    break;
                case Sdl.FileTypeSupport.Framework.NativeApi.ErrorLevel.Note:
                    //l_Severity.Image = Resources.information2;
                    break;
                case Sdl.FileTypeSupport.Framework.NativeApi.ErrorLevel.Warning:
                    //l_Severity.Image = Resources.warning;
                    break;
                case Sdl.FileTypeSupport.Framework.NativeApi.ErrorLevel.Unspecified:
                default:
                    break;
            }
        }

        /// <summary>
        /// Updates the source segment using the given target segment.
        /// </summary>
        /// <param name="targetSegment">target segment</param>
        private void UpdateTargetSegment(ISegment targetSegment)
        {
            // don't listen for events when contents are reset
            _targetSegmentControl.SegmentContentChanged -= OnSegmentContentChanged;

            // show target segment in segment control
            _targetSegmentControl.ReplaceDocumentSegment((ISegment)targetSegment.Clone());

            _hasSegmentChanged = false;

            // make the target segment editable
            _targetSegmentControl.IsReadOnly = false;

            // start listening to changes again 
            _targetSegmentControl.SegmentContentChanged += OnSegmentContentChanged;

            _suggestionsList.Enabled = true;
        }

        /// <summary>
        /// Updates the actual segment using the given target segment.
        /// </summary>
        /// <param name="sourceSegment">target segment</param>
        private void UpdateSourceSegment(ISegment sourceSegment)
        {
            // show target segment in segment control
            _sourceSegmentControl.ReplaceDocumentSegment(sourceSegment);
        }

        /// <summary>
        /// Updates the problem description using the given message event arguments.
        /// </summary>
        /// <param name="messageEventArgs">message event arguments</param>
        private void UpdateProblemDescription(MessageEventArgs messageEventArgs)
        {
            CustomMessageData qaCheckerMessageData = (CustomMessageData) messageEventArgs.ExtendedData;
            _problemDescriptionTextBox.Text = qaCheckerMessageData.DetailedDescription;
        }

        /// <summary>
        /// Reads the suggestions from ExtendedMessageEventData
        /// </summary>
        /// <param name="messageEventArgs"></param>
        private void UpdateSuggestions(MessageEventArgs messageEventArgs)
        {
            CustomMessageData messageData = messageEventArgs.ExtendedData as CustomMessageData;
            foreach (var item in messageData.SuggestedChanges)
            {
                Suggestion suggestion = new Suggestion(messageEventArgs.FromLocation, messageEventArgs.UptoLocation,
                    CreateSuggestionMarkup(item.Key));
                _suggestionsList.Items.Add(suggestion);
            }
            if (_suggestionsList.Items.Count > 0)
            {
                _suggestionsList.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Creates suggestion markup for the given suggestion text.
        /// </summary>
        /// <param name="suggestionText">suggestion text</param>
        /// <returns>suggestion markup</returns>
        private IAbstractMarkupData CreateSuggestionMarkup(string suggestionText)
        {
            if (string.IsNullOrEmpty(suggestionText))
            {
                return null;
            }

            return BilingualDocument.ItemFactory.CreateText(BilingualDocument.PropertiesFactory.CreateTextProperties(suggestionText));
        }

        /// <summary>
        /// Fires the SuggestionChanged event when the selected suggestion is changed in the suggestions list.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="eventArgs">event arguments</param>
        private void _suggestionsList_SelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if (SuggestionChanged != null)
            {
                SuggestionChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handle content changed event
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="args">Always null</param>
        private void OnSegmentContentChanged(object sender, EventArgs args)
        {
            // suggestions are no longer valid when
            // user starts to modify target segment manually
            _suggestionsList.Enabled = false;
            _hasSegmentChanged = true;
            if (SegmentChanged != null)
            {
                SegmentChanged(this, null);
            }
        }

        #endregion

        #region ISuggestionProvider Members

        public event EventHandler SuggestionChanged;
        
        public bool HasSuggestion()
        {
            return _suggestionsList.Items.Count != 0;
        }

        public Suggestion GetSuggestion()
        {
            return _suggestionsList.SelectedItem as Suggestion;
        }

        #endregion
    }
}
