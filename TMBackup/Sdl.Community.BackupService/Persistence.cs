using Newtonsoft.Json;
using Sdl.Community.BackupService.Helpers;
using Sdl.Community.BackupService.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Sdl.Community.BackupService
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
						else
						{
							MessageBox.Show(Constants.ActionAlreadyExist, Constants.InformativeMessage, MessageBoxButtons.OK);
						}
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

		public void UpdateBackupDetailsForm(List<BackupDetailsModel> backupDetailsModelList)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			if(request != null && request.BackupDetailsModelList.Any())
			{
				request.BackupDetailsModelList.Clear();
				request.BackupDetailsModelList = backupDetailsModelList;

				var json = JsonConvert.SerializeObject(request);
				File.WriteAllText(_persistancePath, json);
			}
		}

		public void DeleteDetailsFromInfo(List<BackupDetailsModel> removedBackupDetailsList)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request != null)
			{
				foreach (var item in removedBackupDetailsList)
				{
					var requestItem = request.BackupDetailsModelList
						.Where(r => r.BackupAction == item.BackupAction && r.BackupType == item.BackupType && r.BackupPattern == item.BackupPattern)
						.FirstOrDefault();
					if (requestItem != null)
					{
						request.BackupDetailsModelList.Remove(requestItem);
					}
				}
				var json = JsonConvert.SerializeObject(request);
				File.WriteAllText(_persistancePath, json);
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