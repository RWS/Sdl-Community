using Sdl.Community.Toolkit.Core.Services;

namespace Sdl.Community.InvoiceAndQuotes.Helpers
{
	public class Utils
	{
		public string GetStudioVersion()
		{
			var studioService = new StudioVersionService();
			var shortVersion = studioService.GetStudioVersion()?.ShortVersion;
			return $"Studio {shortVersion}";
		}
	}
}