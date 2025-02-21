using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.InvoiceAndQuotes.Integration.TellMe.Actions
{
    public class DocumentationAction : AbstractTellMeAction
    {
        public DocumentationAction()
        {
            Name = $"{PluginResources.Plugin_Name} Documentation";
        }

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.TellmeDocumentation;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/55?tab=documentation");
        }
    }
}