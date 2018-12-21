using System;

namespace Sdl.Community.AmazonTranslateTradosPlugin.Helpers
{
	public static class Constants
	{
		public readonly static string JsonFilePath = string.Format(@"C:\Users\{0}\AppData\Roaming\SDL Community\AmazonTranslationProvider", Environment.UserName);
		public readonly static string JsonFileName = "AmazonProviderSettings.json";
	}
}