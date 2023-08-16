using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface IGlossaryBrowserService
	{
		public bool Browse(List<string> supportedLanguages, out string path, out string sourceLanguage,
			out string targetLanguage);
	}
}