using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.RecordSourceTU.TellMe.Actions
{
    public class SourceCodeAction : AbstractTellMeAction
    {
        public SourceCodeAction()
        {
            Name = $"{PluginResources.Plugin_Name} Source Code";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.SourceCode;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://github.com/RWS/Sdl-Community/tree/master/Record%20Source%20TU");
        }
    }
}