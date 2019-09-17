﻿using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace ETSTranslationProvider
{
	public struct Connection
    {
        public Connection(string host, int port)
        {
            Host = host;
            Port = port;
        }

        public string Host { get; set; }
        public int Port { get; set; }

    }

    public class PluginConfiguration
    {
        public string LogLevel { get; set; }

        public Connection? DefaultConnection { get; set; }

        [JsonIgnore]
        public static PluginConfiguration CurrentInstance { get; set; }

        [JsonIgnore]
        public string Directory { get; set; }

		static PluginConfiguration()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var assemblyName = assembly.GetName().Name;

			var pluginDataDirectory = Environment.ExpandEnvironmentVariables($"%AppData%\\SDL Community\\{assemblyName}");
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

            using (StreamReader r = new StreamReader(file))
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
            var settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            var configString = JsonConvert.SerializeObject(this, Formatting.Indented, settings);
			System.IO.Directory.CreateDirectory(Directory);
			File.WriteAllText(Path.Combine(Directory, "config.json"), configString);
        }
    }
}