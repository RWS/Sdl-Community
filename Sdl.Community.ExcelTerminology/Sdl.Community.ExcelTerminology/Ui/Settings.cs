using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.ExcelTerminology.Ui
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            sourceBox.Text = "A";
            targetBox.Text = "B";
            approvedBox.Text = "C";
        }

        private void defaultSettingsLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            var file = new OpenFileDialog();
            if (file.ShowDialog() == DialogResult.OK)
            {
                filePath = file.FileName;
            }

            pathTextBox.Text = filePath;
        }
    }
}
