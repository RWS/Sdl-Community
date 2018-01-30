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

		public void SaveBackupFormInfo(List<BackupModel> backupModelList, bool isNewTask)
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
							var existingBackupModelItem = request.BackupModelList.Where(b => b.BackupName == backupModelItem.BackupName)
																				 .FirstOrDefault();

							if (existingBackupModelItem == null)
							{
								request.BackupModelList.Add(backupModelItem);
								WriteJsonRequestModel(request);
							}
							else
							{
								if (!isNewTask)
								{
									// update model with the updated values
									request.BackupModelList.Remove(existingBackupModelItem);
									request.BackupModelList.Add(backupModelItem);
									WriteJsonRequestModel(request);
								}
								else
								{
									// task is new and informative message is displayed
									MessageBox.Show(Constants.TaskSchedulerAlreadyExist, Constants.InformativeMessage, MessageBoxButtons.OK);
								}
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
							var existingBackupItem = request.BackupDetailsModelList.Where(b => b.BackupName.Equals(taskName))
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
					if (request.ChangeSettingsModelList != null && request.ChangeSettingsModelList.Count > 0 && request.ChangeSettingsModelList[0] != null)
					{
						foreach (var changeSettingModelItem in changeSettingsModelList)
						{
							var existingChangeSettingsModelItem = request.ChangeSettingsModelList.Where(b => b.BackupName == changeSettingModelItem.BackupName)
																								.FirstOrDefault();

							if (existingChangeSettingsModelItem == null)
							{
								request.ChangeSettingsModelList.Add(changeSettingModelItem);
								WriteJsonRequestModel(request);
							}
							else
							{
								// Update json request model with the updated values
								request.ChangeSettingsModelList.Remove(existingChangeSettingsModelItem);
								request.ChangeSettingsModelList.Add(changeSettingModelItem);
								WriteJsonRequestModel(request);
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
					if (request.PeriodicBackupModelList != null && request.PeriodicBackupModelList.Count > 0 && request.PeriodicBackupModelList[0] != null) 
					{
						foreach (var periodicBackupModelItem in periodicBackupModelList)
						{
							var existingperiodicBackupModelItem = request.PeriodicBackupModelList.Where(b => b.BackupName == periodicBackupModelItem.BackupName)
																								 .FirstOrDefault();

							if (existingperiodicBackupModelItem == null)
							{
								request.PeriodicBackupModelList.Add(periodicBackupModelItem);
								WriteJsonRequestModel(request);
							}
							else
							{
								// Update json request model with the updated values
								request.PeriodicBackupModelList.Remove(existingperiodicBackupModelItem);
								request.PeriodicBackupModelList.Add(periodicBackupModelItem);
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

		public void RemoveDataFromJson(string backupName)
		{
			if (File.Exists(_persistancePath))
			{
				var jsonText = File.ReadAllText(_persistancePath);
				var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

				if (request != null
				&& request.BackupDetailsModelList != null
				&& request.BackupModelList != null
				&& request.ChangeSettingsModelList != null
				&& request.PeriodicBackupModelList != null)
				{
					var backupDetailsModelItem = request.BackupDetailsModelList.Where(b => b.BackupName.Equals(backupName)).FirstOrDefault();
					var backupModelItem = request.BackupModelList.Where(b => b.BackupName.Equals(backupName)).FirstOrDefault();
					var changeSettingsModelItem = request.ChangeSettingsModelList.Where(c => c.BackupName.Equals(backupName)).FirstOrDefault();
					var periodicBackupModelItem = request.PeriodicBackupModelList.Where(p => p.BackupName.Equals(backupName)).FirstOrDefault();

					if (backupDetailsModelItem != null)
					{
						request.BackupDetailsModelList.Remove(backupDetailsModelItem);
					}
					if (backupModelItem != null)
					{
						request.BackupModelList.Remove(backupModelItem);
					}
					if (changeSettingsModelItem != null)
					{
						request.ChangeSettingsModelList.Remove(changeSettingsModelItem);
					}
					if (periodicBackupModelItem != null)
					{
						request.PeriodicBackupModelList.Remove(periodicBackupModelItem);
					}
					WriteJsonRequestModel(request);
				}
			}
		}

		public void SaveBackupModel(BackupModel backupModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			var item = request.BackupModelList.Where(p => p.BackupName.Equals(backupModel.BackupName)).FirstOrDefault();
			if (item != null)
			{
				request.BackupModelList.Remove(item);
			}

			request.BackupModelList.Add(backupModel);
			WriteJsonRequestModel(request);
		}

		public void SaveDetailModel(BackupDetailsModel detailsModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			var item = request.BackupDetailsModelList.Where(p => p.BackupName.Equals(detailsModel.BackupName)).FirstOrDefault();
			if (item != null)
			{
				request.BackupDetailsModelList.Remove(item);
			}

			request.BackupDetailsModelList.Add(detailsModel);
			WriteJsonRequestModel(request);
		}

		public void SavePeriodicModel(PeriodicBackupModel periodicModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			var item = request.PeriodicBackupModelList.Where(p => p.BackupName.Equals(periodicModel.BackupName)).FirstOrDefault();
			if (item != null)
			{
				request.PeriodicBackupModelList.Remove(item);
			}

			request.PeriodicBackupModelList.Add(periodicModel);
			WriteJsonRequestModel(request);
		}

		public void SaveChangeModel(ChangeSettingsModel changeModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			var item = request.ChangeSettingsModelList.Where(b=>b.BackupName.Equals(changeModel.BackupName)).FirstOrDefault();
			if(item != null)
			{
				request.ChangeSettingsModelList.Remove(item);
			}
			request.ChangeSettingsModelList.Add(changeModel);
			WriteJsonRequestModel(request);
		}

		public void UpdateJsonRequest()
		{

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
	}
}