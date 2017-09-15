using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostEdit.Compare;
using PostEdit.Compare.Model;

namespace Sdl.Community.PostEdit.Compare
{
	public partial class SkipSettingsWindow : Form
	{
		
		public SkipSettingsWindow()
		{
			InitializeComponent();
		}

		private void skippBtn_Click(object sender, EventArgs e)
		{
			Close();
		
		}
	}
}
