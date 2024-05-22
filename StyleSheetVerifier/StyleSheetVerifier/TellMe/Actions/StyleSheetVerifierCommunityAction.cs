using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;

namespace Sdl.Community.StyleSheetVerifier.TellMe.Actions
{
    public class StyleSheetVerifierCommunityAction : AbstractTellMeAction
    {
        public StyleSheetVerifierCommunityAction()
        {
            Name = "RWS Community AppStore Forum";
        }

        public override string Category => "StyleSheet Verifier results";
        public override Icon Icon => PluginResources.Forum;
        public override bool IsAvailable => true;

        public override void Execute() =>
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f");
    }
}