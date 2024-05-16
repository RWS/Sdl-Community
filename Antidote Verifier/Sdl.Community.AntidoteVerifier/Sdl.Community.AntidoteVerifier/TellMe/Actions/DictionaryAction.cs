using Sdl.Community.AntidoteVerifier.TellMe.WarningWindow;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    public class DictionaryAction : AbstractTellMeAction
    {
        public DictionaryAction()
        {
            Name = $"{PluginResources.Plugin_Name} Dictionary";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

        public override Icon Icon => PluginResources.dictionary;

        public override bool IsAvailable => true;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<AntidoteDictionaryAction>();
    }
}