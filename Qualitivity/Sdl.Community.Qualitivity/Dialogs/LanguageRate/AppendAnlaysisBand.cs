using System;
using System.Windows.Forms;

namespace Sdl.Community.Qualitivity.Dialogs.LanguageRate
{
    public partial class AppendAnlaysisBand : Form
    {
        public AppendAnlaysisBand()
        {
            InitializeComponent();
        }

        public bool Saved { get; set; }

        private void FormAppendAnlaysisBand_Load(object sender, EventArgs e)
        {
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
