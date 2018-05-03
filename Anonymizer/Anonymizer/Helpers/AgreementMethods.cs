using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
