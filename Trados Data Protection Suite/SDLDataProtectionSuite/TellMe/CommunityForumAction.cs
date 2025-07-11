using System.Drawing;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
    internal class CommunityForumAction() : TellMeAction(_actionName, _icon, _helpKeywords, _isAvailable, url: _url)
    {
        private static readonly string[] _helpKeywords = ["community", "support", "forum"];
        private static readonly string _actionName = TellMeConstants.TellMe_Forum_Name;
        private static readonly string _url = TellMeConstants.TellMe_Forum_Url;
        private static readonly Icon _icon = PluginResources.question;
        private static readonly bool _isAvailable = true;
    }
}