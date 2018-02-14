using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TQA
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
            this.Close();
        }
    }
}
