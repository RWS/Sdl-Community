using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Studio;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;
using System.Media;

namespace Sdl.Community.SdlDataProtectionSuite.TellMe
{
    internal class TmAnonymizerViewAction() : TellMeAction(_actionName, _icon, _helpKeywords, _isAvailable,
        customAction: ShowTmAnonymizerView)
    {
        private static readonly string[] _helpKeywords = ["tm anonymizer tmanonymizer view"];
        private static readonly string _actionName = TellMeConstants.TmAnonymizerView;
        private static readonly Icon _icon = PluginResources.SDLTMAnonymizer;
        private static readonly bool _isAvailable = true;

        private static void ShowTmAnonymizerView() =>
            SdlTradosStudio.Application.GetController<SDLTMAnonymizerView>().Activate();
    }
}