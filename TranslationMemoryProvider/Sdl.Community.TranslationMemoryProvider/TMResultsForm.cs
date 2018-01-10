using Sdl.Community.Utilities.TMTool.Task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace Sdl.Community.TranslationMemoryProvider
{
	public partial class TMResultsForm : Form
	{
		delegate void UpdateProgressDelegate(double value, string msg);
		delegate void UpdateFilesProgressDelegate(int value);
		delegate void UpdateLogDelegate(string value);
		delegate void UpdateUIDelegate(bool value);

		/// <summary>
		/// list of files to process
		/// </summary>
		private List<string> _files;
		/// <summary>
		/// task to perform
		/// </summary>
		private ITask _task;
		/// <summary>
		/// folder processed files were created in
		/// (can be empty if task does not require target folder)
		/// </summary>
		private string _targetFolder;

		/// <summary>
		/// initializes new TMResultsForm
		/// </summary>
		/// <param name="files">list of files to perform task on</param>
		/// <param name="currTask">task to perform</param>
		public TMResultsForm(List<string> files, ITask currTask)
		{
			InitializeComponent();

			_files = files;
			_task = currTask;
			_task.OnProgress += new OnProgressDelegate(task_OnProgress);
			_task.OnLogAdded += new OnAddLogDelegate(WriteLog);

			SetTargetFolder();
		}

		/// <summary>
		/// set target folder mode (visible/hidden) & value
		/// </summary>
		private void SetTargetFolder()
		{
			btnOpenFolder.Visible = false;

			ISettings settings = _task.Control.Options;
			if (settings != null)
				if (settings.TargetFolder != null && settings.TargetFolder.Length > 0)
				{
					_targetFolder = settings.TargetFolder;
					btnOpenFolder.Visible = true;
				}
		}

		/// <summary>
		/// set buttons mode (enable/disable)
		/// </summary>
		/// <param name="isEnabled"></param>
		private void EnableButtons(bool isEnabled)
		{
			if (InvokeRequired)
			{
				// not in the UI thread, so need to call BeginInvoke
				BeginInvoke(new UpdateUIDelegate(EnableButtons), new object[] { isEnabled });
				return;
			}

			btnClose.Enabled = isEnabled;
			btnOpenFolder.Enabled = isEnabled;
		}

		/// <summary>
		/// write message in log textbox
		/// </summary>
		/// <param name="msg">message to write</param>
		private void WriteLog(string msg)
		{
			if (InvokeRequired)
			{
				// not in the UI thread, so need to call BeginInvoke
				BeginInvoke(new UpdateLogDelegate(WriteLog), new object[] { msg });
				return;
			}
			tbLog.AppendText(msg.Replace("\\r\\n", System.Environment.NewLine));
		}

		/// <summary>
		/// starts new thread to perform task in
		/// </summary>
		private void StartTask()
		{
			EnableButtons(false);

			// start split process
			Thread t = null;
			t = new Thread(new ThreadStart(PerformTask));
			t.IsBackground = true;
			t.Start();
		}

		/// <summary>
		/// performs task
		/// </summary>
		private void PerformTask()
		{
			ResourceManager rm = new ResourceManager("Sdl.Community.TranslationMemoryProvider.PluginResource", Assembly.GetExecutingAssembly());

			WriteLog(string.Format(rm.GetString("logTaskStarted", CultureInfo.CurrentCulture), _task.TaskName));

			int filesCount = 0;
			foreach (string file in _files)
			{
				tmManager_OnFilesProgress(filesCount++);
				if (IsFileExtValid(file))
				{
					_task.Execute(file);
				}
				else
					WriteLog(string.Format(rm.GetString("errFileNotSupported", CultureInfo.CurrentCulture),
						file));
			}

			tmManager_OnFilesProgress(filesCount);

			EnableButtons(true);
		}

		/// <summary>
		/// checks if selected task can process file with current extension
		/// </summary>
		/// <param name="fName">file path</param>
		/// <returns></returns>
		private bool IsFileExtValid(string fName)
		{
			if (_task.SupportedFileTypes.ContainsKey("*.*"))
				return true;

			string fileExt = Path.GetExtension(fName).ToLower();
			foreach (KeyValuePair<string, string> ext in _task.SupportedFileTypes)
				if (Path.GetExtension(ext.Key).ToLower() == fileExt)
					return true;

			return false;
		}

		#region events

		private void btnOpenFolder_Click(object sender, EventArgs e)
		{
			if (_targetFolder != null && _targetFolder.Length > 0)
			{
				System.Diagnostics.Process prc = new System.Diagnostics.Process();
				prc.StartInfo.FileName = _targetFolder;
				prc.Start();
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void TMResultsForm_Load(object sender, EventArgs e)
		{
			StartTask();
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			string logText = tbLog.Text;

			SaveFileDialog dialog = new SaveFileDialog() { CreatePrompt = true };
			dialog.Filter = "Text Document |*.txt";
			dialog.Title = "Save Log File";
			dialog.DefaultExt = ".txt";
			if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				using (StreamWriter writer = new StreamWriter(dialog.FileName))
				{
					writer.Write(logText);
					writer.Close();
				}
			}
		}

		#endregion

		/// <summary>
		/// change UI on task progress event
		/// </summary>
		/// <param name="progress">percent of processed</param>
		/// <param name="operationMsg">additional inforrmation on operation currently processing</param>
		private void task_OnProgress(double progress, string operationMsg)
		{		
			if (InvokeRequired)
			{
				// not in the UI thread, so need to call BeginInvoke
				BeginInvoke(new UpdateProgressDelegate(task_OnProgress), new object[] { progress, operationMsg });
				return;
			}

			pbProgress.Value = (int)progress;
			lblOperation.Text = operationMsg;
		}
		/// <summary>
		/// change UI on files progress event
		/// </summary>
		/// <param name="fileNum">file number currently processing</param>
		private void tmManager_OnFilesProgress(int fileNum)
		{
			ResourceManager rm = new ResourceManager("Sdl.Community.TranslationMemoryProvider.PluginResource", Assembly.GetExecutingAssembly());

			if (InvokeRequired)
			{
				// not in the UI thread, so need to call BeginInvoke
				BeginInvoke(new UpdateFilesProgressDelegate(tmManager_OnFilesProgress), new object[] { fileNum });
				return;
			}

			pbProgressFiles.Value = (int)(fileNum * 100) / _files.Count;
			if (fileNum == _files.Count)
				pbProgress.Text = rm.GetString("progressFileFinished", CultureInfo.CurrentCulture);
			else
				pbProgress.Text = string.Format(rm.GetString("progressFile", CultureInfo.CurrentCulture), ++fileNum, _files.Count);
		}
	}
}