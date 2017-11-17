using Sdl.Community.TMBackup.Helpers;
using System;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Models;
using Microsoft.Win32.TaskScheduler;
using System.IO;
using System.Collections.Generic;

namespace Sdl.Community.TMBackup
{
	public partial class TMBackupForm : Form
	{
		public TMBackupForm()
		{
			InitializeComponent();

			GetBackupFormInfo();
		}

		private void btn_BackupFrom_Click(object sender, EventArgs e)
		{
			var fromFolderDialog = new FolderSelectDialog();
			txt_BackupFrom.Text = string.Empty;

			if (fromFolderDialog.ShowDialog())
			{
				if (fromFolderDialog.Files.Any())
				{
					foreach (var folderName in fromFolderDialog.Files)
					{
						txt_BackupFrom.Text = txt_BackupFrom.Text + folderName + ";";
					}
					txt_BackupFrom.Text.Remove(txt_BackupFrom.Text.Length - 1);
				}
			}
		}

		private void btn_BackupTo_Click(object sender, EventArgs e)
		{
			var toFolderDialog = new FolderSelectDialog();

			if (toFolderDialog.ShowDialog())
			{
				txt_BackupTo.Text = toFolderDialog.FileName;
			}
		}

		private void btn_Change_Click(object sender, EventArgs e)
		{
			TMBackupChangeForm changeForm = new TMBackupChangeForm();
			changeForm.ShowDialog();

			txt_BackupTime.Text = changeForm.GetBackupTimeInfo();
		}

		private void btn_Details_Click(object sender, EventArgs e)
		{
			TMBackupDetailsForm detailsForm = new TMBackupDetailsForm();
			detailsForm.ShowDialog();

			txt_BackupDetails.Text = TMBackupDetailsForm.BackupDetailsInfo;

			GetBackupFormInfo();
		}

		private void btn_Cancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btn_SaveSettings_Click(object sender, EventArgs e)
		{
			BackupModel backupModel = new BackupModel();
			backupModel.BackupFrom = txt_BackupFrom.Text;
			backupModel.BackupTo = txt_BackupTo.Text;
			backupModel.Description = txt_Description.Text;
			backupModel.BackupDetails = txt_BackupDetails.Text;
			backupModel.BackupTime = txt_BackupTime.Text;

			Persistence persistence = new Persistence();
			persistence.SaveBackupFormInfo(backupModel);

			this.Close();

			//CreateTaskScheduler();
			BackupFilesRecursive(txt_BackupFrom.Text);
		}

		private void GetBackupFormInfo()
		{
			Persistence persistence = new Persistence();
			var result = persistence.ReadFormInformation();

			if (result.BackupModel != null)
			{
				txt_BackupFrom.Text = result.BackupModel.BackupFrom;
				txt_BackupTo.Text = result.BackupModel.BackupTo;
				txt_BackupTime.Text = result.BackupModel.BackupTime;
				txt_Description.Text = result.BackupModel.Description;
			}

			if (result.BackupDetailsModelList != null)
			{
				string res = string.Empty;
				foreach (var backupDetail in result.BackupDetailsModelList)
				{
					res = res + backupDetail.BackupAction + ", " + backupDetail.BackupType + ", " + backupDetail.BackupPattern + ";  ";
				}
				txt_BackupDetails.Text = res;
			}

			TMBackupChangeForm tmBackupChangeForm = new TMBackupChangeForm();
			txt_BackupTime.Text = tmBackupChangeForm.GetBackupTimeInfo();
		}

		private void CreateTaskScheduler()
		{
			Persistence persistence = new Persistence();
			var jsonRequestModel = persistence.ReadFormInformation();

			// Get the service on the remote machine
			using (TaskService ts = new TaskService(string.Empty))
			{
				// Create a new task definition and assign properties
				TaskDefinition td = ts.NewTask();
				td.RegistrationInfo.Description = "Backup files";

				// Create a trigger that will fire the task at this time every other day
				td.Triggers.Add(new DailyTrigger { DaysInterval = 2 });

				// Create an action that will launch Notepad whenever the trigger fires
				td.Actions.Add(new ExecAction("notepad.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\TMBackup\TaskScheduler.log", null)));

				// Register the task in the root folder
				ts.RootFolder.RegisterTaskDefinition(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\TMBackup"), td);
			}
		}

		private string GetAcceptedRequestsFolder(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		internal void BackupFilesRecursive(string sourcePaths)
		{
			List<string> splittedSourcePathList = sourcePaths.Split(';').ToList<string>();

			foreach (var sourcePath in splittedSourcePathList)
			{
				if (!string.IsNullOrEmpty(sourcePath))
				{
					var acceptedRequestFolder = GetAcceptedRequestsFolder(sourcePath);

					// create the directory "Accepted request"
					if (!Directory.Exists(txt_BackupTo.Text))
					{
						Directory.CreateDirectory(txt_BackupTo.Text);
					}

					var files = Directory.GetFiles(sourcePath);
					if (files.Length != 0)
					{
						MoveFilesToAcceptedFolder(files, txt_BackupTo.Text);
					} //that means we have a subfolder in watch folder
					else
					{

						var subdirectories = Directory.GetDirectories(sourcePath);
						foreach (var subdirectory in subdirectories)
						{
							var currentDirInfo = new DirectoryInfo(subdirectory);
							CheckForSubfolders(currentDirInfo, acceptedRequestFolder);

						}
					}
				}
			}
		}

		private void CheckForSubfolders(DirectoryInfo directory, string root)
		{
			var sourcePath = this.txt_BackupFrom.Text;
			var subdirectories = directory.GetDirectories();
			var path = root + @"\" + directory.Parent;
			var subdirectoryFiles = Directory.GetFiles(directory.FullName);
			if (subdirectoryFiles.Length != 0)
			{
				var subdirectoryToMovePath = Path.Combine(path, directory.Name);
				if (!Directory.Exists(subdirectoryToMovePath))
				{
					Directory.CreateDirectory(subdirectoryToMovePath);
				}
				MoveFilesToAcceptedFolder(subdirectoryFiles, subdirectoryToMovePath);

			}
			if (subdirectories.Length != 0)
			{
				foreach (var subdirectory in subdirectories)
				{
					CheckForSubfolders(subdirectory, path);
				}
			}

		}
		private void MoveFilesToAcceptedFolder(string[] files, string acceptedFolderPath)
		{
			foreach (var subFile in files)
			{
				var dirName = new DirectoryInfo(subFile).Name;
				var parentName = new DirectoryInfo(subFile).Parent != null ? new DirectoryInfo(subFile).Parent.Name : string.Empty;

				var fileName = subFile.Substring(subFile.LastIndexOf(@"\", StringComparison.Ordinal));
				var destinationPath = Path.Combine(acceptedFolderPath, parentName);
				if (!Directory.Exists(destinationPath))
				{
					Directory.CreateDirectory(destinationPath);
				}
				try
				{
					File.Copy(subFile, destinationPath + fileName, true);
				}
				catch (Exception e)
				{
				}
			}
		}
	}
}