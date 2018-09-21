using System.IO;
using Newtonsoft.Json;
using Sdl.Community.projectAnonymizer.Models;

namespace Sdl.Community.projectAnonymizer.Helpers
{
	public static class AgreementMethods
	{
		public static bool UserAgreed()
		{
			if (File.Exists(Constants.AcceptFilePath))
			{
				var json = File.ReadAllText(Constants.AcceptFilePath);
				var accepted = JsonConvert.DeserializeObject<Agreement>(json);

				return accepted.Accept;
			}
			return false;
		}
	}
}