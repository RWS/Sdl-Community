using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Model;
using Newtonsoft.Json;

namespace IATETerminologyProvider.Service
{
	public class PersistenceService
	{
		private readonly string _persistancePath;
		private List<ProviderSettings> _providerSettingList = new List<ProviderSettings>();
        public static readonly Log Log = Log.Instance;

		public PersistenceService()
		{
			_persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\IATETerminologyProvider\IATETerminology.json");
		}

		public void AddSettings(ProviderSettings providerSettings)
		{
			if (providerSettings == null)
			{
				throw new NullReferenceException("Provider settings cannot be null");
			}

			GetProviderSettingsList();

			var result = _providerSettingList.FirstOrDefault();

			if (result != null)
			{
				result.Domains = providerSettings.Domains;
				result.TermTypes = providerSettings.TermTypes;
			}
			else
			{
				_providerSettingList.Add(providerSettings);
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

		internal void WriteToFile()
		{
			try
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
			catch (Exception e)
			{
                Log.Logger.Error($"{e.Message}\n{e.StackTrace}");
			}
		}
	}
}