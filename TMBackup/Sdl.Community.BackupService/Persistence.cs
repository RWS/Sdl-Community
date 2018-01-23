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

		public void SaveBackupFormInfo(List<BackupModel> backupModelList)
		{
			if (backupModelList.Any())
			{
				CheckIfJsonFileExist();

				var jsonText = File.ReadAllText(_persistancePath);
				var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
				if (request == null)
				{
					request = new JsonRequestModel();
					request.BackupModelList = backupModelList;
					WriteJsonRequestModel(request);
				}
				else
				{
					if (request.BackupModelList != null)
					{
						foreach (var backupModelItem in backupModelList)
						{
							var existingBackupModelItem = request.BackupModelList.Where(b => b.BackupName == backupModelItem.BackupName
																						&& b.Description == backupModelItem.Description)
																				 .FirstOrDefault();

							if (existingBackupModelItem == null)
							{
								request.BackupModelList.Add(backupModelItem);
								WriteJsonRequestModel(request);
							}
							else
							{
								MessageBox.Show(Constants.TaskSchedulerAlreadyExist, Constants.InformativeMessage, MessageBoxButtons.OK);
							}
						}
					}
					else
					{
						request.BackupModelList = backupModelList;
						WriteJsonRequestModel(request);
					}
				}			
			}
		}

		public void SaveDetailsFormInfo(List<BackupDetailsModel> backupDetailsModelList, string taskName)
		{
			if (backupDetailsModelList != null)
			{
				foreach(var item in backupDetailsModelList)
				{
					item.BackupName = taskName;
					item.TrimmedBackupName = string.Concat(taskName.Where(c => !char.IsWhiteSpace(c)));
				}

				CheckIfJsonFileExist();

				var jsonText = File.ReadAllText(_persistancePath);
				var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
				if (request == null)
				{
					request = new JsonRequestModel();
					request.BackupDetailsModelList = backupDetailsModelList;
					WriteJsonRequestModel(request);
				}
				else
				{
					if (request.BackupDetailsModelList != null)
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
								WriteJsonRequestModel(request);
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
						WriteJsonRequestModel(request);
					}
				}
			}
		}

		public void UpdateBackupDetailsForm(List<BackupDetailsModel> backupDetailsModelList, string taskName)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			if(request != null && request.BackupDetailsModelList.Where(b=>b.BackupName.Equals(taskName)) != null)
			{
				request.BackupDetailsModelList.Clear();
				request.BackupDetailsModelList = backupDetailsModelList;

				WriteJsonRequestModel(request);
			}
		}

		public void DeleteDetailsFromInfo(List<BackupDetailsModel> removedBackupDetailsList, string taskName)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request != null)
			{
				foreach (var item in removedBackupDetailsList)
				{
					var requestItem = request.BackupDetailsModelList
						.Where(r => r.BackupAction == item.BackupAction && r.BackupType == item.BackupType && r.BackupPattern == item.BackupPattern && r.BackupName.Equals(taskName))
						.FirstOrDefault();

					if (requestItem != null)
					{
						request.BackupDetailsModelList.Remove(requestItem);
					}
				}
				WriteJsonRequestModel(request);
			}
		}

		public void SaveChangeSettings(List<ChangeSettingsModel> changeSettingsModelList, string taskName)
		{
			if (changeSettingsModelList != null)
			{
				CheckIfJsonFileExist();

				var jsonText = File.ReadAllText(_persistancePath);
				var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
				if (request == null)
				{
					request = new JsonRequestModel();
					request.ChangeSettingsModelList = changeSettingsModelList;
					WriteJsonRequestModel(request);
				}
				else
				{
					if (request.ChangeSettingsModelList != null)
					{
						foreach (var changeSettingModelItem in changeSettingsModelList)
						{
							var existingChangeSettingsModelItem = request.ChangeSettingsModelList.Where(b => b.BackupName == changeSettingModelItem.BackupName
																						&& b.IsManuallyOptionChecked == changeSettingModelItem.IsManuallyOptionChecked
																						&& b.IsPeriodicOptionChecked == changeSettingModelItem.IsPeriodicOptionChecked)
																				 .FirstOrDefault();

							if (existingChangeSettingsModelItem == null)
							{
								request.ChangeSettingsModelList.Add(existingChangeSettingsModelItem);
								WriteJsonRequestModel(request);
							}
							else
							{
								// replace the model in the request by updating it with the new values
								MessageBox.Show(Constants.TaskSchedulerAlreadyExist, Constants.InformativeMessage, MessageBoxButtons.OK);
							}
						}
					}
					else
					{
						request.ChangeSettingsModelList = changeSettingsModelList;
						WriteJsonRequestModel(request);
					}
				}
			}
		}

		public void SavePeriodicBackupInfo(List<PeriodicBackupModel> periodicBackupModelList, string taskName)
		{
			if (periodicBackupModelList != null)
			{
				CheckIfJsonFileExist();

				var jsonText = File.ReadAllText(_persistancePath);
				var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
				if (request == null)
				{
					request = new JsonRequestModel();
					request.PeriodicBackupModelList = periodicBackupModelList;
					WriteJsonRequestModel(request);
				}
				else
				{
					if (request.PeriodicBackupModelList != null)
					{
						foreach (var periodicBackupModelItem in periodicBackupModelList)
						{
							var existingperiodicBackupModelItem = request.PeriodicBackupModelList.Where(b => b.BackupName == periodicBackupModelItem.BackupName)
																								 .FirstOrDefault();

							if (existingperiodicBackupModelItem == null)
							{
								request.PeriodicBackupModelList.Add(existingperiodicBackupModelItem);
								WriteJsonRequestModel(request);
							}
							else
							{
								// replace the model in the request by updating it with the new values
								MessageBox.Show(Constants.TaskSchedulerAlreadyExist, Constants.InformativeMessage, MessageBoxButtons.OK);
							}
						}
					}
					else
					{
						request.PeriodicBackupModelList = periodicBackupModelList;
						WriteJsonRequestModel(request);
					}
				}
			}
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

		public void WriteJsonRequestModel(JsonRequestModel request)
		{
			var json = JsonConvert.SerializeObject(request);
			File.WriteAllText(_persistancePath, json);
		}
	}
}