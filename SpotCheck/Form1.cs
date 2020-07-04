using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sdl.Community.SpotCheck
{
    public partial class dlgSpotcheckWarning : Form
    {
        public dlgSpotcheckWarning()
        {
            InitializeComponent();
        }

        public void FillList(List<string> paths)
        {
            foreach (string path in paths)
                lstFiles.Items.Add(path);
        }

        private void cmdYes_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            Close();
        }

        private void cmdNo_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.No;
            Close();
        }
    }
}
