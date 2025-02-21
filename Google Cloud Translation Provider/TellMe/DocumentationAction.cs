using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleCloudTranslationProvider.TellMe
{
    class DocumentationAction : TellMeAction
    {
        private static readonly string[] _helpKeywords = { "help", "guide" };
        private static readonly bool _isAvailable = true;

        public DocumentationAction() : base($"{PluginResources.Plugin_Name} Documentation",
            PluginResources.TellMeDocumentation, 
            _helpKeywords, 
            _isAvailable, 
            url: "https://appstore.rws.com/Plugin/39?tab=documentation") { }
    }
}
