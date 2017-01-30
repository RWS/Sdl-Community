using System;
using System.Windows.Forms;

namespace Sdl.Community.Studio.Time.Tracker.Dialogs
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

        private void textBox_addressStreet_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_nameMiddle_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_nameLast_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox_addressCity_TextChanged(object sender, EventArgs e)
        {

        }

        private void AddressDetails_Load(object sender, EventArgs e)
        {
            Saved = false;
        }
    }
}
