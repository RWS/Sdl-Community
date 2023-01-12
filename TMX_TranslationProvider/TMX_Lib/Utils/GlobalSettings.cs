using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;

namespace TMX_Lib.Utils
{
	public class GlobalSettings
	{
		private static readonly Logger log = NLog.LogManager.GetCurrentClassLogger();

		[JsonProperty]
		private List<TmxDbInfo> _localTmxDatabases = new List<TmxDbInfo>();

		private bool _needsSave;

		private GlobalSettings()
		{
		}

		public static GlobalSettings Inst { get; } = Load();

		private static string FileName => $"{Util.PluginDirectory}\\tmx_settings.txt";

		[JsonIgnore]
		public IReadOnlyList<TmxDbInfo> LocalTmxDatabases => _localTmxDatabases;

		public void AddTmxDatabase(TmxDbInfo dbInfo)
		{
			lock (this)
			{
				if (_localTmxDatabases.Any(db => db.DbName == dbInfo.DbName))
					return; // already have it
				_localTmxDatabases.Add(dbInfo);
				_needsSave = true;
			}
		}

		private static GlobalSettings Load()
		{
			if (File.Exists(Util.PluginDirectory))
			{
				var inst = JsonConvert.DeserializeObject<GlobalSettings>(File.ReadAllText(FileName));
				return inst;
			} else 
				return new GlobalSettings();
		}

		public void Save()
		{
			lock (this)
				if (!_needsSave)
					return;

			var str = JsonConvert.SerializeObject(this, Formatting.Indented);
			try
			{
				File.WriteAllText(FileName, str);
				lock (this)
					_needsSave = false;
			}
			catch(Exception e) {
				log.Error($"Can't save settings {e.Message}");
			}
		}
	}
}
