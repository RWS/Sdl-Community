using System;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.QualityMetrics
{
    public partial class QualityMetricItem : Form
    {
        public Sdl.Community.Structures.QualityMetrics.QualityMetricGroup MetricGroup { get; set; }
        public Sdl.Community.Structures.QualityMetrics.QualityMetric Metric { get; set; }

        public bool IsEdit { get; set; }

        internal bool Saved { get; set; }
        public QualityMetricItem()
        {
            InitializeComponent();
        }

        private void AddressDetails_Load(object sender, EventArgs e)
        {
            Saved = false;

            textBox_name.Text = Metric.Name;
            textBox_description.Text = Metric.Description;
            comboBox_severity.Items.Clear();
            var iSelectedIndex = 0;
            var i = 0;

            foreach (var severity in MetricGroup.Severities)
            {
                if (string.Compare(Metric.MetricSeverity.Name, severity.Name, StringComparison.OrdinalIgnoreCase) == 0)
                    iSelectedIndex = i;

                comboBox_severity.Items.Add(severity.Name + " {" + severity.Value + "}");
                i++;
            }


            comboBox_severity.SelectedIndex = iSelectedIndex;

        }


        private void button_save_Click(object sender, EventArgs e)
        {
            if (textBox_name.Text.Trim() == string.Empty) return;
            var continueSave = true;
            if (!IsEdit)
            {
                //quick check -> if the name already exists in the lsit
                foreach (var qc in MetricGroup.Metrics)
                {
                    if (string.Compare(qc.Name, textBox_name.Text.Trim(), StringComparison.OrdinalIgnoreCase) != 0) continue;
                    continueSave = false;
                    MessageBox.Show(PluginResources.Item_name_already_exists_in_the_list_);
                }
            }

            if (continueSave)
            {
                Metric.Name = textBox_name.Text.Trim();
                Metric.Description = textBox_description.Text.Trim();
                foreach (var severity in MetricGroup.Severities)
                {
                    if (severity.Name + " {" + severity.Value + "}" == comboBox_severity.SelectedItem.ToString())
                        Metric.MetricSeverity = severity;

                }

                Saved = true;
                Close();
            }
        }

     
        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


      
       
        
    }
}
