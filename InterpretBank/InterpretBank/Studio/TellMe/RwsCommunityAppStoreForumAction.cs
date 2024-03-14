using System.Diagnostics;
using System.Drawing;

namespace InterpretBank.Studio.TellMe
{
    public class RwsCommunityAppStoreForumAction : InterpretBankTellMeAction
    {
        public RwsCommunityAppStoreForumAction()
        {
            Name = "RWS Community AppStore Forum";
            Keywords = ["interpret", "bank", "community", "forum", "appstore", "rws", "interpretbank"];
        }

        public override Icon Icon => PluginResources.Question;

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f/rws-appstore");
        }
    }
}