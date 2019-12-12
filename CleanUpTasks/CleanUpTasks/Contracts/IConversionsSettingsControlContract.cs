using System.Windows.Forms;

namespace SDLCommunityCleanUpTasks.Contracts
{
    internal abstract class IConversionsSettingsControlContract : IConversionsSettingsControl
    {
        public Button Add => default(Button);

	    public Button Down => default(Button);

	    public Button Edit => default(Button);

	    CheckedListBox IConversionsSettingsControl.FileList => default(CheckedListBox);

	    public Button New => default(Button);

	    public Button Remove => default(Button);

	    public ICleanUpConversionSettings Settings { get; set; }

		public Button Up => default(Button);

	    public CheckBox ApplyToNonTranslatables => default(CheckBox);

	    public void InitializeUI()
        {
        }

        public void SaveSettings()
        {
        }

        public void SetPresenter(IConversionsSettingsPresenter presenter)
        {
        }

        public void SetSettings(ICleanUpConversionSettings settings, BatchTaskMode taskMode)
        {
        }
    }
}