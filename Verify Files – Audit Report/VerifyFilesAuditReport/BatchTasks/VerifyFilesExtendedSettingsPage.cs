using System.Collections.Generic;
using System.Linq;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using VerifyFilesAuditReport.BatchTasks.UI;
using VerifyFilesAuditReport.Components.SegmentMetadata_Provider;
using VerifyFilesAuditReport.Components.SegmentMetadata_Provider.Model;

namespace VerifyFilesAuditReport.BatchTasks
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

            var languageFiles = currentProject.GetTargetLanguageFiles().Where(lf => lf.Role != FileRole.Reference);

            List<Segment> statuses = [];
            foreach (var languageFile in languageFiles)
            {
                var langFileStatuses = SegmentMetadataProvider.GetAllSegmentStatuses(currentProject, languageFile.Id);
                if (langFileStatuses is null) continue;
                statuses.AddRange(langFileStatuses);
            }

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