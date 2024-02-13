using System.Drawing;

namespace GoogleCloudTranslationProvider.Studio.TellMe
{
	class DocumentationAction : BaseTellMeAction
	{
		private static readonly string[] _helpKeywords = { "help", "guide" };
		private static readonly string _actionName = $"{PluginResources.Plugin_Name} Documentation";
		private static readonly string _url = "https://appstore.rws.com/Plugin/174?tab=documentation";
		private static readonly bool _isAvailable = true;
		private static readonly Icon _icon = PluginResources.documentation;


		public DocumentationAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
	}
}