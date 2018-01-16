using Sdl.Community.Utilities.TMTool.Lib;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.Utilities.TMTool.Tasks.RemoveDuplicates
{
	public partial class AccessRightsForm : Form
	{
		private TMFileManager _tm;

		/// <summary>
		/// initializes new AccessRightsForm
		/// </summary>
		/// <param name="TM"></param>
		public AccessRightsForm(TMFileManager TM)
		{
			InitializeComponent();

			_tm = TM;
		}

		/// <summary>
		/// value indicating wheather user-entered psw was successfully accepted
		/// </summary>
		public bool IsPswAccepted
		{ get; private set; }

		#region events
		private void btnOK_Click(object sender, EventArgs e)
		{
			if (tbPSW.Text.Length < 1)
			{
				MessageBox.Show(Properties.Resources.errEmptyPsw, Properties.Resources.Title);
				return;
			}

			if (!_tm.OpenWithPassword(tbPSW.Text))
			{
				MessageBox.Show(Properties.Resources.errInvalidPsw, Properties.Resources.Title);
				return;
			}

			IsPswAccepted = true;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void AccessRightsForm_Load(object sender, EventArgs e)
		{
			lblInfo.Text = string.Format(Properties.Resources.accessSubTitle, Path.GetFileNameWithoutExtension(_tm.TMFilePath));
		}
		#endregion
	}
}