using System.Drawing;

namespace GoogleCloudTranslationProvider.Studio.TellMe
{
	class CommunityForumAction : BaseTellMeAction
	{
		private static readonly string[] _helpKeywords = { "community", "support", "documentation", "forum" };
		private static readonly string _actionName = "RWS Community AppStore Forum";
		private static readonly string _url = "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f";
		private static readonly bool _isAvailable = true;
		private static readonly Icon _icon = PluginResources.ForumIcon;

		public CommunityForumAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, _url) { }
	}
}