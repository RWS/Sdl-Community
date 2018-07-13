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

		public Persistence()
		{
			_studioPersistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL",
				"StudioCleanup", "Settings", "studioSettings.json");
			_multiTermPersistancePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL",
				"StudioCleanup", "Settings", "multiTermSettings.json");
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

		public void SaveSettings(List<LocationDetails> locations,bool studioSettings)
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

		public List<LocationDetails> Load(bool studioSettings)
		{
			var json = string.Empty;
			json = File.ReadAllText(studioSettings ? _studioPersistancePath : _multiTermPersistancePath);

			var settings = JsonConvert.DeserializeObject<List<LocationDetails>>(json);
			if (settings != null)
			{
				return settings; }
			return new List<LocationDetails>();
		}

	}
}
