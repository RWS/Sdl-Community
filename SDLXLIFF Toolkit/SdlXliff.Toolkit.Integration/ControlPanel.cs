using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SdlXliff.Toolkit.Integration.Controls;
using SdlXliff.Toolkit.Integration.Data;

namespace SdlXliff.Toolkit.Integration
{
    public partial class ControlPanel : UserControl, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        DetailCellData data;

        private string _fileName;
        private int _sID;
        private int _segmentID;
        private string _segmentStatus;
        private string _sourceText;
        private string _targetText;

        private ISegment _sourceContent;
        private ISegment _targetContent;

        private List<IndexData> _sourceMatches;
        private List<IndexData> _targetMatches;
        private List<TagData> _sourceTags;
        private List<TagData> _targetTags;
        private List<WarningCellData> _warnings;

        private bool valueChanged = false;
        int rowIndex;

        public ControlPanel()
        {
            InitializeComponent();
        }

        public object Value
        {
            set 
            {
                if (value is DetailCellData)
                {
                    try
                    {
                        this._fileName = ((DetailCellData)value).FileName;
                        this._sID = ((DetailCellData)value).SID;
                        this._segmentID = ((DetailCellData)value).SegID;
                        this._segmentStatus = ((DetailCellData)value).SegStatus;
                        this._sourceText = ((DetailCellData)value).Source.Text;
                        this._targetText = ((DetailCellData)value).Target.Text;
                        this._sourceContent = ((DetailCellData)value).Source.Content;
                        this._targetContent = ((DetailCellData)value).Target.Content;
                        this._sourceMatches = ((DetailCellData)value).Source.MatchIndexes;
                        this._targetMatches = ((DetailCellData)value).Target.MatchIndexes;
                        this._sourceTags = ((DetailCellData)value).Source.Tags;
                        this._targetTags = ((DetailCellData)value).Target.Tags;
                        this._warnings = ((DetailCellData)value).Warnings;
                    }
                    catch
                    {
                        this._fileName = "Err -1";
                        this._sID = -1;
                        this._segmentID = -1;
                        this._segmentStatus = "Err -1";
                        this._sourceText = "Err -1";
                        this._targetText = "Err -1";
                        this._sourceContent = null;
                        this._targetContent = null;
                        this._sourceMatches = null;
                        this._targetMatches = null;
                        this._sourceTags = null;
                        this._targetTags = null;
                        this._warnings = null;
                    }
                }
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                data = new DetailCellData(_fileName, 
                    _sID,
                    _segmentID,
                    _segmentStatus,
                    _sourceText,
                    _sourceContent,
                    _targetText,
                    _targetContent);

                data.Source.MatchIndexes = _sourceMatches;
                data.Target.MatchIndexes = _targetMatches;
                data.Source.Tags = _sourceTags;
                data.Target.Tags = _targetTags;
                data.Warnings = _warnings;
                return data;
            }
            set
            {
                if (value is DetailCellData)
                {
                    try
                    {
                        // This will throw an exception of the string is 
                        // null, empty, or not in the format of a date.
                        this._fileName = ((DetailCellData)value).FileName;
                        this._sID = ((DetailCellData)value).SID;
                        this._segmentID = ((DetailCellData)value).SegID;
                        this._segmentStatus = ((DetailCellData)value).SegStatus;
                        this._sourceText = ((DetailCellData)value).Source.Text;
                        this._targetText = ((DetailCellData)value).Target.Text;
                        this._sourceContent = ((DetailCellData)value).Source.Content;
                        this._targetContent = ((DetailCellData)value).Target.Content;
                        this._sourceMatches = ((DetailCellData)value).Source.MatchIndexes;
                        this._targetMatches = ((DetailCellData)value).Target.MatchIndexes;
                        this._sourceTags = ((DetailCellData)value).Source.Tags;
                        this._targetTags = ((DetailCellData)value).Target.Tags;
                        this._warnings = ((DetailCellData)value).Warnings;
                    }
                    catch
                    {
                        // In the case of an exception, just use the 
                        // default value so we're not left with a null
                        // value.
                        this._fileName = "Err -2";
                        this._sID = -2;
                        this._segmentID = -2;
                        this._segmentStatus = "Err -2";
                        this._sourceText = "Err -2";
                        this._targetText = "Err -2";
                        this._sourceContent = null;
                        this._targetContent = null;
                        this._sourceMatches = null;
                        this._targetMatches = null;
                        this._sourceTags = null;
                        this._targetTags = null;
                        this._warnings = null;
                    }
                }
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            // TODO - REMOVE
            //this.lblFileName.Font = dataGridViewCellStyle.Font;
            //this.lblFileName.ForeColor = dataGridViewCellStyle.ForeColor;
            //this.lblFileName.BackColor = dataGridViewCellStyle.BackColor;

            //this.rtbText.Font = dataGridViewCellStyle.Font;
            //this.rtbText.ForeColor = dataGridViewCellStyle.ForeColor;
            //this.rtbText.BackColor = dataGridViewCellStyle.BackColor;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnTextChanged(e);
        }
    }
}
