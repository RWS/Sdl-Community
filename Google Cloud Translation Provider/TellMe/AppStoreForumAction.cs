using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudTranslationProvider.TellMe
{
    class AppStoreForumAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "community", "support", "documentation" };
        private static readonly bool _isAvailable = true;
        public AppStoreForumAction() : base("RWS Community AppStore Forum", 
            PluginResources.TellMeAppStoreForum, 
            _helpKeywords, 
            _isAvailable, 
            url: "https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f") { }
    }
}
