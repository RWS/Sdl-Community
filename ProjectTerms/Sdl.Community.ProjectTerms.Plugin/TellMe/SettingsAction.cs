using Sdl.Community.ProjectTerms.Plugin.ProjectTermsAction;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Sdl.Community.ProjectTerms.Plugin.TellMe
{
    public class SettingsAction : AbstractTellMeAction
    {
        public SettingsAction()
        {
            Name = $"{PluginResources.Plugin_Name} Settings";
        }

        public override string Category => $"{PluginResources.Plugin_Name} results";
        public override Icon Icon => PluginResources.TellMe_Settings;
        public override bool IsAvailable => true;

        public override void Execute()
        {
            SdlTradosStudio.Application.ExecuteAction<ProjectTermsFilesControllerAction>();
        }
    }
}
