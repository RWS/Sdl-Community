using System.Drawing;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
    internal class DocumentationAction() : TellMeAction(_actionName, _icon, _helpKeywords, _isAvailable, url: _url)
    {
        private static readonly string[] _helpKeywords = ["help", "guide"];
        private static readonly string _actionName = TellMeConstants.TellMe_Documentation_Name;
        private static readonly string _url = TellMeConstants.TellMe_Documentation_Url;
        private static readonly Icon _icon = PluginResources.TellMe_Documentation;
        private static readonly bool _isAvailable = true;
    }
}