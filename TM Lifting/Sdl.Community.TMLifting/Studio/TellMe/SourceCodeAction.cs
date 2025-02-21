using System.Drawing;

namespace Sdl.Community.TMLifting.Studio.TellMe
{
    internal class SourceCodeAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "source", "code", "github" };
        private static readonly string _actionName = TellMeConstants.TellMe_SourceCode_Name;
        private static readonly string _url = TellMeConstants.TellMe_SourceCode_Url;
        private static readonly Icon _icon = TellMeResources.TellMe_SourceCode;
        private static readonly bool _isAvailable = true;

        public SourceCodeAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
    }
}