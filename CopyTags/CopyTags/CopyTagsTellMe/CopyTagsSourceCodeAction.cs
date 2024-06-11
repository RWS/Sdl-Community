using Sdl.TellMe.ProviderApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SDLCopyTags.CopyTagsTellMe
{
    public class CommunitySourceCodeAction : AbstractTellMeAction
    {
        public override bool IsAvailable => true;

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

        public override Icon Icon => PluginResources.TellMe_SourceCode;

        public CommunitySourceCodeAction()
        {
            Name = string.Format("{0} Source Code", PluginResources.Plugin_Name);
        }

        public override void Execute()
        {
            Process.Start("https://github.com/RWS/Sdl-Community/tree/master/CopyTags");
        }
    }
}
