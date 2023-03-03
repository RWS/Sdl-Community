using System.Collections.Generic;

namespace Sdl.Community.MTCloud.Provider.Extensions
{
	public static class ValidationExtensions
	{
		private static readonly List<string> _providerNames = new()
		{
			PluginResources.SDLMTCloud_Provider_Name,
			PluginResources.SDLMTCloud_Provider_OldName,
			PluginResources.SDLMTCloud_Provider_OldName2
		};

		public static bool IsLanguageWeaverOrigin(this string originSystem)
			=> originSystem is string originSystemString
			&& (_providerNames.Contains(originSystemString) || originSystemString.ToLower().Contains(PluginResources.SDLMTCloud_Provider_Name.ToLower()));
	}
}