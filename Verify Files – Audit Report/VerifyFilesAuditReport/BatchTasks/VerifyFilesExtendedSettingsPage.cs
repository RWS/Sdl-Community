using Sdl.Desktop.IntegrationApi;
using VerifyFilesAuditReport.BatchTasks.UI;
using VerifyFilesAuditReport.Components.SegmentMetadata;

namespace VerifyFilesAuditReport.BatchTasks
{
    public class VerifyFilesExtendedSettingsPage : DefaultSettingsPage<VerifyFilesExtendedSettingsView, VerifyFilesExtendedSettings>
    {
        private VerifyFilesExtendedSettingsView Control { get; set; }

        public override object GetControl()
        {
            Control = base.GetControl() as VerifyFilesExtendedSettingsView;
            Control.SetAvailableStatuses(SegmentStatuses.UiNames.Values);

            return Control;
        }

        public override void Save()
        {
            Control.GetSettings();
            base.Save();
        }
    }
}
