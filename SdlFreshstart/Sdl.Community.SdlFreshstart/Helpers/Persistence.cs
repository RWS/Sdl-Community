using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NLog;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class Persistence
	{
		private readonly string _studioPersistancePath;
		private readonly string _multiTermPersistancePath;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public Persistence()
		{
			try
			{
				_studioPersistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "TradosFreshstart", "Settings", "studioSettings.json");
				_multiTermPersistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Trados AppStore", "TradosFreshstart", "Settings", "multiTermSettings.json");
				if (!File.Exists(_studioPersistancePath))
				{
					var studioDirectory = Path.GetDirectoryName(_studioPersistancePath);
					if (studioDirectory != null && !Directory.Exists(studioDirectory))
					{
						Directory.CreateDirectory(studioDirectory);
					}
				}
				if (!File.Exists(_multiTermPersistancePath))
				{
					var multiTermDirectory = Path.GetDirectoryName(_multiTermPersistancePath);
					if (multiTermDirectory != null && !Directory.Exists(multiTermDirectory))
					{
						Directory.CreateDirectory(multiTermDirectory);
					}
				}
			}
			catch(Exception ex)
			{
				_logger.Error($"{Constants.PersistenceConstructor} {ex.Message}\n {ex.StackTrace}");
			}
		}

		public void SaveSettings(List<LocationDetails> locations, bool studioSettings)
		{
			try
			{
				var json = JsonConvert.SerializeObject(locations);
				File.WriteAllText(studioSettings ? _studioPersistancePath : _multiTermPersistancePath, json);
			}
			catch(Exception ex)
			{
				_logger.Error($"{Constants.SaveSettings} {ex.Message}\n {ex.StackTrace}");
			}
		}

		public List<LocationDetails> Load(bool studioSettings)
		{
			try
			{
				var json = File.ReadAllText(studioSettings ? _studioPersistancePath : _multiTermPersistancePath);
				return JsonConvert.DeserializeObject<List<LocationDetails>>(json);
			}
			catch(Exception ex)
			{
				_logger.Error($"{Constants.Load} {ex.Message}\n {ex.StackTrace}");
			}
			return new List<LocationDetails>();
		}
	}
}