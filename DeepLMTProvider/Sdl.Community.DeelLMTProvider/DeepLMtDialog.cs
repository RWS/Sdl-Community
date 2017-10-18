using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sdl.Community.DeepLMTProvider
{
    public partial class DeepLMtDialog : Form
    {
        public DeepLMtDialog()
        {
			
            InitializeComponent();
			mainTableLayout.CellPaint += MainTableLayout_CellPaint;
		}

		private void MainTableLayout_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
		{
			if (e.Row == 0)
			{
				e.Graphics.FillRectangle(Brushes.White, e.CellBounds);
			}
		}
	}
}
