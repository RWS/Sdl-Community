using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Models;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public static class AgreementMethods
	{
		public static bool UserAgreed()
		{
			var accepted = false;
			var settingsFilePathInfo = new SdlDataProtectionSuite.SdlTmAnonymizer.Model.PathInfo();

			var settingsJson = File.ReadAllText(settingsFilePathInfo.SettingsFilePath);

			var jsonObject = JObject.Parse(settingsJson);

			accepted = (bool)jsonObject["Accepted"];

			return accepted;
		}
	}
}