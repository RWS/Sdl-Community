using Sdl.Community.TMBackup.Models;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Sdl.Community.TMBackup.Helpers;

namespace Sdl.Community.TMBackup
{
	public class Persistence
	{
		private readonly string _persistancePath;

		public Persistence()
		{
			_persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\TMBackup\TMBackup.json");
		}

		private void CheckIfJsonFileExist()
		{
			if (!File.Exists(_persistancePath))
			{
				var directory = Path.GetDirectoryName(_persistancePath);
				if (directory != null && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
				File.WriteAllText(_persistancePath, string.Empty);
			}
		}

		public void SaveBackupFormInfo(BackupModel backupModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request == null)
			{
				request = new JsonRequestModel();
			}

			request.BackupModel = backupModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}
		
		public void SaveDetailsFormInfo(List<BackupDetailsModel> backupDetailsModelList)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request == null)
			{
				request = new JsonRequestModel();
				request.BackupDetailsModelList = backupDetailsModelList;
				var json = JsonConvert.SerializeObject(request);

				File.WriteAllText(_persistancePath, json);
			}
			else
			{
				if (request.BackupDetailsModelList.Any())
				{
					foreach (var backupItem in backupDetailsModelList)
					{
						var existingBackupItem = request.BackupDetailsModelList.Where(b => b.BackupAction == backupItem.BackupAction
																					 && b.BackupType == backupItem.BackupType
																					 && b.BackupPattern == backupItem.BackupPattern)
																			 .FirstOrDefault();
						if (existingBackupItem == null)
						{
							request.BackupDetailsModelList.Add(backupItem);

							var json = JsonConvert.SerializeObject(request);
							File.WriteAllText(_persistancePath, json);
						}
						//else
						//{
						//	MessageBox.Show(Constants.ActionAlreadyExist, Constants.InformativeMessage, MessageBoxButtons.OK);
						//}
					}
				}
				else
				{
					request.BackupDetailsModelList = backupDetailsModelList;

					var json = JsonConvert.SerializeObject(request);
					File.WriteAllText(_persistancePath, json);
				}			
			}			
		}

		public void SaveChangeSettings(ChangeSettingsModel changeSettingsModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request == null)
			{
				request = new JsonRequestModel();
			}

			request.ChangeSettingsModel = changeSettingsModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}

		public void SavePeriodicBackupInfo(PeriodicBackupModel periodicBackupModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request == null)
			{
				request = new JsonRequestModel();
			}

			request.PeriodicBackupModel = periodicBackupModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}

		public void SaveRealTimeInfo(RealTimeBackupModel realTimeBackupModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request == null)
			{
				request = new JsonRequestModel();
			}

			request.RealTimeBackupModel = realTimeBackupModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}

		public JsonRequestModel ReadFormInformation()
		{
			if (File.Exists(_persistancePath))
			{
				var jsonText = File.ReadAllText(_persistancePath);
				var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

				return request;
			}
			return new JsonRequestModel();
		}
	}
}