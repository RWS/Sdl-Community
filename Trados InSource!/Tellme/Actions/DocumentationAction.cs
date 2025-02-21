using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.InSource.Tellme.Actions
{
    public class DocumentationAction : AbstractTellMeAction
    {
        public DocumentationAction()
        {
            Name = $"{PluginResources.Plugin_Name} Documentation";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.TellmeDocumentation;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/31?tab=documentation");
        }
    }
}