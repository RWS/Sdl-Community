using Sdl.Community.YourProductivity.UI;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.YourProductivity.TellMe.Actions
{
    public class ProductivityAction : AbstractTellMeAction
    {
        public ProductivityAction()
        {
            Name = $"{PluginResources.Plugin_Name}";
        }

        public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.score;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.ExecuteAction<ProductivityViewPartAction>();
        }
    }
}