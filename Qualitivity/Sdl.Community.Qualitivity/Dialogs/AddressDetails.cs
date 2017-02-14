using System;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs
{
    public partial class AddressDetails : Form
    {
        internal bool Saved { get; set; }
        public AddressDetails()
        {
            InitializeComponent();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }

     
        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }


        private void AddressDetails_Load(object sender, EventArgs e)
        {
            Saved = false;
        }
    }
}
