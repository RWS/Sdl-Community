using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sdl.Community.Amgen
{
	public partial class AmgenForm : Form
	{
		private List<string> _fileNames = new List<string>();
	
		public AmgenForm()
		{
			InitializeComponent();
		}


		private void btn_BrowseFiles_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.InitialDirectory = (@"C:\");
			ofd.Filter = ("*SDLXLIFF Files(*.sdlxliff) | *.sdlxliff");
			ofd.CheckFileExists = false;
			ofd.Multiselect = true;

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				foreach (var fileName in ofd.FileNames)
				{
					txt_SdlxliffFiles.Text = txt_SdlxliffFiles.Text + fileName + ";";
					_fileNames.Add(fileName);
				}
				txt_SdlxliffFiles.Text.Remove(txt_SdlxliffFiles.Text.Length - 1);
			}
		}
		
		private void btn_ConvertFiles_Click(object sender, EventArgs e)
		{
			foreach(var fileName in _fileNames)
			{

			}
		}

		private void btn_SaveFiles_Click(object sender, EventArgs e)
		{

		}
	}
}