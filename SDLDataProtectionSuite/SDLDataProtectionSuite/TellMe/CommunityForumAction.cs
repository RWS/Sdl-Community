namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
	class CommunityForumAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "community", "support", "documentation" };
		private static readonly bool _isAvailable = true;
		public CommunityForumAction() : base("RWS Community AppStore Forum", PluginResources.TellmeForum, _helpKeywords, _isAvailable, url: "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f") { }
	}
}