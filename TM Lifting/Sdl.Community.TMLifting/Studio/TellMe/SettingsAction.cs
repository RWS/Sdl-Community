using System.Drawing;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.TMLifting.Studio.TellMe
{
    internal class SettingsAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "project", "settings", "controller" };
        private static readonly string _actionName = TellMeConstants.TellMe_Settings_Name;
        private static readonly Icon _icon = TellMeResources.TellMe_Settings;
        private static readonly bool _isAvailable = true;

        public SettingsAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, customAction: Action) { }

        private static void Action()
        {
            SdlTradosStudio.Application.GetController<TMLiftingRibbon>().Activate();
        }
    }
}