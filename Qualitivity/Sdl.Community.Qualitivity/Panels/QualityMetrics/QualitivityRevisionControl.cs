using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Panels.Main;
using Sdl.Community.Qualitivity.Tracking;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using QualityMetric = Sdl.Community.Structures.Documents.Records.QualityMetric;

namespace Sdl.Community.Qualitivity.Panels.QualityMetrics
{
    public partial class QualitivityRevisionControl : UserControl
    {
        private QualitivityViewController _controller { get; set; }
        public QualitivityViewController Controller
        {
            get
            {
                return _controller;
            }
            set
            {

                _controller = value;
            }
        }
        private static EditorController GetEditorController()
        {
            return SdlTradosStudio.Application.GetController<EditorController>();
        }

        public QualitivityRevisionControl()
        {

            InitializeComponent();
            comboBox_qm_filter.SelectedIndex = 0;
            CheckEnabledInsertButton();
            comboBox_qm.DropDown += (o, e) => ((ComboBox)o).DroppedDown = false;


        }



        private void CheckEnabledInsertButton()
        {



            var editorController = GetEditorController();
            try
            {
                if (editorController != null && editorController.ActiveDocument != null && editorController.ActiveDocument.ActiveFile != null)
                {
                    button_insert.Enabled = comboBox_qm.SelectedItem.ToString().Trim() != string.Empty;
                    toolStripButton_add.Enabled = true;
                }
                else
                {
                    button_insert.Enabled = false;
                    toolStripButton_add.Enabled = false;
                }
            }
            catch
            {
                button_insert.Enabled = false;
                toolStripButton_add.Enabled = false;
            }
        }



        public void initialize_quick_insert()
        {

            try
            {
                #region  |  comboBox_qm  |
                try
                {
                    comboBox_qm.BeginUpdate();


                    comboBox_qm.Items.Clear();
                    comboBox_qm.Items.Add(string.Empty);

                    contextMenuStrip2 = new ContextMenuStrip();
                    contextMenuStrip2.ItemClicked -= contextMenuStrip2_ItemClicked;
                    contextMenuStrip2.ItemClicked += contextMenuStrip2_ItemClicked;
                    contextMenuStrip2.Items.Clear();


                    foreach (var qm in Tracked.Settings.QualityMetricGroup.Metrics)
                    {
                        if (qm.Name.Trim() != string.Empty)
                        {
                            if (qm.Name.IndexOf(">", StringComparison.Ordinal) > -1)
                            {
                                var name = qm.Name;
                                var nameBefore = name.Substring(0, name.IndexOf(">", StringComparison.Ordinal));
                                var nameAfter = name.Substring(name.IndexOf(">", StringComparison.Ordinal) + 1);


                                var tsiParent = GetParentMenuItem(nameBefore, contextMenuStrip2.Items);

                                name = nameAfter;
                                while (name.Trim() != string.Empty && name.IndexOf(">", StringComparison.Ordinal) > -1)
                                {
                                    nameBefore = name.Substring(0, name.IndexOf(">", StringComparison.Ordinal));
                                    nameAfter = name.Substring(name.IndexOf(">", StringComparison.Ordinal) + 1);

                                    tsiParent = GetParentMenuItem(nameBefore, tsiParent.DropDownItems);

                                    name = nameAfter;
                                }

                                var tsiItem = GetParentMenuItem(name, tsiParent.DropDownItems);
                                tsiItem.Tag = qm;
                                tsiItem.Click += tsi_item_Click;
                                tsiParent.DropDownItems.Add(tsiItem);
                            }
                            else
                            {
                                var tsiParent = GetParentMenuItem(qm.Name, contextMenuStrip2.Items);
                                tsiParent.Tag = qm;
                                contextMenuStrip2.Items.Add(tsiParent);
                            }
                        }
                    }

                    comboBox_qm.SelectedIndex = 0;

                }
                finally
                {
                    comboBox_qm.EndUpdate();
                }
                #endregion

                #region  |  comboBox_severity  |
                try
                {
                    comboBox_severity.BeginUpdate();

                    comboBox_severity.Items.Clear();
                    comboBox_severity.Items.Add(string.Empty);
                    foreach (var severity in Tracked.Settings.QualityMetricGroup.Severities)
                        comboBox_severity.Items.Add(severity.Name);
                    comboBox_severity.SelectedIndex = 0;

                    comboBox_severity.Enabled = false;
                }
                finally
                {
                    comboBox_severity.EndUpdate();
                }
                #endregion

                textBox_content.Text = string.Empty;
                textBox_comment.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                CheckEnabledInsertButton();
            }
        }

        private void tsi_item_Click(object sender, EventArgs e)
        {
            var tsdd = sender as ToolStripDropDownItem;
            if (tsdd != null && tsdd.Tag == null) return;
            if (tsdd == null) return;
            var qmM = tsdd.Tag as Sdl.Community.Structures.QualityMetrics.QualityMetric;
            AddQualityMetricItemToComboBox(qmM);
        }





        private ToolStripMenuItem GetParentMenuItem(string nameBefore, ToolStripItemCollection collection)
        {
            var tsiParent = collection.Cast<ToolStripMenuItem>().FirstOrDefault(tsiItem =>
                string.Compare(tsiItem.Text, nameBefore, StringComparison.OrdinalIgnoreCase) == 0);
            if (tsiParent != null) return tsiParent;
            tsiParent = new ToolStripMenuItem { Text = nameBefore };
            collection.Add(tsiParent);

            return tsiParent;
        }

        public void SetCurrentContentSelection(string content)
        {
            textBox_content.Text = content;
        }

        private void FindSegmentInEditor()
        {
            if (dataGridView_qm.SelectedRows.Count <= 0) return;
            var qm = dataGridView_qm.SelectedRows[0].Tag as QualityMetric;
            var success = qm != null && Tracked.ActiveDocument.SetActiveSegmentPair(qm.ParagraphId, qm.SegmentId);
        }

        public void AddNewQualityMetric(QualityMetric qm)
        {

            var f = new Dialogs.QualityMetrics.QualityMetric();
            qm.ParagraphId = QualitivityRevisionController.SelectedParagraphId;
            qm.SegmentId = QualitivityRevisionController.SelectedSegmentId;
            qm.Id = Guid.NewGuid().ToString();

            qm.Created = DateTime.Now;
            qm.Modified = DateTime.Now;
            qm.UserName = Tracked.Settings.UserProfile.UserName;
            f.Metric = qm;
            f.IsEdit = false;
            f.ShowDialog();
            if (!f.Saved) return;
            qm.Updated = true;
            //add the new qm to the list
            AddQualityMetricToDataViewList(qm);
        }
        public void EditQualityMetric(DataGridViewRow dr, QualityMetric qm)
        {

            if (qm.SegmentId != QualitivityRevisionController.SelectedSegmentId && qm.ParagraphId != QualitivityRevisionController.SelectedParagraphId)
                FindSegmentInEditor();


            var f = new Dialogs.QualityMetrics.QualityMetric
            {
                Metric = qm,
                IsEdit = true
            };

            f.ShowDialog();
            if (!f.Saved) return;
            qm.Updated = true;
            if (qm.RecordId != -1)
            {
                //if the QM entry was created originally in a different session, then
                //a independant entry is always created with a new ID.

                //Note: only when the QM is belonging to the same edit session can it be
                //updated without creating a new entry in the database.
                qm.Id = Guid.NewGuid().ToString();
            }
            qm.Modified = DateTime.Now;
            qm.UserName = Tracked.Settings.UserProfile.UserName;

            UpdateQualityMetricInDataViewList(dr, qm);
        }

        private void AddQualityMetricToDataViewList(QualityMetric qm)
        {

            QualitivityRevisionController.QualityMetrics.Add(qm);

            var n = dataGridView_qm.Rows.Add();

            var dataGridViewImageCell = dataGridView_qm.Rows[n].Cells["Column_status"] as DataGridViewImageCell;
            var img = imageList1.Images["empty"];
            switch (qm.Status)
            {
                case QualityMetric.ItemStatus.Open: img = imageList1.Images["Flag-Blue"]; break;
                case QualityMetric.ItemStatus.Resolved: img = imageList1.Images["Flag-Green"]; break;
                default: img = imageList1.Images["Flag-Violet"]; break;
            }
            if (dataGridViewImageCell != null)
            {
                dataGridViewImageCell.ImageLayout = DataGridViewImageCellLayout.Normal;
                dataGridViewImageCell.Value = img;
                dataGridViewImageCell.ValueType = typeof(Image);
                dataGridViewImageCell.ToolTipText = string.Empty;
                switch (qm.Status)
                {
                    case QualityMetric.ItemStatus.Open: dataGridViewImageCell.ToolTipText = "Open"; break;
                    case QualityMetric.ItemStatus.Resolved: dataGridViewImageCell.ToolTipText = "Resolved"; break;
                    default: dataGridViewImageCell.ToolTipText = "Ignore"; break;
                }
            }

            dataGridViewImageCell = dataGridView_qm.Rows[n].Cells["Column_recorded"] as DataGridViewImageCell;
            img = qm.RecordId > -1 ? imageList1.Images["Flag-Yellow"] : imageList1.Images["empty"];

            if (dataGridViewImageCell != null)
            {
                dataGridViewImageCell.ImageLayout = DataGridViewImageCellLayout.Normal;
                dataGridViewImageCell.Value = img;
                dataGridViewImageCell.ValueType = typeof(Image);
                dataGridViewImageCell.ToolTipText = "";
            }


            var segmentId = dataGridView_qm.Rows[n].Cells["Column_segment_id"] as DataGridViewTextBoxCell;
            if (segmentId != null) segmentId.Value = qm.SegmentId;

            var textBox = dataGridView_qm.Rows[n].Cells["Column_qm"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.Name;

            textBox = dataGridView_qm.Rows[n].Cells["Column_severity"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.SeverityName;

            textBox = dataGridView_qm.Rows[n].Cells["Column_content"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.Content;


            textBox = dataGridView_qm.Rows[n].Cells["Column_comment"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.Comment;



            dataGridView_qm.Rows[n].Tag = qm;

            dataGridView_qm.Sort(dataGridView_qm.Columns[2], ListSortDirection.Ascending);

            dataGridView_qm_SelectionChanged(null, null);
        }
        private void UpdateQualityMetricInDataViewList(DataGridViewRow dr, QualityMetric qm)
        {
            var dataGridViewImageCell = dr.Cells["Column_status"] as DataGridViewImageCell;
            var img = imageList1.Images["empty"];
            switch (qm.Status)
            {
                case QualityMetric.ItemStatus.Open: img = imageList1.Images["Flag-Blue"]; break;
                case QualityMetric.ItemStatus.Resolved: img = imageList1.Images["Flag-Green"]; break;
                default: img = imageList1.Images["Flag-Violet"]; break;
            }
            if (dataGridViewImageCell != null)
            {
                dataGridViewImageCell.ImageLayout = DataGridViewImageCellLayout.Normal;
                dataGridViewImageCell.Value = img;
                dataGridViewImageCell.ValueType = typeof(Image);
                dataGridViewImageCell.ToolTipText = string.Empty;
                switch (qm.Status)
                {
                    case QualityMetric.ItemStatus.Open: dataGridViewImageCell.ToolTipText = "Open"; break;
                    case QualityMetric.ItemStatus.Resolved: dataGridViewImageCell.ToolTipText = "Resolved"; break;
                    default: dataGridViewImageCell.ToolTipText = "Ignore"; break;
                }
            }

            dataGridViewImageCell = dr.Cells["Column_recorded"] as DataGridViewImageCell;
            img = qm.RecordId > -1 ? imageList1.Images["Flag-Yellow"] : imageList1.Images["empty"];

            if (dataGridViewImageCell != null)
            {
                dataGridViewImageCell.ImageLayout = DataGridViewImageCellLayout.Normal;
                dataGridViewImageCell.Value = img;
                dataGridViewImageCell.ValueType = typeof(Image);
                dataGridViewImageCell.ToolTipText = "";
            }


            var textBox = dr.Cells["Column_qm"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.Name;

            textBox = dr.Cells["Column_severity"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.SeverityName;

            textBox = dr.Cells["Column_content"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.Content;

            textBox = dr.Cells["Column_comment"] as DataGridViewTextBoxCell;
            if (textBox != null) textBox.Value = qm.Comment;


            dr.Tag = qm;

            dataGridView_qm_SelectionChanged(null, null);
        }

        private void toolStripButton_add_Click(object sender, EventArgs e)
        {
            var qm = new QualityMetric { Content = Tracked.ActiveDocument.Selection.Current.ToString() };


            AddNewQualityMetric(qm);
        }
        private void toolStripButton_edit_Click(object sender, EventArgs e)
        {
            if (dataGridView_qm.SelectedRows.Count > 0)
            {
                var dr = dataGridView_qm.SelectedRows[0];
                var qm = dr.Tag as QualityMetric;

                EditQualityMetric(dr, qm);
            }
        }

        private void toolStripButton_remove_Click(object sender, EventArgs e)
        {
            if (dataGridView_qm.SelectedRows.Count <= 0) return;
            for (var i = 0; i < dataGridView_qm.SelectedRows.Count; i++)
            {

                var qm = dataGridView_qm.SelectedRows[i].Tag as QualityMetric;
                if (qm != null && qm.RecordId != -1)
                    continue;
                var qmUpdated = QualitivityRevisionController.QualityMetrics.Find(a => a.Id == qm.Id);
                qmUpdated.Updated = true;
                qmUpdated.Removed = true;

                dataGridView_qm.Rows.Remove(dataGridView_qm.SelectedRows[i]);
            }
        }

        private void dataGridView_qm_DoubleClick(object sender, EventArgs e)
        {
            FindSegmentInEditor();
        }

        private void dataGridView_qm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                toolStripButton_remove_Click(sender, null);
        }

        public void dataGridView_qm_SelectionChanged(object sender, EventArgs e)
        {
            var rowIsSelected = false;
            try
            {

                if (dataGridView_qm.SelectedRows.Count > 0)
                {
                    var dr = dataGridView_qm.SelectedRows[0];
                    if (dr != null && dr.Tag != null)
                    {
                        var qm = dr.Tag as QualityMetric;
                        if (qm != null && qm.Modified != null)
                            label_status_text.Text = @"Modified: " + qm.Modified.Value.ToShortDateString()
                                                     + @" " + qm.Modified.Value.ToShortTimeString();


                        if (qm != null && qm.RecordId == -1)
                            rowIsSelected = true;
                    }
                }
                else
                {
                    label_status_text.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (rowIsSelected)
            {
                toolStripButton_edit.Enabled = true;
                toolStripButton_remove.Enabled = true;

                editQualityMetricToolStripMenuItem.Enabled = true;
                removeQualityMetricToolStripMenuItem.Enabled = true;
            }
            else
            {
                toolStripButton_edit.Enabled = true;
                toolStripButton_remove.Enabled = false;

                editQualityMetricToolStripMenuItem.Enabled = true;
                removeQualityMetricToolStripMenuItem.Enabled = false;
            }

        }

        private void dataGridView_qm_DragDrop(object sender, DragEventArgs e)
        {

            var qm = new QualityMetric { Content = Tracked.ActiveDocument.Selection.Current.ToString() };
            QualitivityRevisionController.AddNewQualityMetric(qm);


        }

        private void addNewQualityMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_add_Click(sender, e);
        }

        private void editQualityMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_edit_Click(sender, e);
        }

        private void removeQualityMetricToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripButton_remove_Click(sender, e);
        }


        private void contextMenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Tag == null) return;
            var qmM = e.ClickedItem.Tag as Sdl.Community.Structures.QualityMetrics.QualityMetric;
            AddQualityMetricItemToComboBox(qmM);
        }

        private void AddQualityMetricItemToComboBox(Sdl.Community.Structures.QualityMetrics.QualityMetric qmM)
        {
            comboBox_qm.Items[0] = qmM.Name;
            comboBox_qm.SelectedIndex = 0;


            if (comboBox_severity.Items[0].ToString().Trim() == string.Empty)
            {
                comboBox_severity.Items.RemoveAt(0);
                comboBox_severity.SelectedIndex = 0;
            }
            comboBox_severity.Enabled = true;


            #region  |  check for default and set it  |
            foreach (var qm in Tracked.Settings.QualityMetricGroup.Metrics)
            {
                if (string.Compare(qm.Name, comboBox_qm.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) !=
                    0) continue;
                for (var i = 0; i < comboBox_severity.Items.Count; i++)
                {
                    if (
                        string.Compare(qm.MetricSeverity.Name, comboBox_severity.Items[i].ToString(),
                            StringComparison.OrdinalIgnoreCase) != 0) continue;
                    comboBox_severity.SelectedIndex = i;
                    break;
                }
                break;
            }
            #endregion
        }

        private void comboBox_qm_SelectedIndexChanged(object sender, EventArgs e)
        {

            CheckEnabledInsertButton();
        }

        private void button_insert_Click(object sender, EventArgs e)
        {
            if (comboBox_qm.SelectedItem.ToString().Trim() == string.Empty) return;
            var qm = new QualityMetric
            {
                ParagraphId = QualitivityRevisionController.SelectedParagraphId,
                SegmentId = QualitivityRevisionController.SelectedSegmentId,
                Id = Guid.NewGuid().ToString(),
                Name = comboBox_qm.SelectedItem.ToString(),
                SeverityName = comboBox_severity.SelectedItem.ToString(),
                Created = DateTime.Now,
                Modified = DateTime.Now,
                UserName = Tracked.Settings.UserProfile.UserName,
                Updated = true
            };

            #region  |  severityWeight  |
            foreach (var severity in Tracked.Settings.QualityMetricGroup.Severities)
            {
                if (string.Compare(qm.SeverityName, severity.Name, StringComparison.OrdinalIgnoreCase) != 0) continue;
                qm.SeverityValue = severity.Value;
                break;
            }
            #endregion
            qm.Content = textBox_content.Text;
            qm.Comment = textBox_comment.Text;

            //add the qm to the container
            AddQualityMetricToDataViewList(qm);


            #region  |  clean up quick insert  |

            comboBox_qm.Items.Clear();
            comboBox_qm.Items.Add(string.Empty);
            comboBox_qm.SelectedIndex = 0;

            #region  |  comboBox_severity  |
            try
            {
                comboBox_severity.BeginUpdate();

                comboBox_severity.Items.Clear();
                comboBox_severity.Items.Add(string.Empty);
                foreach (var severity in Tracked.Settings.QualityMetricGroup.Severities)
                    comboBox_severity.Items.Add(severity.Name);
                comboBox_severity.SelectedIndex = 0;

                comboBox_severity.Enabled = false;
            }         
            finally
            {
                comboBox_severity.EndUpdate();
            }
            #endregion

            textBox_content.Text = string.Empty;
            textBox_comment.Text = string.Empty;

            #endregion
        }


        private void UpdateStatusCounters()
        {
            tabControl_qm.TabPages[0].Text = @"Container { " + dataGridView_qm.Rows.Count + " } ";           
        }
        private void dataGridView_qm_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateStatusCounters();
        }

        private void dataGridView_qm_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateStatusCounters();
        }


        private void activateTheSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {

            FindSegmentInEditor();

        }


        private void dataGridView_qm_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index != 2) return;
            e.SortResult = int.Parse(GetIntRep(e.CellValue1)).CompareTo(int.Parse(GetIntRep(e.CellValue2)));
            e.Handled = true;//pass by the default sorting
        }
        private string GetIntRep(object strValue)
        {
            var iValue = strValue.ToString();

            //replace the letter with a number
            var ri = new Regex(@"[a-zA-Z]", RegexOptions.IgnoreCase);
            var mc = ri.Matches(iValue);
            if (mc.Count > 0)
            {
                iValue = mc.Cast<Match>().Aggregate(iValue, (current, m) => current.Replace(m.Value, ReplaceAlphaWithNumber(m.Value.ToLower())));
            }
            iValue = iValue.Replace(" ", "").PadRight(9, '0');
            if (iValue.Length > 9)
                iValue = iValue.Substring(0, 9);

            return iValue;
        }


        private readonly Dictionary<string, string> _alphabetToNumber = new Dictionary<string, string>
        {
             {"a","01"}
            ,{"b","02"}
            ,{"c","03"}
            ,{"d","04"}
            ,{"e","05"}
            ,{"f","06"}
            ,{"g","07"}
            ,{"h","08"}
            ,{"i","09"}
            ,{"j","10"}
            ,{"k","11"}
            ,{"l","12"}
            ,{"m","13"}
            ,{"n","14"}
            ,{"o","15"}
            ,{"p","16"}
            ,{"q","17"}
            ,{"r","18"}
            ,{"s","18"}
            ,{"t","20"}
            ,{"u","21"}
            ,{"v","22"}
            ,{"w","23"}
            ,{"x","24"}
            ,{"y","25"}
            ,{"z","26"}
        };
        private string ReplaceAlphaWithNumber(string alphaChar)
        {
            var iValue = "0";
            if (_alphabetToNumber.ContainsKey(alphaChar))
                iValue = _alphabetToNumber[alphaChar];
            return iValue;
        }


        private void comboBox_qm_Click(object sender, EventArgs e)
        {
            contextMenuStrip2.Show();
            contextMenuStrip2.BringToFront();

            contextMenuStrip2.AutoSize = false;

            contextMenuStrip2.Focus();
            if (comboBox_qm.AccessibilityObject != null)
                contextMenuStrip2.Bounds = new Rectangle(new Point(comboBox_qm.AccessibilityObject.Bounds.X, comboBox_qm.AccessibilityObject.Bounds.Y)
                    , new Size(comboBox_qm.Width, contextMenuStrip2.Height));
        }







    }
}
