using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Services.EventHandlers;
using System.Collections.Generic;

namespace GroupshareExcelAddIn.Services
{
    public class ResourcesReportServices
    {
        public ResourcesReportServices(IGroupshareConnection groupshareConnection)
        {
            ReportServices = new List<IReportService>
            {
                new TranslationMemoriesReportService(groupshareConnection),
                new LanguageResourceTemplatesReportService(groupshareConnection),
                new FieldTemplatesReportService(groupshareConnection),
                new TermbasesReportService(groupshareConnection),
                new ProjectTemplateReportService(groupshareConnection),
                new CustomFieldsReportService(groupshareConnection)
            };

            AttachHandlers();
        }

        public event ProgressChangedEventHandler ProgressChanged;

        public List<IReportService> ReportServices { get; set; }

        private void AttachHandlers()
        {
            ReportServices.ForEach(rs => rs.ProgressChanged += (s, e) => ProgressChanged?.Invoke(s, e));
        }
    }
}