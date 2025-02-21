using System.Drawing;

namespace Sdl.Community.TMLifting.Studio.TellMe
{
    internal class DocumentationAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "help", "guide", "documentation" };
        private static readonly string _actionName = TellMeConstants.TellMe_Documentation_Name;
        private static readonly string _url = TellMeConstants.TellMe_Documentation_Url;
        private static readonly Icon _icon = TellMeResources.TellMe_Documentation;
        private static readonly bool _isAvailable = true;

        public DocumentationAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
    }
}