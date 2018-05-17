using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.TmAnonymizer.Model;

namespace Sdl.Community.TmAnonymizer.Helpers
{
	public static class AgreementMethods
	{
		public static bool UserAgreed()
		{
			if (File.Exists(Constants.SettingsFilePath))
			{
				var json = File.ReadAllText(Constants.SettingsFilePath);
				var settings = JsonConvert.DeserializeObject<Settings>(json);
				if (settings != null)
				{
					return settings.Accepted;
				}
				
			}
			return false;
		}
	}
}
