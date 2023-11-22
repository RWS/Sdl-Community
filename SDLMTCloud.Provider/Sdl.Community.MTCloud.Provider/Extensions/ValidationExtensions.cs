using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.MTCloud.Provider.Extensions
{
	public static class ValidationExtensions
	{
		private static readonly List<string> _providerNames = new()
		{
			PluginResources.SDLMTCloud_Provider_Name,
			PluginResources.SDLMTCloud_Provider_OldName
		};

		public static bool IsLanguageWeaverOrigin(this string originSystem)
			=> originSystem is string originSystemString
			&& _providerNames.Any(x => originSystem.ToLower().Contains(x.ToLower()));
	}
}