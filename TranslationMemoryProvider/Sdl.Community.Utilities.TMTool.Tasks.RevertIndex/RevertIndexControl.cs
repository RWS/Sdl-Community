using Sdl.Community.Utilities.TMTool.Task;
using Sdl.LanguagePlatform.TranslationMemory;
using System;
using System.IO;
using System.Windows.Forms;

namespace Sdl.Community.Utilities.TMTool.Tasks.RevertIndex
{
	public partial class RevertIndexControl : UserControl, IControl
	{
		private RevertIndexSettings _settings;

		/// <summary>
		/// initializes new RevertIndexControl object
		/// </summary>
		public RevertIndexControl()
		{
			InitializeComponent();

			_settings = new RevertIndexSettings();
			ResetUI();
		}

		/// <summary>
		/// settings user control
		/// </summary>
		public UserControl UControl
		{
			get { return this; }
		}
		/// <summary>
		/// current settings
		/// </summary>
		public ISettings Options
		{
			get
			{
				GetSettings();
				return _settings;
			}
		}

		/// <summary>
		/// reset UI view, set default Options values
		/// </summary>
		public void ResetUI()
		{
			_settings.ResetToDefaults();
			UpdateUI(_settings);
		}
		/// <summary>
		/// updates UI with settings
		/// </summary>
		/// <param name="settings">settings to update to</param>
		public void UpdateUI(ISettings settings)
		{
			if (settings != null && settings.GetType() == _settings.GetType())
			{
				_settings = (RevertIndexSettings)settings;
				chOverwriteTUs.Checked = _settings.IsOverwriteTUs;
				chPreservePsw.Checked = _settings.IsPreservePsw;
				tbTargetFile.Text = _settings.TargetFolder;
				switch (_settings.Scenario)
				{
					case ImportSettings.ImportTUProcessingMode.ProcessRawTUOnly:
						rbPresegmentedSc.Checked = true;
						break;
					case ImportSettings.ImportTUProcessingMode.ProcessBothTUs:
						rbMixedSc.Checked = true;
						break;
					default:
						rbDefaultSc.Checked = true;
						break;
				}
			}
		}

		/// <summary>
		/// get settings from UI
		/// </summary>
		private void GetSettings()
		{
			_settings.IsOverwriteTUs = chOverwriteTUs.Checked;
			_settings.IsPreservePsw = chPreservePsw.Checked;
			_settings.TargetFolder = tbTargetFile.Text.Replace("/", @"\");

			_settings.Scenario = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly;
			if (rbPresegmentedSc.Checked)
				_settings.Scenario = ImportSettings.ImportTUProcessingMode.ProcessRawTUOnly;
			else if (rbMixedSc.Checked)
				_settings.Scenario = ImportSettings.ImportTUProcessingMode.ProcessBothTUs;
		}

		#region events
		private void btnBrowse_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
			folderBrowserDialog1.ShowNewFolderButton = true;
			folderBrowserDialog1.Description = Properties.Resources.msgTargetFolder;
			if (Directory.Exists(tbTargetFile.Text))
				folderBrowserDialog1.SelectedPath = tbTargetFile.Text;
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
				tbTargetFile.Text = folderBrowserDialog1.SelectedPath;
		}

		private void tbTargetFile_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && ((string[])e.Data.GetData(DataFormats.FileDrop, false)).Length == 1)
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void tbTargetFile_DragDrop(object sender, DragEventArgs e)
		{
			string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			if (pathList.Length > 0)
			{
				string path = (File.Exists(pathList[0]) ? Path.GetDirectoryName(pathList[0]) : pathList[0]).Replace("/", @"\");
				if (Directory.Exists(path))
					tbTargetFile.Text = path;
			}
		}
		#endregion
	}
}