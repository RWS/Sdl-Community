using System;
using System.Windows.Forms;

namespace PostEdit.Compare
{
    public partial class WaitingDialog : Form
    {
        public WaitingDialog()
        {
            InitializeComponent();
        }
        public bool hitCancel { get; set; }
        private void ProgressDialogLoadProject_Load(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.None;
            hitCancel = false;
        }

        
        private ProgressObject progressObject = new ProgressObject();

        //public Int32 ProgressValue
        //{
        //    get;
        //    set;
        //}


        //public string current_processing_message
        //{
        //    set
        //    {
        //        this.label_processing_message_1.Text = value;
        //    }
        //}
        public Object ProgressObject
        {
            //get { return progressObject; }
            set
            {      
                
                progressObject = (ProgressObject)value;

       
                this.textBox_header_title.Text = progressObject.ProgessTitle;

                this.label_processing_message_1.Text = progressObject.CurrentProcessingMessage;
                this.label_processing_message_2.Text = progressObject.TotalProgressValueMessage;

                Application.DoEvents();
            }
        }


        private void button_cancel_Click(object sender, EventArgs e)
        {
            hitCancel = true;
            this.Close();
        }

    
    }
}
