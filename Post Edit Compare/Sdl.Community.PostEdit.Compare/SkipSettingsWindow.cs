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
		public bool SkipSettings { get; set; }
		public bool CustomizeSettings { get; set; }

		public SkipSettingsWindow()
		{
			InitializeComponent();
		}

		public void skippBtn_Click(object sender, EventArgs e)
		{
			SkipSettings = true;
			CustomizeSettings = false;
			Close();		
		}

		private void reportBtn_Click(object sender, EventArgs e)
		{
			SkipSettings = false;
			CustomizeSettings = true;
			Close();
		}

		private void SkipSettingsWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			Close();
		}

	}
}
