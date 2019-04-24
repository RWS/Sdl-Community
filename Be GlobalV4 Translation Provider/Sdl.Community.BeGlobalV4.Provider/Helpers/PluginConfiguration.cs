using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace Sdl.Community.BeGlobalV4.Provider.Helpers
{
	public class PluginConfiguration
	{
		public string LogLevel { get; set; }
		
		[JsonIgnore]
		public static PluginConfiguration CurrentInstance { get; set; }

		[JsonIgnore]
		public string Directory { get; set; }

		static PluginConfiguration()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var assemblyName = assembly.GetName().Name;

			var pluginDataDirectory = Environment.ExpandEnvironmentVariables($@"%ProgramData%/SDL/SDL Trados Studio/{assemblyName}");
			CurrentInstance = ParseConfiguration(pluginDataDirectory);
			if (CurrentInstance == null)
			{
				CurrentInstance = new PluginConfiguration(pluginDataDirectory);
			}
		}

		public PluginConfiguration(string directory)
		{
			Directory = directory;
		}

		public static PluginConfiguration ParseConfiguration(string directory)
		{
			var file = Path.Combine(directory, "config.json");
			if (!File.Exists(file))
			{
				return null;
			}

			using (var r = new StreamReader(file))
			{
				var config = JsonConvert.DeserializeObject<PluginConfiguration>(r.ReadToEnd());
				if (config == null)
				{
					return null;
				}
				config.Directory = directory;
				return config;
			}
		}

		public void SaveToFile()
		{
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore
			};
			var configString = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
			File.WriteAllText(Path.Combine(Directory, "config.json"), configString);
		}
	}
}
