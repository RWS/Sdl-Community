using System;
using System.Windows.Forms;

namespace Sdl.Community.Taus.Translation.Provider
{
    public partial class FormPasswordCheck : Form
    {
        public bool Saved { get; set; }
        public FormPasswordCheck()
        {
            InitializeComponent();
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            Saved = false;
            Close();
        }

        private void FormPasswordCheck_Load(object sender, EventArgs e)
        {
            Saved = false;
            textBox_password.Select();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            Saved = true;
            Close();
        }

        private void textBox_password_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button_ok_Click(sender, new EventArgs());
            }
        }
    }
}
