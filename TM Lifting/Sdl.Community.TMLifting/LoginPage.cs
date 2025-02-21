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
		public LoginPage()
		{
			InitializeComponent();
		}

		private void serverNameTxtBox_TextChanged(object sender, EventArgs e)
		{

		}

		private void btnOkServerBased_Click(object sender, EventArgs e)
		{
			_addDetailsCallback(userNameTxtBox.Text, passwordTxtBox.Text, serverNameTxtBox.Text);
			Close();
		}

		private void cancelBtnServerBased_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
