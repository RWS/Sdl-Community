using System.Drawing;

namespace MicrosoftTranslatorProvider.Studio.TellMe
{
    public class ThirdPartyRedirectAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "community", "support", "forum" };
        private static readonly string _actionName = Constants.TellMe_ThirdPartyRedirect_Name;
        private static readonly string _url = Constants.TellMe_ThirdPartyRedirect_Url;
        private static readonly Icon _icon = PluginResources.TellMe_ThirdParty;
        private static readonly bool _isAvailable = true;

        public ThirdPartyRedirectAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
    }
}
