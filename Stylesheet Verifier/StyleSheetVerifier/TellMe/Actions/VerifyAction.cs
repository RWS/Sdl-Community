using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.StyleSheetVerifier.TellMe.Actions
{
    public class VerifyAction : AbstractTellMeAction
    {
        public VerifyAction() => Name = $"{PluginResources.Plugin_Name}";

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.icon;
        public override bool IsAvailable => true;

        public override void Execute() =>
            SdlTradosStudio.Application.ExecuteAction<StyleSheetVerifierAction>();
    }
}