using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.XmlReader.TellMe.Actions
{
    public class XmlReaderAction : AbstractTellMeAction
    {
        public XmlReaderAction()
        {
            Name = $"{PluginResources.Plugin_Name}";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.logo;
        public override bool IsAvailable => true;

        public override void Execute() => SdlTradosStudio.Application.ExecuteAction<GenerateXmlReaderAction>();
    }
}