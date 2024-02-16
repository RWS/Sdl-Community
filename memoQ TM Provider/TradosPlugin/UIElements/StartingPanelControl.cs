using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TradosPlugin
{
    public partial class StartingPanelControl : UserControl
    {
        public delegate void MemoQServerClickedHandler();
        public delegate void LTClickedHandler();
        public event MemoQServerClickedHandler MemoQServerClicked;
        public event LTClickedHandler LTClicked;

        public StartingPanelControl()
        {
            InitializeComponent();

            lblStartText.Text = PluginResources.lblStartText;
        }

        private void pbServer_Click(object sender, EventArgs e)
        {
            if (MemoQServerClicked != null) MemoQServerClicked();
        }

        private void pbLT_Click(object sender, EventArgs e)
        {
           
        }
    }
}
