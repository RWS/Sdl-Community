namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	class DocumentationAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "help", "guide" };
		private static readonly bool _isAvailable = true;

		public DocumentationAction() : base($"{PluginResources.Plugin_Name} Documentation", PluginResources.TellMeDoc, _helpKeywords, _isAvailable, url: "https://appstore.rws.com/Plugin/39?tab=documentation") { }
	}
}