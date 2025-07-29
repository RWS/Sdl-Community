using CaptureQARuleState.BatchTasks.UI;
using CaptureQARuleState.Components.SegmentMetadata_Provider;
using CaptureQARuleState.Components.SegmentMetadata_Provider.Model;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Linq;

namespace CaptureQARuleState.BatchTasks
{
    public class VerifyFilesExtendedSettingsPage : DefaultSettingsPage<VerifyFilesExtendedSettingsView, VerifyFilesExtendedSettings>
    {
        private VerifyFilesExtendedSettingsView Control { get; set; }
        private SegmentMetadataProvider SegmentMetadataProvider { get; } = new();
        private VerifyFilesExtendedSettings VerifyFilesExtendedSettings { get; set; }

        public override object GetControl()
        {
            var controller = SdlTradosStudio.Application.GetController<ProjectsController>();

            var currentProject = controller.SelectedProjects.FirstOrDefault() ?? controller.CurrentProject;

            var languageFiles = currentProject.GetTargetLanguageFiles();

            List<Segment> statuses = [];
            foreach (var languageFile in languageFiles)
                statuses.AddRange(SegmentMetadataProvider.GetAllSegmentStatuses(currentProject, languageFile.Id));

            VerifyFilesExtendedSettings = ((ISettingsBundle)DataSource).GetSettingsGroup<VerifyFilesExtendedSettings>();
            Control = base.GetControl() as VerifyFilesExtendedSettingsView;

            var distinctStatuses = statuses.Select(s => s.Status).Distinct().ToList();
            Control.AllStatuses = distinctStatuses;

            return Control;
        }

        public override void Save()
        {
            VerifyFilesExtendedSettings = Control.GetSettings();
            base.Save();
        }
    }
}