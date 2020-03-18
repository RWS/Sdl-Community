using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Sdl.Community.MTCloud.Provider.Helpers
{
	public static class Utils
	{
		public static readonly Log Log = Log.Instance;

	
		public static List<string> SplitLanguagePair(string languagePair)
		{
			try
			{
				if (!string.IsNullOrEmpty(languagePair))
				{
					var splittedLanguagePair = languagePair.Split('-');
					var sourceLangPair = splittedLanguagePair?.Count() > 1 ? splittedLanguagePair[0].TrimStart().TrimEnd() : string.Empty;
					var targetLangPair = splittedLanguagePair?.Count() > 1 ? splittedLanguagePair[1].TrimStart().TrimEnd() : string.Empty;

					return new List<string> { sourceLangPair, targetLangPair };
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"{Constants.SplitLanguagePair}: {ex.Message}\n {ex.StackTrace}");
				throw new Exception(ex.Message);
			}
			return new List<string>();
		}

		public static void LogServerIPAddresses()
		{
			var ipAddresses = Dns.GetHostAddresses(Constants.HostAddress);
			foreach (var ipAddress in ipAddresses)
			{
				Log.Logger.Info($"{Constants.MTCloudServerIPMessage} {ipAddress}");
			}
		}
	}
}