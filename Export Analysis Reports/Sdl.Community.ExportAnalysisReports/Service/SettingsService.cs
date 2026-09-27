using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Service
{
	public class SettingsService
	{
		public SettingsService(PathInfo	pathInfo)
		{
			PathInfo = pathInfo;
		}

		public PathInfo PathInfo { get; }

		public JsonSettings GetSettings()
		{
			var settings = new JsonSettings();

			if (!File.Exists(PathInfo.SettingsFilePath))
			{
				return settings;
			}

			using (var r = new StreamReader(PathInfo.SettingsFilePath))
			{
				var json = r.ReadToEnd();
				settings = JsonConvert.DeserializeObject<JsonSettings>(json);
			}

			return settings;
		}


		public void SaveSettings(JsonSettings settings)
		{
			using (var tw = new StreamWriter(PathInfo.SettingsFilePath, false))
			{
				tw.WriteLine(JsonConvert.SerializeObject(settings));
				tw.Close();
			}
		}
	}
}
