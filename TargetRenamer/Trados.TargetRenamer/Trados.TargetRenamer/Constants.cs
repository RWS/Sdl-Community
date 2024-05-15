using System;
using System.IO;

namespace Trados.TargetRenamer
{
	public static class Constants
	{
		// Plugin Details
		public const string PluginName = "Trados Studio Target Renamer";
		public const string TeamName = "Trados AppStore";

		// Paths
		public static string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), TeamName, PluginName);

		// Tell Me
		public const string TellMe_Name = "Target Renamer";
		public static readonly string TellMe_Provider_Name = $"{TellMe_Name} Tell Me";
		public static readonly string TellMe_Forum_Name = $"RWS Community AppStore Forum ";
		public static readonly string TellMe_Forum_Url = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f";
		public static readonly string TellMe_Documentation_Name = $"{TellMe_Name} Documentation";
		public static readonly string TellMe_Documentation_Url = "https://appstore.rws.com/Plugin/73?tab=documentation";
		public static readonly string TellMe_SourceCode_Name = $"{TellMe_Name} Source Code";
		public static readonly string TellMe_SourceCode_Url = "https://github.com/RWS/Sdl-Community/tree/master/TargetRenamer";
		public static readonly string TellMe_Settings_Name = $"{TellMe_Name} Settings";
	}
}