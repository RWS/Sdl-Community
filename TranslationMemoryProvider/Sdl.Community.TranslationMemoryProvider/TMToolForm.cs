using Sdl.Community.Utilities.TMTool.Task;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Sdl.Utilities.TMTool;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace Sdl.Community.TranslationMemoryManagementUtility
{
	public partial class TMToolForm : Form
	{
		OptionsPanel _options;

		/// <summary>
		/// list of files added by user
		/// </summary>
		List<string> _files;
		/// <summary>
		/// manages list of ITask objetcs and selected task
		/// </summary>
		TasksManager _tManager;

		/// <summary>
		/// initializes new TMToolForm
		/// </summary>
		/// <param name="tasks">list of tasks to perform on TM</param>
		public TMToolForm(List<ITask> tasks)
		{
			InitializeComponent();

			_files = new List<string>();
			_tManager = new TasksManager(tasks);
		}

		/// <summary>
		/// show tasks to perform on TM to treeview
		/// </summary>
		private void bindTasks()
		{
			tvTasks.Nodes.Clear();

			TreeNode parent = new TreeNode("TM Task");
			foreach (KeyValuePair<string, string> taskInfo in _tManager.GetTaskNames())
				parent.Nodes.Add(taskInfo.Key, taskInfo.Value);
			tvTasks.Nodes.Add(parent);

			tvTasks.AfterSelect += new TreeViewEventHandler(tvTasks_AfterSelect);

			tvTasks.ExpandAll();
		}

		/// <summary>
		/// form file dialog filter string
		/// </summary>
		/// <returns>filter string</returns>
		private string GetFileDialogFilter()
		{
			StringBuilder filter = new StringBuilder();
			foreach (KeyValuePair<string, string> fType in _tManager.GetSupportedFileTypes())
			{
				filter.Append(fType.Value);
				filter.Append("|");
				filter.Append(fType.Key);
				filter.Append("|");
			}

			return filter.Remove(filter.Length - 1, 1).ToString();
		}

		private void RemoveSelectedFile()
		{
			if (lvFiles.SelectedItems.Count > 0)
			{
				for (int i = lvFiles.Items.Count - 1; i >= 0; i--)
					if (lvFiles.Items[i].Selected)
					{
						lvFiles.Items.RemoveAt(i);
						_files.RemoveAt(i);
					}
			}
			else MessageBox.Show(PluginResources.filesEmptySelectedList, PluginResources.Title);
		}

		private void AddFile(string filePath)
		{
			if (!_files.Contains(filePath))
			{
				lvFiles.Items.Add(filePath);
				_files.Add(filePath);
			}
		}

		private void SetOptionsSize()
		{
			if (_tManager != null && _tManager.SelectedTask != null)
			{
				_tManager.SelectedTask.Control.UControl.Height = tvTasks.Height;
				_tManager.SelectedTask.Control.UControl.Width = this.Width - tvTasks.Width - 30;
			}
		}

		private void SetPanelMinSize()
		{
			// calculate min panel size
			int minWidth = _tManager.GetMinWidth() + 6;
			int minHeight = _tManager.GetMinHeight();
			int minWidthShift = minWidth - scTasks.Panel2MinSize;
			int minHeightShift = minHeight - scTasks.Height + 100;

			int panelWidth = scTasks.Width;
			int splitterDist = scTasks.SplitterDistance;
			int windowLVMargin = this.Size.Width - lvFiles.Columns[0].Width;

			if (minWidthShift != 0 || minHeightShift != 0)
			{
				// panel size
				scTasks.Width = scTasks.Width + (minWidth - scTasks.Panel2.Width);

				// window size
				this.Size = new Size(this.Size.Width + (scTasks.Width - panelWidth),
					this.Size.Height + (minHeightShift > 0 ? minHeightShift : 0));

				// listview column size
				lvFiles.Columns[0].Width = this.Size.Width - windowLVMargin;

				// min window size
				this.MinimumSize = new Size(this.Width,
					this.MinimumSize.Height + minHeightShift);

				// min panel size
				scTasks.Panel2MinSize = minWidth;
				scTasks.MinimumSize = new Size(scTasks.Panel1MinSize + scTasks.Panel2MinSize + scTasks.SplitterWidth,
					minHeight);

				scTasks.SplitterDistance = splitterDist;
			}
		}

		private string ValidateFiles()
		{
			if (_files.Count < 1)
			{
				return PluginResources.findFilesNotAdded;
			}
			else
			{
				return string.Empty;
			}
		}

		#region events
		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnSplitInFileAdd_Click(object sender, EventArgs e)
		{
			OpenFileDialog a = new OpenFileDialog();
			a.Title = PluginResources.filesFileDialogTitle;
			a.Multiselect = true;
			a.Filter = GetFileDialogFilter();
			if (a.ShowDialog() == DialogResult.OK)
			{
				string inFile = "";
				foreach (string file in a.FileNames)
				{
					inFile = file.Trim().Replace("/", @"\");
					AddFile(inFile);
				}
			}
		}

		private void btnSplitInFileRemove_Click(object sender, EventArgs e)
		{
			RemoveSelectedFile();
		}

		private void btnSplitRemoveAll_Click(object sender, EventArgs e)
		{
			if (lvFiles.Items.Count > 0)
			{
				lvFiles.Items.Clear();
				_files.Clear();
			}
			else MessageBox.Show(PluginResources.filesEmptyList, PluginResources.Title);
		}

		private void btnPerform_Click(object sender, EventArgs e)
		{
			ISettings taskOptions = _tManager.GetSelectedTaskSettings();
			if (taskOptions != null)
			{
				string errMsg = ValidateFiles();
				if (errMsg.Length > 0 || !taskOptions.ValidateSettings(out errMsg))
				{
					MessageBox.Show(errMsg, PluginResources.Title);
					return;
				}

				TMResultsForm results = new TMResultsForm(_files, _tManager.SelectedTask);
				results.ShowDialog();
			}
		}

		private void lvFiles_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void lvFiles_DragDrop(object sender, DragEventArgs e)
		{
			string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
			string inFile = "";
			foreach (string file in fileList)
			{
				if (File.Exists(file))
				{
					inFile = file.Trim().Replace("/", @"\");
					if (Path.GetExtension(inFile).ToLower() == ".sdltm")
						AddFile(inFile);
				}
			}
		}

		private void lvFiles_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;
		}

		private void scTasks_SplitterMoved(object sender, SplitterEventArgs e)
		{
			SetOptionsSize();
		}

		private void TMToolForm_Resize(object sender, EventArgs e)
		{
			SetOptionsSize();
		}

		private void TMToolForm_Load(object sender, EventArgs e)
		{
			// create treeview items
			bindTasks();

			// create options panel
			_options = new OptionsPanel();
			scTasks.Panel2.Controls.Add(_options);
			SetPanelMinSize();

			btnPerform.Enabled = false;
		}

		private void tvTasks_AfterSelect(object sender, TreeViewEventArgs e)
		{
			IControl optionsControl = null;

			// remove all the option controls
			scTasks.Panel2.Controls.Clear();

			// try to set options panel
			if (e.Node != null)
			{
				_tManager.SelectTask(e.Node.Name);
				optionsControl = _tManager.GetSelectedTaskControl();
				if (optionsControl != null)
				{
					scTasks.Panel2.Controls.Add(optionsControl.UControl);
					SetOptionsSize();
					btnPerform.Enabled = true;
				}
			}

			// set default panel
			if (e.Node == null || optionsControl == null)
			{
				scTasks.Panel2.Controls.Add(_options);
				btnPerform.Enabled = false;
			}
		}
		#endregion
	}
}