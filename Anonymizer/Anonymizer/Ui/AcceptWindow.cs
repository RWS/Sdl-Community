using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.projectAnonymizer.Helpers;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Ui
{
	public partial class AcceptWindow : Form
	{
		public AcceptWindow()
		{
			InitializeComponent();
			descriptionLabel.Text = Constants.AcceptDescription();
		}

		private void acceptBtn_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void acceptBox_CheckedChanged(object sender, EventArgs e)
		{
			var agreement = new Agreement
			{
				Accept = ((CheckBox)sender).Checked
			};
			File.WriteAllText(Constants.AcceptFilePath, JsonConvert.SerializeObject(agreement));
		}
	}
}
