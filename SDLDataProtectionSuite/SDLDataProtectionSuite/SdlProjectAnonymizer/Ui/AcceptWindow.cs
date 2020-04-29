using System;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Ui
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