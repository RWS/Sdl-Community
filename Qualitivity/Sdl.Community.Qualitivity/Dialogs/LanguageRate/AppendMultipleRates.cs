using System;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.LanguageRate
{
    public partial class AppendMultipleRates : Form
    {

        public bool Saved { get; set; }

        public AppendMultipleRates()
        {
            InitializeComponent();
            Saved = false;
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
    }
}
