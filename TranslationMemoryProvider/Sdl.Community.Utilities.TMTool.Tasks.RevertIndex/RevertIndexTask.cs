using Sdl.Community.Utilities.TMTool.Lib;
using Sdl.Community.Utilities.TMTool.Task;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;


namespace Sdl.Community.Utilities.TMTool.Tasks.RevertIndex
{
	public class RevertIndexTask : ITask
	{
		private Guid _taskGuid = new Guid("69fd83e1-a1db-49ea-aa8e-5b2ca59fbcb5");
		private const string _taskFileExt = "*.sdltm";

		delegate bool UpdatePswDelegate(TMFileManager value);

		public event OnProgressDelegate OnProgress;
		public event OnAddLogDelegate OnLogAdded;

		/// <summary>
		/// initializes new task
		/// </summary>
		public RevertIndexTask()
		{
			TaskID = _taskGuid;
			TaskName = Properties.Resources.taskName;
			SupportedFileTypes = new Dictionary<string, string>();
			SupportedFileTypes.Add(_taskFileExt, string.Format(Properties.Resources.taskFileExt_Desc, _taskFileExt));

			Control = new RevertIndexControl();
		}

		/// <summary>
		/// task unique identifier
		/// </summary>
		public Guid TaskID
		{
			get;
			private set;
		}
		/// <summary>
		/// task name
		/// </summary>
		public string TaskName
		{
			get;
			private set;
		}
		/// <summary>
		/// file types that can be processed by current task
		/// key - filter, value - description
		/// e.g. key - "*.sdltm", value - "SDL Translation Memory files"
		/// </summary>
		public Dictionary<string, string> SupportedFileTypes
		{
			get;
			private set;
		}
		/// <summary>
		/// settings user control object
		/// </summary>
		public IControl Control
		{
			get;
			private set;
		}

		/// <summary>
		/// performs task
		/// </summary>
		/// <param name="fileName">file full path to perform task on</param>
		public void Execute(string fileName)
		{
			if (fileName.Length > 0)
			{
				RevertIndexSettings userSettings = (RevertIndexSettings)Control.Options;

				Progress(0, -1);
				LogAdded(string.Format(Properties.Resources.logFileProcessing, fileName));

				try
				{
					TMFileManager manager = new TMFileManager(fileName);
					manager.OnProgress += new TMFileManager.OnProgressDelegate(Progress);

					// check password
					if (manager.IsProtected && !IsPswValid(manager))
						LogAdded(string.Format(Properties.Resources.logCancelledFile, fileName));
					else
					{
						// settings
						DataImportSettings setts = new DataImportSettings(userSettings.Scenario);
						setts.OverwriteExistingTUs = userSettings.IsOverwriteTUs;
						setts.PreservePsw = userSettings.IsPreservePsw;
						setts.CreateBackupFile = false;

						// progress & perform task
						manager.ReverseIndex(string.Format(@"{0}\{1}",
								userSettings.TargetFolder,
								Path.GetFileName(fileName)),
								setts);
						LogAdded(Properties.Resources.logTaskFinished);
						LogAdded(string.Format(Properties.Resources.logTUDetails,
							manager.TUsCount,
							manager.TUsImportedCount));

					}
				}
				catch (Exception ex)
				{
					LogAdded(string.Format(Properties.Resources.errRevertTaskFailed,
						fileName, ex.Message));
					MessageBox.Show(string.Format(Properties.Resources.errRevertTaskFailed,
						fileName, ex.Message).Replace("\\r\\n", "\r\n"),
						Properties.Resources.Title);
				}

				Progress(100, -1);
			}
		}

		/// <summary>
		/// opens AccessRightsForm form, checks TM password entered by user
		/// </summary>
		/// <param name="manager"></param>
		/// <returns>true - if psw was successfully accepted</returns>
		private bool IsPswValid(TMFileManager manager)
		{
			Form parent = this.Control.UControl.ParentForm;
			if (parent.InvokeRequired)
			{
				// not in the UI thread, so need to call BeginInvoke
				IAsyncResult ar = parent.BeginInvoke(new UpdatePswDelegate(IsPswValid), new object[] { manager });
				return (bool)parent.EndInvoke(ar);
			}

			AccessRightsForm pswForm = new AccessRightsForm(manager);
			pswForm.ShowDialog();

			if (!pswForm.IsPswAccepted)
				MessageBox.Show(string.Format(Properties.Resources.accessFileSkipped,
					Path.GetFileNameWithoutExtension(manager.TMFilePath)),
					Properties.Resources.Title);

			return pswForm.IsPswAccepted;
		}

		/// <summary>
		/// reports file processing progress to the application
		/// </summary>
		/// <param name="progress">processed percent</param>
		/// <param name="operationNum">the number of operation
		/// (to report operation title if there are several of them)</param>
		private void Progress(double progress, int operationNum)
		{
			string msg = "";
			if (operationNum == 1)
				msg = Properties.Resources.progressOperationImport;
			else if (operationNum == 0)
				msg = Properties.Resources.progressOperationExport;
			if (this.OnProgress != null)
			{
				this.OnProgress(progress, msg);
			}
		}
		/// <summary>
		/// reports log message to the application
		/// </summary>
		/// <param name="logMsg">message to report</param>
		private void LogAdded(string logMsg)
		{
			if (this.OnLogAdded != null)
			{
				this.OnLogAdded(logMsg);
			}
		}
	}
}