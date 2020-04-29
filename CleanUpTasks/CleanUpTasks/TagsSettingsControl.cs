using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using SDLCommunityCleanUpTasks;

namespace Sdl.Community.CleanUpTasks
{
	public partial class TagsSettingsControl : UserControl, ITagsSettingsControl
    {
        private ITagSettingsPresenter presenter = null;

        public TagsSettingsControl()
        {
            InitializeComponent();
        }

        public CheckedListBox FormatTagList { get { return fmtCheckedListBox; } }

        public CheckedListBox PlaceholderTagList { get { return phCheckedListBox; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICleanUpSourceSettings Settings { get; set; }

        public void InitializeUI()
        {
            presenter.Initialize();
        }

        public void SaveSettings()
        {
            presenter.SaveSettings();
        }

        public void SetPresenter(ITagSettingsPresenter presenter)
        {
            this.presenter = presenter;
        }

        public void SetSettings(ICleanUpSourceSettings settings)
        {
            Settings = settings;

            SettingsBinder.DataBindSetting<bool>(tagCheckGroupBox, "Checked", settings.Settings,
                                                   nameof(settings.UseTagCleaner));
        }
    }
}