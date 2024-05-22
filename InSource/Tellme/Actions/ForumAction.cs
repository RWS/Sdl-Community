using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.InSource.Tellme.Actions
{
    public class ForumAction : AbstractTellMeAction
    {
        public ForumAction()
        {
            Name = "RWS Community AppStore forum";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";

        public override Icon Icon => PluginResources.Forum;

        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
        }
    }
}