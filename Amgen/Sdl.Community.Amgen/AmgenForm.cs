using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.Community.Amgen.Core;

namespace Sdl.Community.Amgen
{
	public partial class AmgenForm : Form
	{
		private List<string> _filePaths = new List<string>();
	
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
				foreach (var filePath in ofd.FileNames)
				{
					txt_SdlxliffFiles.Text = txt_SdlxliffFiles.Text + filePath + ";";
					_filePaths.Add(filePath);
				}
				txt_SdlxliffFiles.Text.Remove(txt_SdlxliffFiles.Text.Length - 1);
			}
		}
		
		private void btn_ConvertFiles_Click(object sender, EventArgs e)
		{
			foreach(var filePath in _filePaths)
			{
				ReadFile(filePath);
			}
		}

		private void btn_SaveFiles_Click(object sender, EventArgs e)
		{

		}

		private static void ReadFile(string filePath)
		{
			var processor = new Processor();

			try
			{
				var options = new ProcessorOptions
				{
					SourceToTargetCopier = new SourceToTargetHandler
					{
						CopySourceToTaret = false,
						Preserve = true
					}
				};

				var segmentInfos = processor.ReadFile(filePath, options);

				if (segmentInfos == null)
					return;

				foreach (var segmentInfo in segmentInfos)
				{
					// ToDO: add guid
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}			
		}
	}
}