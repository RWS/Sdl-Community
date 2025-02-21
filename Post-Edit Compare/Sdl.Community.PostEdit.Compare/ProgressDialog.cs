using System;
using System.Windows.Forms;

namespace PostEdit.Compare
{
    public partial class ProgressDialog : Form
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        public bool HitCancel { get; set; }
        private void ProgressDialogLoadProject_Load(object sender, EventArgs e)
        {
            
            DialogResult = DialogResult.None;
            HitCancel = false;
        }

        
        private ProgressObject _progressObject = new ProgressObject();

      

      
        public object ProgressObject
        {
            //get { return progressObject; }
            set
            {
                try
                {
                    _progressObject = (ProgressObject)value;

                    if (_progressObject.ProgessTitle.Trim() != string.Empty)
                        this.textBox_header_title.Text = _progressObject.ProgessTitle;

                    if (_progressObject.CurrentProcessingMessage.Trim() != string.Empty)
                        this.label_processing_message_1.Text = _progressObject.CurrentProcessingMessage;

                    if (_progressObject.CurrentProgressValue < this.progressBar_progress.Maximum)
                    {
                        this.progressBar_progress.Invoke((MethodInvoker)delegate
                        {

                            this.progressBar_progress.Value = _progressObject.CurrentProgressValue;
                            if (_progressObject.CurrentProgressValue != 0)
                                this.progressBar_progress.Value = _progressObject.CurrentProgressValue - 1;
                            this.progressBar_progress.Value = _progressObject.CurrentProgressValue;

                        });
                    }

                    this.label_processing_message_2.Text = _progressObject.CurrentProgressValueMessage;
                    this.label_progress_percentage.Text = this.progressBar_progress.Value.ToString() + "%";


                    this.progressBar_progress.Invoke((MethodInvoker)delegate
                    {
                        this.progressBar_total_progress.Value = _progressObject.TotalProgressValue;
                        if (_progressObject.TotalProgressValue != 0)
                            this.progressBar_total_progress.Value = _progressObject.TotalProgressValue - 1;
                        this.progressBar_total_progress.Value = _progressObject.TotalProgressValue;
                    });

                    this.label_processing_message_3.Text = _progressObject.TotalProgressValueMessage;
                    this.label_total_progress_percentage.Text = this.progressBar_total_progress.Value.ToString() + "%";


                    Application.DoEvents();
                }
                catch { }
            }
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {           
            
            HitCancel = true;
            this.Close();
        }

    
    }
}
