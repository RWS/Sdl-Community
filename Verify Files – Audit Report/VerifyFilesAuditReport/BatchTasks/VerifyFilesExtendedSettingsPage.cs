using System.Collections.Generic;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using VerifyFilesAuditReport.BatchTasks.UI;
using VerifyFilesAuditReport.Components.SegmentMetadata;
using VerifyFilesAuditReport.Components.SegmentMetadata.Model;

namespace VerifyFilesAuditReport.BatchTasks
{
    public class VerifyFilesExtendedSettingsPage : DefaultSettingsPage<VerifyFilesExtendedSettingsView, VerifyFilesExtendedSettings>
    {
        private VerifyFilesExtendedSettingsView Control { get; set; }
        private SegmentMetadataProvider SegmentMetadataProvider { get; } = new();

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

            Control = base.GetControl() as VerifyFilesExtendedSettingsView;
            Control.SetAvailableStatuses(statuses.Select(s => s.Status).Distinct());

            return Control;
        }

        public override void Save()
        {
            Control.GetSettings();
            base.Save();
        }
    }
}
