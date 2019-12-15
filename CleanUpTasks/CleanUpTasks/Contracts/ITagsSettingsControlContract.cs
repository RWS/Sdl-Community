using System.Windows.Forms;

namespace SDLCommunityCleanUpTasks.Contracts
{
	internal abstract class ITagsSettingsControlContract : ITagsSettingsControl
    {
        public CheckedListBox FormatTagList => default(CheckedListBox);

        public CheckedListBox PlaceholderTagList => default(CheckedListBox);

        public ICleanUpSourceSettings Settings { get; set; }

        public void InitializeUI()
        {
        }

        public void SaveSettings()
        {
        }

        public void SetSettings(ICleanUpSourceSettings settings)
        {
        }
    }
}