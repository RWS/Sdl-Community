using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.AntidoteVerifier.TellMe.Actions
{
    public class DictionaryAction : AntidoteToolTellMeAction
    {
        public DictionaryAction() : base("Dictionary")
        {
        }

        public override Icon Icon => PluginResources.dictionary;

        protected override void ExecuteTool()
            => SdlTradosStudio.Application.ExecuteAction<AntidoteDictionaryAction>();
    }
}
