using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IATETerminologyProvider.Model;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public class PersistenceService
	{
		#region Private Fields
		private readonly string _persistancePath;
		private List<ProviderSettings> _providerSettingList = new List<ProviderSettings>();
		#endregion

		#region Constructors
		public PersistenceService()
		{
			_persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\IATETerminologyProvider\IATETerminology.json");
		}
		#endregion

		#region Public Methods
		public void AddSettings(ProviderSettings providerSettings)
		{
			if (providerSettings == null)
			{
				throw new NullReferenceException("Provider settings cannot be null");
			}
			
			GetProviderSettingsList();

			if (providerSettings != null)
			{
				var result = _providerSettingList.FirstOrDefault();

				if (result != null)
				{
					result.Limit = providerSettings.Limit;					
				}
				else
				{
					_providerSettingList.Add(providerSettings);
				}
			}
			WriteToFile();
		}

		public void GetProviderSettingsList()
		{
			if (!File.Exists(_persistancePath)) return;
			var json = File.ReadAllText(_persistancePath);
			if (!string.IsNullOrEmpty(json))
			{
				_providerSettingList = JsonConvert.DeserializeObject<List<ProviderSettings>>(json);
			}
		}

		public ProviderSettings Load()
		{
			GetProviderSettingsList();
			var providerSettings = _providerSettingList.FirstOrDefault();

			return providerSettings;
		}
		#endregion

		#region Internal methods
		internal void WriteToFile()
		{
			if (!File.Exists(_persistancePath))
			{
				var directory = Path.GetDirectoryName(_persistancePath);
				if (directory != null && !Directory.Exists(directory))
				{
					Directory.CreateDirectory(directory);
				}
			}
			var json = JsonConvert.SerializeObject(_providerSettingList);
			File.WriteAllText(_persistancePath, json);
		}
		#endregion
	}
}