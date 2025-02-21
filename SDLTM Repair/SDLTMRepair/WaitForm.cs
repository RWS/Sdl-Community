using System;
using System.Drawing;
using System.Windows.Forms;

namespace Sdl.Community.TMRepair
{
    public partial class WaitForm : Form
    {
        public WaitForm()
        {
            InitializeComponent();
        }

        private void WaitForm_Load(object sender, EventArgs e)
        {
            labelWait.Location = new Point((Width - labelWait.Width) / 2, Height / 2 + 60);
        }
    }
}
