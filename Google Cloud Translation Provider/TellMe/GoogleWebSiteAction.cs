using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudTranslationProvider.TellMe
{
    class GoogleWebSiteAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "website", "web", "site" };
        private static readonly bool _isAvailable = true;
        public GoogleWebSiteAction() : base("Google Cloud Translate official website", 
            PluginResources.GoogleCloud,
            _helpKeywords, _isAvailable,
            url: "https://cloud.google.com/translate") { }
    }
}
