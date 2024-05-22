using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.RecordSourceTU.TellMe.Actions
{
    public class HelpAction : AbstractTellMeAction
    {
        public HelpAction()
        {
            Name = $"{PluginResources.Plugin_Name} Documentation";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.Question;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/36?tab=documentation");
        }
    }
}