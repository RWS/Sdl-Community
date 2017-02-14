using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.Structures.QualityMetrics;

namespace Sdl.Community.Qualitivity.Dialogs.QualityMetrics
{
    public partial class QualityMetricGroup : Form
    {


        public Sdl.Community.Structures.QualityMetrics.QualityMetricGroup MetricGroup { get; set; }
        public bool IsEdit { get; set; }
        public bool Saved { get; set; }

        public QualityMetricGroup()
        {
            InitializeComponent();
            IsEdit = false;
            Saved = false;
        }



        private void QualityMetricGroup_Load(object sender, EventArgs e)
        {
            textBox_name.Select();
            textBox_name.Text = MetricGroup.Name;
            textBox_description.Text = MetricGroup.Description;
            numericUpDown_company_profile_maximum_value.Value = MetricGroup.MaxSeverityValue;
            numericUpDown_company_profile_maximum_value_in_words.Value = MetricGroup.MaxSeverityInValue;


            try
            {
                listView_severities.BeginUpdate();

                listView_severities.Items.Clear();
                if (MetricGroup.Severities != null)
                {
                    foreach (var severity in MetricGroup.Severities)
                    {
                        var item = listView_severities.Items.Add(severity.Name);
                        item.SubItems.Add(severity.Value.ToString());
                        item.Tag = severity;
                    }
                }

            }
            finally
            {
                listView_severities.EndUpdate();
            }

            label_severity_item_count.Text = PluginResources.Items_ + listView_severities.Items.Count + @" ";

        }

        private void button_Save_Click(object sender, EventArgs e)
        {
            MetricGroup.Name = textBox_name.Text;
            MetricGroup.Description = textBox_description.Text;
            MetricGroup.MaxSeverityValue = Convert.ToInt32(numericUpDown_company_profile_maximum_value.Value);
            MetricGroup.MaxSeverityInValue = Convert.ToInt32(numericUpDown_company_profile_maximum_value_in_words.Value);

            MetricGroup.Severities.Clear();

            foreach (ListViewItem item in listView_severities.Items)
            {
                MetricGroup.Severities.Add((Severity)item.Tag);
            }


            foreach (var qc in MetricGroup.Metrics)
            {
                foreach (var severity in MetricGroup.Severities)
                {
                    if ((severity.Id <= -1 || severity.Id != qc.MetricSeverity.Id) &&
                        string.Compare(qc.Name, severity.Name, StringComparison.OrdinalIgnoreCase) != 0) continue;
                    qc.MetricSeverity = severity;
                    break;
                }
            }

            Saved = true;
            Close();
        }

        private void button_add_severity_Click(object sender, EventArgs e)
        {
            var f = new QualityMetricWeightsAppend();
            f.textBox_name.Text = string.Empty;
            f.numericUpDown_weight.Value = 0;
            f.Text = PluginResources.Severity_Weight___Add;
            f.ShowDialog();
            if (f.Saved)
            {
                if (f.textBox_name.Text.Trim() != string.Empty)
                {
                    var found = listView_severities.Items.Cast<ListViewItem>().Any(item => string.Compare(item.Text, f.textBox_name.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
                    if (found)
                    {
                        MessageBox.Show(PluginResources.Item_name_already_exists_in_the_list_);
                    }
                    else
                    {
                        var severity = new Severity(f.textBox_name.Text.Trim(), Convert.ToInt32(f.numericUpDown_weight.Value), -1);
                        var item = listView_severities.Items.Add(severity.Name);
                        item.SubItems.Add(severity.Value.ToString());
                        item.Tag = severity;
                        
                    }
                }
            }
            label_severity_item_count.Text = PluginResources.Items_ + listView_severities.Items.Count + @" ";
        }

        private void button_edit_severity_Click(object sender, EventArgs e)
        {
            if (listView_severities.SelectedItems.Count > 0)
            {
                var itemSelected = listView_severities.SelectedItems[0];
                var severity = (Severity)itemSelected.Tag;

                var f = new QualityMetricWeightsAppend
                {
                    textBox_name = {Text = severity.Name},
                    numericUpDown_weight = {Value = Convert.ToInt32(severity.Value)},
                    Text = PluginResources.Severity_Weight___Edit
                };
                f.ShowDialog();
                if (f.Saved)
                {
                    if (f.textBox_name.Text.Trim() != string.Empty)
                    {
                        var found = listView_severities.Items.Cast<ListViewItem>().Where(item => item.Index != itemSelected.Index).Any(item => string.Compare(item.Text, f.textBox_name.Text.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
                        if (found)
                        {
                            MessageBox.Show(PluginResources.Item_name_already_exists_in_the_list_);
                        }
                        else
                        {
                            severity.Name = f.textBox_name.Text;
                            severity.Value = Convert.ToInt32(f.numericUpDown_weight.Value);
                            itemSelected.Text = severity.Name;
                            itemSelected.SubItems[1].Text = severity.Value.ToString();
                            itemSelected.Tag = severity;
                        }
                    }
                }
            }

            label_severity_item_count.Text = PluginResources.Items_ + listView_severities.Items.Count + @" ";
        }

        private void button_delete_severity_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView_severities.SelectedItems)
                item.Remove();

            label_severity_item_count.Text = PluginResources.Items_ + listView_severities.Items.Count + @" ";
        }

        private void listView_severities_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView_severities.SelectedIndices.Count > 0)
            {
                button_edit_severity.Enabled = true;
                button_delete_severity.Enabled = true;
            }
            else
            {
                button_edit_severity.Enabled = false;
                button_delete_severity.Enabled = false;
            }
            
        }

        private void listView_severities_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                button_delete_severity_Click(null, null);
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void listView_severities_DoubleClick(object sender, EventArgs e)
        {
            button_edit_severity_Click(null, null);
        }

       
    }
}
