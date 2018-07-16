using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.TMLifting.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.TMLifting
{
	public partial class LoginPage : Form
	{
		public AddServerBasedTMsDetails _addDetailsCallback;
		public LoginPage(string serverName)
		{
			InitializeComponent();
			serverNameTxtBox.Text = serverName;
		}

		private void serverNameTxtBox_TextChanged(object sender, EventArgs e)
		{

		}

		private async void btnOkServerBased_Click(object sender, EventArgs e)
		{
			//Properties.Settings.Default.UserName = userNameTxtBox.Text;
			//Properties.Settings.Default.Password = passwordTxtBox.Text;
			//Properties.Settings.Default.Uri = serverNameTxtBox.Text;
			//Properties.Settings.Default.Save();


			_addDetailsCallback(userNameTxtBox.Text, passwordTxtBox.Text, serverNameTxtBox.Text);
			this.Close();
		}

		private void cancelBtnServerBased_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
