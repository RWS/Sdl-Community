using System;
using System.Drawing;
using LanguageWeaverProvider.Model.Options;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Helpers
{
	public static class ImagePathUtility
	{
		public static (Icon IcoFile, Bitmap PngFile) GetImagePaths(string translationProviderState)
		{
			return translationProviderState switch
			{
				Constants.CloudFullScheme => (PluginResources.lwLogo_Cloud_Icon, PluginResources.lwLogo_Cloud16),
				Constants.EdgeFullScheme => (PluginResources.lwLogo_Edge_Icon, PluginResources.lwLogo_Edge16),
				_ => throw new ArgumentException("Unsupported PluginVersion value"),
			};
		}
	}
}