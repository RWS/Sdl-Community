namespace GoogleCloudTranslationProvider.Studio.TellMe
{
	class SourceCodeAction : BaseTellMeAction
	{
		private static readonly string[] _helpKeywords = { "source code", "github" };
		private static readonly string _actionName = $"{PluginResources.Plugin_Name} Source Code";
		private static readonly string _url = "https://github.com/RWS/Sdl-Community/tree/master/GoogleCloudTranslationProvider";
		private static readonly bool _isAvailable = true;

		public SourceCodeAction() : base(_actionName, PluginResources.sourceCode, _helpKeywords, _isAvailable, url: _url) { }
	}
}