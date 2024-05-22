using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.YourProductivity.TellMe.Actions
{
    public class CommunityAppStoreForumAction : AbstractTellMeAction
    {
        public CommunityAppStoreForumAction()
        {
            Name = "RWS Community AppStore Forum";
        }

        public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.Forum;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/");
        }
    }
}