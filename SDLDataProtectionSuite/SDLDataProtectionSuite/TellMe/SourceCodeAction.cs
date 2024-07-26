namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	class SourceCodeAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "source code", "github" };
		private static readonly bool _isAvailable = true;
		public SourceCodeAction() : base($"{PluginResources.Plugin_Name} Source Code", PluginResources.TellMeSourceCode, _helpKeywords, _isAvailable, url: "https://github.com/RWS/Sdl-Community/tree/master/SDLDataProtectionSuite") { }
	}
}