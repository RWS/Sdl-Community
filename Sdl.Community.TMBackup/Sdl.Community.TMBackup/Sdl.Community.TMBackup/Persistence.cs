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
			}
		}

		public void SaveBackupFormInfo(BackupModel backupModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			request.BackupModel = backupModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}
		
		public void SaveDetailsFormInfo(List<BackupDetailsModel> backupDetailsModelList)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			request.BackupDetailsModelList = backupDetailsModelList;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}

		public void SaveChangeSettings(ChangeSettingsModel changeSettingsModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			request.ChangeSettingsModel = changeSettingsModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}

		public void SavePeriodicBackupInfo(PeriodicBackupModel periodicBackupModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			request.PeriodicBackupModel = periodicBackupModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}

		public void SaveRealTimeInfo(RealTimeBackupModel realTimeBackupModel)
		{
			CheckIfJsonFileExist();

			var jsonText = File.ReadAllText(_persistancePath);
			var request = JsonConvert.DeserializeObject<JsonRequestModel>(jsonText);

			request.RealTimeBackupModel = realTimeBackupModel;
			var json = JsonConvert.SerializeObject(request);

			File.WriteAllText(_persistancePath, json);
		}
	}
}