using System;
using System.Windows.Forms;

namespace Sdl.Community.TQA
{
    public partial class HelpForm : Form
    {
        public HelpForm()
        {
            InitializeComponent();

            label2.Text = "To generate report just open desired project and click\"Run TQA\" button.\nYou can run TQA only on 1 project in the same time.\nIf report does not contain any TQA items them output file won't be generated.\nIn case of any issues please contact: kpeka@sdl.com";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
