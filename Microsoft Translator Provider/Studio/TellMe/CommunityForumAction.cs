using System.Drawing;

namespace MicrosoftTranslatorProvider.Studio.TellMe
{
    class CommunityForumAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "azure", "support", "documentation" };
        private static readonly string _actionName = Constants.TellMe_Forum_Name;
        private static readonly string _url = Constants.TellMe_Forum_Url;
        private static readonly Icon _icon = PluginResources.TellMe_CommunityForum;
        private static readonly bool _isAvailable = true;

        public CommunityForumAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
    }
}