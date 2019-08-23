using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sdl.Community.DtSearch4Studio.Provider.Helpers;
using Sdl.Community.DtSearch4Studio.Provider.Model;

namespace Sdl.Community.DtSearch4Studio.Provider.Service
{
	public class PersistenceService
	{
		#region Private Fields
		private readonly string _persistancePath;
		private List<ProviderSettings> _providerSettingList = new List<ProviderSettings>();
		#endregion

		#region Public properties
		public static readonly Log Log = Log.Instance;
		#endregion

		#region Constructors
		public PersistenceService()
		{
			_persistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\DtSearch4Studio\DtSearch4Studio.json");
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
					//To Do: set the index in result
					//result.Domains = providerSettings.Domains;
					//result.TermTypes = providerSettings.TermTypes;
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
				Log.Logger.Error($"{Constants.WriteToFile}: {e.Message}\n{e.StackTrace}");
			}
		}
		#endregion
	}
}
