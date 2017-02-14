using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Qualitivity.Tracking;

namespace Sdl.Community.Qualitivity.Dialogs.QualityMetrics
{
    public partial class QualityMetric : Form
    {

        public Sdl.Community.Structures.Documents.Records.QualityMetric Metric { get; set; }


        public bool IsEdit { get; set; }
        private bool IsLoading { get; set; }
        internal bool Saved { get; set; }
        public QualityMetric()
        {
            InitializeComponent();
        }


        private void CheckEnabled()
        {
            //maybe also include content == empty??
            button_save.Enabled = comboBox_qm.SelectedItem.ToString().Trim() != string.Empty;

        }

        private void QualityMetric_Load(object sender, EventArgs e)
        {
            IsLoading = true;
            Saved = false;

            try
            {
                #region  |  comboBox_qm  |

                try
                {
                    comboBox_qm.BeginUpdate();

                    comboBox_qm.Items.Clear();
                    comboBox_qm.Items.Add(string.Empty);


                    Sdl.Community.Structures.QualityMetrics.QualityMetric qmItemInList = null;
                    foreach (var qmIn in Tracked.Settings.QualityMetricGroup.Metrics)
                    {
                        if (qmIn.Name.Trim() == string.Empty) continue;
                        if (qmIn.Name.IndexOf(">", StringComparison.Ordinal) > -1)
                        {
                            var name = qmIn.Name;
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
                            tsiItem.Tag = qmIn;
                            tsiItem.Click += tsi_item_Click;
                            tsiParent.DropDownItems.Add(tsiItem);

                            if (string.Compare(qmIn.Name, Metric.Name, StringComparison.OrdinalIgnoreCase) == 0)
                                qmItemInList = qmIn;
                        }
                        else
                        {
                            var tsiParent = GetParentMenuItem(qmIn.Name, contextMenuStrip2.Items);
                            tsiParent.Tag = qmIn;
                            contextMenuStrip2.Items.Add(tsiParent);

                            if (string.Compare(qmIn.Name, Metric.Name, StringComparison.OrdinalIgnoreCase) == 0)
                                qmItemInList = qmIn;
                        }
                    }

                    if (qmItemInList != null)
                        comboBox_qm.Items[0] = qmItemInList.Name;
                    else
                        comboBox_qm.Items[0] = Metric.Name;

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
                    var iIndex = 0;
                    var iSelectedIndex = 0;
                    foreach (var severity in Tracked.Settings.QualityMetricGroup.Severities)
                    {
                        if (string.Compare(Metric.SeverityName, severity.Name, StringComparison.OrdinalIgnoreCase) == 0)
                            iSelectedIndex = iIndex;
                        comboBox_severity.Items.Add(severity.Name + " {" + severity.Value + "}");
                        iIndex++;
                    }


                    if (comboBox_qm.SelectedItem.ToString().Trim() == string.Empty)
                    {
                        comboBox_severity.Items.Insert(0, string.Empty);
                        comboBox_severity.SelectedIndex = 0;
                        comboBox_severity.Enabled = false;
                    }
                    else
                    {
                        comboBox_severity.SelectedIndex = iSelectedIndex;
                        comboBox_severity.Enabled = true;
                    }
                }
                finally
                {
                    comboBox_severity.EndUpdate();
                }
                #endregion

                #region  |  comboBox_status  |

                switch (Metric.Status)
                {
                    case Sdl.Community.Structures.Documents.Records.QualityMetric.ItemStatus.Open: comboBox_status.SelectedIndex = 0; break;
                    case Sdl.Community.Structures.Documents.Records.QualityMetric.ItemStatus.Resolved: comboBox_status.SelectedIndex = 1; break;
                    case Sdl.Community.Structures.Documents.Records.QualityMetric.ItemStatus.Ignore: comboBox_status.SelectedIndex = 2; break;

                }
                #endregion

                textBox_content.Text = Metric.Content;
                textBox_comment.Text = Metric.Comment;

                if (Metric.Modified.HasValue)
                    label_status_text.Text = PluginResources.Modified_ + Metric.Modified.Value + "\r\n" + PluginResources.By_ + Metric.UserName;

                label_status_text.Visible = IsEdit;

                comboBox_qm.Focus();
                comboBox_qm.Select();
            }
            catch (Exception ex)
            {
                //if there is an error while loading then close the dialog
                MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
            finally
            {
                IsLoading = false;
                CheckEnabled();
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
        private static ToolStripMenuItem GetParentMenuItem(string nameBefore, ToolStripItemCollection collection)
        {
            var tsiParent = collection.Cast<ToolStripMenuItem>().FirstOrDefault(tsiItem =>
                string.Compare(tsiItem.Text, nameBefore, StringComparison.OrdinalIgnoreCase) == 0);
            if (tsiParent != null) return tsiParent;
            tsiParent = new ToolStripMenuItem { Text = nameBefore };
            collection.Add(tsiParent);

            return tsiParent;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Metric.Name = comboBox_qm.SelectedItem.ToString();
            Metric.SeverityName = comboBox_severity.SelectedItem.ToString().Substring(0,
                comboBox_severity.SelectedItem.ToString().LastIndexOf(" {", StringComparison.Ordinal)).Trim();

            foreach (var severity in Tracked.Settings.QualityMetricGroup.Severities)
                if (string.Compare(Metric.SeverityName, severity.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    Metric.SeverityValue = severity.Value;


            if (comboBox_status.SelectedIndex == 0)
            {
                Metric.Status = Sdl.Community.Structures.Documents.Records.QualityMetric.ItemStatus.Open;
            }
            else if (comboBox_status.SelectedIndex == 1)
            {
                Metric.Status = Sdl.Community.Structures.Documents.Records.QualityMetric.ItemStatus.Resolved;
            }
            else if (comboBox_status.SelectedIndex == 2)
            {
                Metric.Status = Sdl.Community.Structures.Documents.Records.QualityMetric.ItemStatus.Ignore;
            }

            Metric.Content = textBox_content.Text;
            Metric.Comment = textBox_comment.Text;
            Metric.Modified = DateTime.Now;
            if (!IsEdit)
                Metric.Created = DateTime.Now;

            Metric.Updated = true;


            Saved = true;
            Close();
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void comboBox_qm_SelectedIndexChanged(object sender, EventArgs e)
        {          
            CheckEnabled();
        }

        private void comboBox_severity_SelectedIndexChanged(object sender, EventArgs e)
        {
            CheckEnabled();
        }

        private void textBox_content_TextChanged(object sender, EventArgs e)
        {
            CheckEnabled();
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
                if (string.Compare(qm.Name, comboBox_qm.SelectedItem.ToString(), StringComparison.OrdinalIgnoreCase) != 0) continue;
                for (var i = 0; i < comboBox_severity.Items.Count; i++)
                {
                    if (string.Compare(qm.MetricSeverity.Name, comboBox_severity.Items[i].ToString(), StringComparison.OrdinalIgnoreCase) != 0)
                        continue;
                    comboBox_severity.SelectedIndex = i;
                    break;
                }
                break;
            }
            #endregion
        }
    }
}
