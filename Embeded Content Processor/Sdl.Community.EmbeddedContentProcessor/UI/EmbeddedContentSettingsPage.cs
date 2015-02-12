using Sdl.Community.EmbeddedContentProcessor.Settings;
using Sdl.FileTypeSupport.Framework.Core.Settings;

namespace Sdl.Community.EmbeddedContentProcessor.UI
{
    [FileTypeSettingsPage(Id = "CommunityEmbedddedContentProcessor_Settings", Name = "Community Embedded Content Processor",
       Description = "Community Embedded Content Processor", HelpTopic = "Embedded_Regex_Content")]
    public class EmbeddedContentSettingsPage : AbstractFileTypeSettingsPage<EmbeddedContentSettingsControl, EmbeddedContentRegexSettings>
    {
        public override void ResetToDefaults()
        {
            base.ResetToDefaults();
            Control.UpdateUi();
        }

        public override void Save()
        {
            Control.UpdateSettings();
            base.Save();
        }

        public override void Refresh()
        {
            base.Refresh();
            Control.UpdateUi();
        }
    }
}
