using Sdl.Community.TMBackup.Models;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

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
				foreach (var backupItem in request.BackupDetailsModelList)
				{
					if (!backupDetailsModelList.Contains(backupItem))
					{
						request.BackupDetailsModelList = backupDetailsModelList;
						var json = JsonConvert.SerializeObject(request);

						File.WriteAllText(_persistancePath, json);
					}
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