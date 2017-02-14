using System;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Progress
{
    public partial class ProgressDialog : Form
    {
        public ProgressDialog()
        {
            InitializeComponent();
            progressObject = new ProgressObject();
        }
        public int DocumentsMaximum { get; set; }
        public int DocumentCurrentIndex { get; set; }
        public string DocumentProgressLabelStringFormat { get; set; }
        public string DialogProcessingMessage { get; set; }

        private ProgressObject progressObject { get; set; }

        public object ProgressObject
        {            
            set
            {
                progressObject = value as ProgressObject;

                if (progressObject == null) return;
                current_processing_message.Text = progressObject.CurrentProcessingMessage;


                current_progress_value_message.Text = progressObject.CurrentProgressValueMessage;
                current_progress_title.Text = progressObject.CurrentProgressTitle;
                current_progress_percentage.Text = progressObject.CurrentProgressPercentage;
                progressBar_current.Maximum = progressObject.CurrentProgressMaximum;
                progressBar_current.Invoke((MethodInvoker)delegate
                {
                    progressBar_current.Value = progressObject.CurrentProgressValue;
                    if (progressObject.CurrentProgressValue != 0)
                        progressBar_current.Value = progressObject.CurrentProgressValue - 1;
                    progressBar_current.Value = progressObject.CurrentProgressValue;
                });


                total_progress_value_message.Text = progressObject.TotalProgressValueMessage;
                total_progress_title.Text = progressObject.TotalProgressTitle;
                total_progress_percentage.Text = progressObject.TotalProgressPercentage;
                progressBar_total.Maximum = progressObject.TotalProgressMaximum;
                progressBar_total.Invoke((MethodInvoker)delegate
                {
                    progressBar_total.Value = progressObject.TotalProgressValue;
                    if (progressObject.TotalProgressValue != 0)
                        progressBar_total.Value = progressObject.TotalProgressValue - 1;
                    progressBar_total.Value = progressObject.TotalProgressValue;
                });
            }
        }

        private void ProgressDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
