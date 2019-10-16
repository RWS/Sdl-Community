using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Sdl.Community.SdlFreshstart.Model;

namespace Sdl.Community.SdlFreshstart.Helpers
{
	public class Persistence
	{
		private readonly string _studioPersistancePath;
		private readonly string _multiTermPersistancePath;
		public static readonly Log Log = Log.Instance;

		public Persistence()
		{
			try
			{
				_studioPersistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup", "Settings", "studioSettings.json");
				_multiTermPersistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL", "StudioCleanup", "Settings", "multiTermSettings.json");
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
				Log.Logger.Error($"{Constants.PersistenceConstructor} {ex.Message}\n {ex.StackTrace}");
			}
		}

		public void SaveSettings(List<LocationDetails> locations,bool studioSettings)
		{
			try
			{
				var json = JsonConvert.SerializeObject(locations);
				if (studioSettings)
				{
					File.WriteAllText(_studioPersistancePath, json);
				}
				else
				{
					File.WriteAllText(_multiTermPersistancePath, json);
				}
			}
			catch(Exception ex)
			{
				Log.Logger.Error($"{Constants.SaveSettings} {ex.Message}\n {ex.StackTrace}");
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
				Log.Logger.Error($"{Constants.Load} {ex.Message}\n {ex.StackTrace}");
			}
			return new List<LocationDetails>();
		}
	}
}