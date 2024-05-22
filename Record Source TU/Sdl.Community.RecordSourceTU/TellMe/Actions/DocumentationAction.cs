using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.RecordSourceTU.TellMe.Actions
{
    public class DocumentationAction : AbstractTellMeAction
    {
        public DocumentationAction()
        {
            Name = "RWS Community AppStore forum";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.TellmeDocumentation;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
        }
    }
}