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
							var existingBackupModelItem = request.BackupModelList.FirstOrDefault(b => b.BackupName == backupModelItem.BackupName);

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
				foreach (var item in backupDetailsModelList)
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
							var existingBackupItem = request.BackupDetailsModelList.FirstOrDefault(b => b.BackupName.Equals(taskName)
																						  && b.BackupAction.Equals(backupItem.BackupAction)
																						  && b.BackupPattern.Equals(backupItem.BackupPattern)
																						  && b.BackupType.Equals(backupItem.BackupType));															
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

		public void DeleteDetailsFromInfo(List<BackupDetailsModel> removedBackupDetailsList, string taskName)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (request.BackupDetailsModelList != null)
			{
				foreach (var item in removedBackupDetailsList)
				{
					var requestItem = request.BackupDetailsModelList.FirstOrDefault(r => r.BackupAction == item.BackupAction
					                                                                     && r.BackupType == item.BackupType 
					                                                                     && r.BackupPattern == item.BackupPattern
					                                                                     && r.BackupName.Equals(taskName));

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
							var existingChangeSettingsModelItem = request.ChangeSettingsModelList.FirstOrDefault(b => b.BackupName == changeSettingModelItem.BackupName);

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
							var existingperiodicBackupModelItem = request.PeriodicBackupModelList.FirstOrDefault(b => b.BackupName == periodicBackupModelItem.BackupName);

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
								WriteJsonRequestModel(request);
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
				
				if (request != null)
				{
					if (request.BackupDetailsModelList != null)
					{
						foreach (var backupDetailsModel in request.BackupDetailsModelList)
						{
							if (backupDetailsModel.BackupName.Equals(backupName))
							{
								request.BackupDetailsModelList.Remove(backupDetailsModel);
								break;
							}
						}
					}
					if (request.BackupModelList != null)
					{
						foreach (var backupModel in request.BackupModelList)
						{
							if (backupModel.BackupName.Equals(backupName))
							{
								request.BackupModelList.Remove(backupModel);
								break;
							}
						}
					}
					if (request.ChangeSettingsModelList != null)
					{
						foreach (var changeSettingModel in request.ChangeSettingsModelList)
						{
							if (changeSettingModel.BackupName.Equals(backupName))
							{
								request.ChangeSettingsModelList.Remove(changeSettingModel);
								break;
							}
						}
					}
					if (request.PeriodicBackupModelList != null)
					{
						foreach (var periodicModel in request.PeriodicBackupModelList)
						{
							if (periodicModel.BackupName.Equals(backupName))
							{
								request.PeriodicBackupModelList.Remove(periodicModel);
								break;
							}
						}
					}
					WriteJsonRequestModel(request);
				}				
			}
		}

		public void SaveBackupModel(BackupModel backupModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			if (backupModel != null && request.BackupModelList != null)
			{
				foreach (var item in request.BackupModelList)
				{
					if (item.BackupName.Equals(backupModel.BackupName))
					{
						request.BackupModelList.Remove(item);
						break;
					}
				}			
				request.BackupModelList.Add(backupModel);
				WriteJsonRequestModel(request);
			}
		}

		public void SaveDetailModel(BackupDetailsModel detailsModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);
			if (detailsModel != null && request.BackupDetailsModelList != null)
			{
				foreach (var item in request.BackupDetailsModelList)
				{
					if (item.BackupName.Equals(detailsModel.BackupName))
					{
						request.BackupDetailsModelList.Remove(item);
						break;
					}
				}
				request.BackupDetailsModelList.Add(detailsModel);
				WriteJsonRequestModel(request);
			}
		}

		public void SavePeriodicModel(PeriodicBackupModel periodicModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			if (periodicModel != null && request.PeriodicBackupModelList != null)
			{
				foreach (var item in request.PeriodicBackupModelList)
				{
					if (item.BackupName.Equals(periodicModel.BackupName))
					{
						request.PeriodicBackupModelList.Remove(item);
						break;
					}
				}				
				request.PeriodicBackupModelList.Add(periodicModel);
				WriteJsonRequestModel(request);
			}
		}

		public void SaveChangeModel(ChangeSettingsModel changeModel)
		{
			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			if (changeModel != null && request.ChangeSettingsModelList != null)
			{
				foreach (var item in request.ChangeSettingsModelList)
				{
					if (item.BackupName.Equals(changeModel.BackupName))
					{
						request.ChangeSettingsModelList.Remove(item);
						break;
					}
				}
				request.ChangeSettingsModelList.Add(changeModel);
				WriteJsonRequestModel(request);
			}
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