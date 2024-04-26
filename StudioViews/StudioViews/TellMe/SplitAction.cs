using Sdl.Community.StudioViews.Actions;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Drawing;

namespace Sdl.Community.StudioViews.TellMe
{
    public class SplitAction : AbstractTellMeAction
    {
        public SplitAction()
        {
            Name = $"{PluginResources.Plugin_Name} Split";
        }

        public override string Category => string.Format(PluginResources.TellMe_String_Results, PluginResources.Plugin_Name);
        public override Icon Icon => PluginResources.ForumIcon;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            var splitAction = SdlTradosStudio.Application.GetAction<SpitSelectedFilesAction>();
            splitAction.Run();
        }
    }
}