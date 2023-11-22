using System;
using System.Drawing;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.FileType.Preview
{
    public class InternalPreviewController : ISingleFilePreviewControl, INavigablePreview, IPreviewUpdatedViaRefresh, IDisposable
    {
        TempFileManager _previewFile;
        readonly InternalPreviewControl _control; // the actual control object
        private bool _disposed; // used for properly disposing of the control        
        FileId _fileId; // the actual file ID

        // indicates whether a file has already been opened in the preview, 
        // so we know if we should close it during a Refresh()  
        bool _isFileOpen;

        #region Initialize the Preview Control
        /// <summary>
        /// initialize the preview controller and preview control
        /// </summary>
        public InternalPreviewController()
        {
            _control = new InternalPreviewControl();
        }

        /// <summary>
        /// used for disposing of the control
        /// </summary>
        ~InternalPreviewController()
        {
            Dispose(false);
        }
        #endregion

        #region Implementation of ISingleFilePreviewControl

        public Control Control => _control;

        /// <summary>
        /// handler for the WindowSelectionChanged event from the preview control,
        /// raises the corresponding event on the INavigablePreview interface.
        /// </summary>
        /// <param name="component"></param>
        void ControlWindowSelectionChanged(IInteractivePreviewComponent component)
        {
            var marker = _control.GetSelectedSegment();
            var selectedSegment = new SegmentReference(_fileId, marker.ParagraphUnitId, marker.SegmentId);

            // raise the event
            OnSegmentSelected(this, new SegmentSelectedEventArgs(this, selectedSegment));
        }

        /// <summary>
        /// refresh the file in the preview control
        /// </summary>
        public void Refresh()
        {
            if (_isFileOpen)
            {
                _control.WindowSelectionChanged -= ControlWindowSelectionChanged;
                _control.Close();
            }

            // show the preview file in the control
            _control.OpenTarget(_previewFile.FilePath);

            // attach event handler
            _control.WindowSelectionChanged += ControlWindowSelectionChanged;

            _isFileOpen = true;

        }

        /// <summary>
        /// access to the temporary preview file
        /// </summary>
        public TempFileManager PreviewFile
        {
            get => _previewFile;
            set => _previewFile = value;
        }

        #endregion

        #region INavigablePreview
        /// <summary>
        /// reference to the current segment
        /// </summary>
        public void NavigateToSegment(SegmentReference segment)
        {
            _fileId = segment.FileId;
            _control.ScrollToSegment(segment);
        }

        /// <summary>
        /// communicates the preferred highlighting in the control
        /// not used in this implementation
        /// </summary>
        public Color PreferredHighlightColor { get; set; }

        public event EventHandler<SegmentSelectedEventArgs> SegmentSelected;

        /// <summary>
        /// custom implementation - raise the <see cref="SegmentSelected"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public virtual void OnSegmentSelected(object sender, SegmentSelectedEventArgs args)
        {
	        SegmentSelected?.Invoke(sender, args);
        }
        
        #endregion

        #region IPreviewUpdatedViaRefresh

        public void AfterFileRefresh()
        {
            Refresh();
            ((InternalPreviewControl)Control).JumpToActiveElement();

        }

        public void BeforeFileRefresh()
        {
            // no action required here
        }

        /// <summary>
        /// returns the file for preview
        /// </summary>
        public TempFileManager TargetFilePath
        {
            get => PreviewFile;
            set => PreviewFile = value;
        }

        #endregion

        #region Dispose of the Preview file

        /// <summary>
        /// deletes the preview file, if it exists.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// implementation of the recommended dispose protocol
        /// deletes the file if possible.
        /// </summary>
        /// <param name="disposing">true if this method is called from IDisposable.Dispose() and false if called from Finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _previewFile.Dispose();
            }
            // release the native unmanaged resources you added
            // in this derived class here.

            _disposed = true;
        }

        #endregion
    }
}
