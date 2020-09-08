using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using Sdl.Community.DeepLMTProvider.WPF;

namespace Sdl.Community.DeepLMTProvider
{
	public static class Helpers
	{
		private static readonly List<string> FormalityIncompatibleTargetLanguages = new List<string>
		{
			"ja",
			"es",
			"zh",
			"en",
			"en-gb",
			"en-us",
		};

		public static bool IsInvalidServerMessage { get; set; }

		public static bool AreLanguagesCompatibleWithFormalityParameter(List<CultureInfo> targetLanguages)
		{
			return targetLanguages.All(IsLanguageCompatible);
		}

		public static bool IsLanguageCompatible(CultureInfo targetLanguage)
		{
			var twoLetterIsoLanguage = targetLanguage.TwoLetterISOLanguageName.ToLowerInvariant();
			return !FormalityIncompatibleTargetLanguages.Contains(twoLetterIsoLanguage);
		}

		public static void DisplayServerMessage(HttpResponseMessage response)
		{
			if (response.StatusCode == HttpStatusCode.Forbidden)
			{
				MessageBox.Show(PluginResources.Forbidden_Message, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				IsInvalidServerMessage = true;
				return;
			}

			if (!response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.Forbidden)
			{
				MessageBox.Show(string.Format(PluginResources.ServerGeneralResponse_Message,response.StatusCode.ToString()), string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				IsInvalidServerMessage = true;
				return;
			}

			IsInvalidServerMessage = false;
		}
	}
}