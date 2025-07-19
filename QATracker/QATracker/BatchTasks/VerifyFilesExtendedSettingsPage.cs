using QATracker.BatchTasks.UI;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;

namespace QATracker.BatchTasks
{
    public class VerifyFilesExtendedSettingsPage : DefaultSettingsPage<VerifyFilesExtendedSettingsView, VerifyFilesExtendedSettings>
    {
        private VerifyFilesExtendedSettings _settings;
        private VerifyFilesExtendedSettingsView Control { get; set; }

        public override object GetControl()
        {
            _settings = ((ISettingsBundle)DataSource).GetSettingsGroup<VerifyFilesExtendedSettings>();
            Control = base.GetControl() as VerifyFilesExtendedSettingsView;
            return Control;
        }

        public override void Save()
        {
            _settings = Control.GetSettings();
            base.Save();
        }
    }
}