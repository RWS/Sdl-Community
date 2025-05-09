using Sdl.TellMe.ProviderApi;
using System.Diagnostics;
using System.Drawing;
using System.Media;
using TradosStudioQuickInfo.View;

namespace TradosStudioQuickInfo
{
    [TellMeProvider]
    public class QuickInfoTellMeProvider : ITellMeProvider
    {
       
        public string Name => string.Format(PluginResources.TellMe_Provider, PluginResources.Plugin_Name);

        public AbstractTellMeAction[] ProviderActions => new AbstractTellMeAction[]
        {
            new QuickInfoDocumentationAction
            {
                Keywords = new string[] { "quick", "info", "quickinfo", "documentation" }
            },
            new AppStoreForumAction
            {
                Keywords = new[] { "quick", "info", "quickinfo", "support", "forum" }
            },
            new QuickInfoSettingsAction
            {
                Keywords = new[] { "quick", "info", "quickinfo", "settings" }
            }
        };
    }

    public class QuickInfoDocumentationAction : AbstractTellMeAction
    {

        public QuickInfoDocumentationAction()
        {
            Name = string.Format("{0} Documentation", PluginResources.Plugin_Name);

        }
        public override bool IsAvailable => true;

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);


        public override Icon Icon => PluginResources.TellMe_Documentation;



        public override void Execute()
        {
            Process.Start("https://appstore.rws.com/Plugin/91?tab=documentation");
        }
    }

    public class QuickInfoSettingsAction : AbstractTellMeAction
    {
        public QuickInfoSettingsAction()
        {
            Name = string.Format("{0} Settings", PluginResources.Plugin_Name);
        }

        private static void ShowDialog()
        {
            SystemSounds.Beep.Play();
            new WarningSettingsView("https://appstore.rws.com/Plugin/91?tab=documentation").ShowDialog();
        }

        public override void Execute()
        {
            ShowDialog();
        }

        public override bool IsAvailable => true;
        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.TellMe_Settings;
    }

    public class AppStoreForumAction : AbstractTellMeAction
    {
        public override bool IsAvailable => true;

        public override string Category => string.Format(PluginResources.TellMe_Provider_Results, PluginResources.Plugin_Name);

        public override Icon Icon => PluginResources.Question;

        public AppStoreForumAction()
        {
            Name = "RWS Community AppStore Forum";
        }

        public override void Execute()
        {
            Process.Start("https://community.rws.com/product-groups/trados-portfolio/rws-appstore/f/rws-appstore");
        }
    }
}
