using System;
using System.Windows.Forms;

namespace PostEdit.Compare.Forms
{
    public partial class AppendMultiplePrices : Form
    {
    
        public bool Saved { get; set; }

        public AppendMultiplePrices()
        {
            InitializeComponent();
            Saved = false;
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Saved = true;
            this.Close();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }
    }
}
