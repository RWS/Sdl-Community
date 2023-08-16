using System.Collections.Generic;
using Sdl.Community.DeepLMTProvider.Interface;
using Sdl.Community.DeepLMTProvider.UI;

namespace Sdl.Community.DeepLMTProvider.Service
{
	public class GlossaryBrowserService : IGlossaryBrowserService
	{
		public bool Browse(List<string> supportedLanguages, out string path, out string sourceLanguage, out string targetLanguage)
		{
			path = default;
			sourceLanguage = default;
			targetLanguage = default;

			var browseGlossaryWindow = new BrowseGlossaryWindow(supportedLanguages);
			if (!(browseGlossaryWindow.ShowDialog() ?? false))
				return false;

			path = browseGlossaryWindow.Path;
			sourceLanguage =
				browseGlossaryWindow.SupportedLanguages[browseGlossaryWindow.SelectedIndexSourceLanguage];
			targetLanguage =
				browseGlossaryWindow.SupportedLanguages[browseGlossaryWindow.SelectedIndexTargetLanguage];

			return true;
		}
	}
}