using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Newtonsoft.Json;
using Reports.Viewer.Api.Model;
using Reports.Viewer.Api.Providers;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.FileBased.Reports.Operations;

namespace Reports.Viewer.Api
{
    public class ReportsController
    {
        private List<Report> _reports;
        private readonly ProjectReportsOperations _projectReportsOperations;

        public ReportsController(FileBasedProject fileBasedProject, PathInfo pathInfo, TaskTemplateIdProvider taskTemplateIdProvider)
        {
            Project = fileBasedProject;
            PathInfo = pathInfo;
            TaskTemplateIdProvider = taskTemplateIdProvider;

            _projectReportsOperations = new ProjectReportsOperations(Project);
        }

        public FileBasedProject Project { get; }

        public string ProjectLocalFolder
        {
            get
            {
                return Project?.GetProjectInfo().LocalProjectFolder.Trim('\\');
            }
        }

        public TaskTemplateIdProvider TaskTemplateIdProvider { get; }

        public PathInfo PathInfo { get; }

        public List<Report> Reports
        {
            get { return _reports; }
            set
            {
                _reports = value;
            }
        }

        public async Task<List<Report>> GetReports(bool forceRefresh)
        {
            if (forceRefresh || _reports == null || _reports?.Count == 0)
            {
                return await Task.FromResult(await GetProjectReports());
            }

            return GetClonedReports();
        }

        public ActionResult AddReports(List<Report> reports)
        {
            if (reports == null || reports.Count == 0)
            {
                return new ActionResult(false)
                {
                    Message = StringResources.Message_TheParameterCannotBeNull
                };
            }

            if (reports.Exists(a => a.Id == Guid.Empty || a.ToString() == string.Empty))
            {
                return new ActionResult(false)
                {
                    Message = StringResources.Message_TheReportIdCannotBeNull
                };
            }

            var clonedReports = AddRecordData(reports);
            UpdateProjectReports();

            return new ActionResult(true) { Reports = clonedReports };
        }

        public ActionResult UpdateReports(List<Report> reports)
        {
            if (reports == null || reports.Count == 0)
            {
                return new ActionResult(false)
                {
                    Message = StringResources.Message_TheParameterCannotBeNull
                };
            }

            if (reports.Exists(a => a.Id == Guid.Empty || a.ToString() == string.Empty))
            {
                return new ActionResult(false)
                {
                    Message = StringResources.Message_TheReportIdCannotBeNull
                };
            }

            foreach (var report in reports)
            {
                var existingReport = _reports?.FirstOrDefault(a => a.Id == report.Id);
                if (existingReport == null)
                {
                    return new ActionResult(false)
                    {
                        Message = string.Format(StringResources.Message_UnableToLocateReport, report.Id)
                    };
                }
            }

            List<Report> clonedReports = null;
            try
            {
                clonedReports = UpdateReportData(reports);
                UpdateProjectReports();

                return new ActionResult(true) { Reports = clonedReports };
            }
            catch (Exception e)
            {
                return new ActionResult(false)
                {
                    Reports = clonedReports,
                    Message = e.Message
                };
            }
        }

        public ActionResult RemoveReports(List<Guid> reportIds)
        {
            if (reportIds == null || reportIds.Count == 0)
            {
                return new ActionResult(false)
                {
                    Message = StringResources.Message_TheParameterCannotBeNull
                };
            }

            if (reportIds.Exists(a => a == Guid.Empty || a.ToString() == string.Empty))
            {
                return new ActionResult(false)
                {
                    Message = StringResources.Message_TheReportIdCannotBeNull
                };
            }

            foreach (var reportId in reportIds)
            {
                var existingReport = _reports?.FirstOrDefault(a => a.Id == reportId);
                if (existingReport == null)
                {
                    return new ActionResult(false)
                    {
                        Message = string.Format(StringResources.Message_UnableToLocateReport, reportId)
                    };
                }
            }

            List<Report> clonedReports = null;
            try
            {
                clonedReports = RemoveReportData(reportIds);
                UpdateProjectReports();

                return new ActionResult(true) { Reports = clonedReports };
            }
            catch (Exception e)
            {
                return new ActionResult(false)
                {
                    Reports = clonedReports,
                    Message = e.Message
                };
            }
        }

        public bool IsDirty
        {
            get
            {
                try
                {
                    var settingsBundle = Project?.GetSettings();
                    var reportViewerSettings = settingsBundle?.GetSettingsGroup<ReportsViewerSettings>();
                    var nonStudioReports = DeserializeReports(reportViewerSettings?.ReportsJson?.Value)
                        ?.Where(a => !a.IsStudioReport).ToList();

                    var studioReports = GetStudioReports();

                    var count1 = nonStudioReports?.Count + studioReports?.Count;
                    var count2 = _reports?.Count;

                    return count1 != count2;
                }
                catch
                {
                    //ignore; catch all
                }

                return false;
            }
        }

        private List<Sdl.ProjectAutomation.FileBased.Reports.Models.Report> GetStudioReports()
        {
            try
            {
                return _projectReportsOperations?.GetProjectReports();
            }
            catch
            {
                // ignore; catch all
            }

            return new List<Sdl.ProjectAutomation.FileBased.Reports.Models.Report>();
        }

        private async Task<List<Report>> GetProjectReports()
        {
            _reports = new List<Report>();

            if (Project != null)
            {
                var settingsBundle = Project.GetSettings();
                var reportViewerSettings = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();
                var savedReports = DeserializeReports(reportViewerSettings.ReportsJson.Value);
                var studioReports = GetStudioReports();
                var reports = new List<Report>();

                // get the reports managed by studio with updated settings from the reviewer
                foreach (var studioReport in studioReports)
                {
                    var savedReport = savedReports.FirstOrDefault(a => a.Id == studioReport.Id);
                    var report = new Report
                    {
                        Id = studioReport.Id,
                        Name = studioReport.Name,
                        Description = studioReport.Description,
                        Date = studioReport.Date,
                        Group = savedReport?.Group ?? studioReport.Group,
                        Language = studioReport.Language,
                        Path = studioReport.Path,
                        TemplateId = studioReport.TemplateId,
                        IsCustomReport = studioReport.IsCustomReport,
                        IsStudioReport = true,
                        IsSelected = savedReport?.IsSelected ?? false,
                        XsltPath = savedReport?.XsltPath ?? string.Empty
                    };

                    if (!TaskTemplateIdProvider.TaskTemplateIdExists(studioReport.TemplateId))
                    {
                        TaskTemplateIdProvider.AddTemplateId(studioReport.TemplateId, studioReport.TemplateId);
                    }

                    reports.Add(report);
                }

                // recover also the saved reports that aren't managed by Studio
                foreach (var report in savedReports)
                {
                    var existingReport = reports.FirstOrDefault(a => a.Id == report.Id);
                    if (existingReport == null)
                    {
                        reports.Add(report);
                    }
                }

                var customReportTemplates = DeserializeCustomReportTemplates(reportViewerSettings.CustomReportTemplatesJson.Value);
                var overwrite = UICultureChanged(reportViewerSettings);

                var reportTemplates = GetCustomReportTemplates();
                var updatedReports = GetUpdatedReports(reports, reportTemplates, overwrite, ref customReportTemplates);
                UpdateCustomTemplateReports(customReportTemplates);

                _reports = updatedReports;
            }

            return await Task.FromResult(GetClonedReports(_reports));
        }

        private List<Report> AddRecordData(List<Report> reports)
        {
            if (_reports == null)
            {
                _reports = new List<Report>();
            }

            var clonedReports = new List<Report>();
            foreach (var report in reports)
            {
                var existingReport = _reports?.FirstOrDefault(a => a.Id == report.Id);
                if (existingReport == null)
                {
                    if (report.Path.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase) &&
                        TaskTemplateIdProvider.TaskTemplateIdExists(report.TemplateId))
                    {
                        var reportData = File.ReadAllText(report.Path);
                        var addedReport = _projectReportsOperations
                            .AddReport(report.TemplateId, report.Name, report.Description, report.Language, reportData);

                        report.Id = addedReport.Id;
                        report.IsCustomReport = addedReport.IsCustomReport;
                        report.IsStudioReport = true;
                        var reportPath = Path.Combine(ProjectLocalFolder, addedReport.Path);
                        if (string.Compare(report.Path, reportPath, StringComparison.InvariantCultureIgnoreCase) != 0)
                        {
                            AttemptDeleteFile(report.Path);
                        }
                        report.Path = reportPath;
                    }
                    else
                    {
                        report.IsStudioReport = false;
                    }

                    var clonedReport = GetClonedReport(report);
                    clonedReports.Add(clonedReport);
                }
            }

            Project.Save();

            var updatedReports = GetUpdatedReports(clonedReports);
            _reports.AddRange(updatedReports);

            return _reports;
        }

        private List<Report> UpdateReportData(List<Report> reports)
        {
            var clonedReports = new List<Report>();
            if (reports == null)
            {
                return clonedReports;
            }

            foreach (var report in reports)
            {
                var existingReport = _reports?.FirstOrDefault(a => a.Id == report.Id);
                if (existingReport == null)
                {
                    continue;
                }

                var clonedReport = GetClonedReport(report);

                existingReport.Name = clonedReport.Name;
                existingReport.Description = clonedReport.Description;
                if (!clonedReport.IsStudioReport)
                {
                    existingReport.Group = clonedReport.Group;
                    existingReport.Language = clonedReport.Language;
                }

                if (existingReport.IsStudioReport)
                {
                    var xmlPath = Path.Combine(ProjectLocalFolder, report.Path);

                    if (!xmlPath.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        xmlPath = GetPreviousExtensionPath(xmlPath);
                    }

                    if (xmlPath.EndsWith(".xml", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var reportData = File.ReadAllText(xmlPath);
                        _projectReportsOperations.UpdateReport(existingReport.Id, Guid.NewGuid().ToString(), existingReport.Description, reportData);
                        _projectReportsOperations.UpdateReport(existingReport.Id, existingReport.Name,
                            existingReport.Description, reportData);
                    }
                }

                clonedReports.Add(existingReport.Clone() as Report);
            }

            Project.Save();


            if (reports.Exists(a => !a.IsStudioReport))
            {
                clonedReports = GetUpdatedReports(clonedReports);
            }


            return clonedReports;
        }

        private List<Report> GetUpdatedReports(List<Report> reports, IReadOnlyCollection<ReportTemplate> reportTemplates,
            bool overwrite, ref List<CustomReportTemplate> customReportTemplates)
        {
            var updatedReports = new List<Report>();
            foreach (var report in reports)
            {
                if (report == null || string.IsNullOrEmpty(report.Path))
                {
                    continue;
                }

                var reportTemplate = GetReportTemplate(report, reportTemplates);
                var xmlPath = Path.Combine(ProjectLocalFolder, report.Path);
                var xsltPath = string.Empty;
                var shouldOverwrite = overwrite;

                if (!report.IsStudioReport)
                {
                    xsltPath = Path.Combine(ProjectLocalFolder, report.XsltPath ?? string.Empty);
                    if (!xmlPath.ToLower().EndsWith(".xml"))
                    {
                        xmlPath = GetPreviousExtensionPath(xmlPath);
                    }

                    if (!File.Exists(xsltPath) || !File.Exists(xmlPath) || !xmlPath.ToLower().EndsWith(".xml"))
                    {
                        updatedReports.Add(report);
                        continue;
                    }
                }
                else
                {
                    if (!File.Exists(xmlPath) || !xmlPath.ToLower().EndsWith(".xml"))
                    {
                        updatedReports.Add(report);
                        continue;
                    }

                    if (!TaskTemplateIdProvider.TaskTemplateIdExists(report.TemplateId))
                    {
                        throw new Exception(string.Format("Invalid template id {0}", report.TemplateId));
                    }
                }

                if (reportTemplate != null)
                {
                    var customReportTemplate = customReportTemplates?.FirstOrDefault(a =>
                        a.ReportId == report.Id.ToString() && a.ReportTemplateId == reportTemplate.Id);
                    if (customReportTemplate == null)
                    {
                        xsltPath = reportTemplate.Path;
                        shouldOverwrite = true;
                        UpdateCustomTemplates(customReportTemplates, report, reportTemplate);
                    }
                }
                else
                {
                    RemoveCustomReportTemplate(report, customReportTemplates, ref shouldOverwrite);
                }

                var reportPath = CreateHtmlReport(report.Id, xsltPath, xmlPath, report.IsStudioReport, shouldOverwrite);
                SetReportVirtualPath(report, ProjectLocalFolder, reportPath);

                updatedReports.Add(report);
            }

            return updatedReports;
        }

        private List<Report> GetUpdatedReports(List<Report> clonedReports)
        {
            var settingsBundle = Project.GetSettings();
            var reportViewerSettings = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();
            var reportTemplates = GetCustomReportTemplates();
            var customReportTemplates = DeserializeCustomReportTemplates(reportViewerSettings.CustomReportTemplatesJson.Value);
            var overwrite = UICultureChanged(reportViewerSettings);

            clonedReports = GetUpdatedReports(clonedReports, reportTemplates, overwrite, ref customReportTemplates);

            UpdateCustomTemplateReports(customReportTemplates);


            return clonedReports;
        }

        private List<Report> RemoveReportData(List<Guid> reportIds)
        {
            var clonedReports = new List<Report>();
            if (reportIds == null)
            {
                return clonedReports;
            }

            foreach (var reportId in reportIds)
            {
                var existingReport = _reports?.FirstOrDefault(a => a.Id == reportId);
                if (existingReport == null)
                {
                    continue;
                }

                var clonedReport = GetClonedReport(existingReport);
                clonedReports.Add(clonedReport);

                DeleteReportFile(clonedReport);

                _reports.Remove(existingReport);
            }

            _projectReportsOperations.RemoveReports(reportIds);

            return clonedReports;
        }

        private static void AttemptDeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // ignore; catch all
            }
        }

        private void DeleteReportFile(Report report)
        {
            if (Project == null)
            {
                return;
            }

            if (!Directory.Exists(ProjectLocalFolder))
            {
                return;
            }

            try
            {
                var htmlPath = Path.Combine(ProjectLocalFolder, report.Path);
                if (File.Exists(htmlPath))
                {
                    File.Delete(htmlPath);
                }

                // attempt to also remove the corresponding xml file
                if (!htmlPath.ToLower().EndsWith(".xml"))
                {
                    var extension = Path.GetExtension(htmlPath);
                    {
                        var xmlPath = htmlPath.Substring(0, htmlPath.Length - extension.Length);
                        if (xmlPath.ToLower().EndsWith(".xml") && File.Exists(xmlPath))
                        {
                            File.Delete(xmlPath);
                        }
                    }
                }

                // no attempt should be made to remove the .xslt file
            }
            catch
            {
                // catch all; ignore
            }
        }

        private string CreateHtmlReport(Guid id, string xsltPath, string xmlPath, bool isStudioReport, bool replace)
        {
            var reportPath = xmlPath + ".html";
            if (File.Exists(reportPath))
            {
                if (replace)
                {
                    File.Delete(reportPath);
                }
                else
                {
                    return reportPath;
                }
            }

            var success = isStudioReport
                ? TransformXmlReport(id, xsltPath, xmlPath, ref reportPath)
                : TransformXmlReport(xmlPath, xsltPath, reportPath);
            if (!success)
            {
                reportPath = xmlPath;
            }

            return reportPath;

        }

        private bool TransformXmlReport(Guid id, string xsltPath, string xmlPath, ref string reportPath)
        {
            try
            {
                var html = string.IsNullOrEmpty(xsltPath)
                ? Encoding.UTF8.GetString(_projectReportsOperations.GetReportRendering(id, "html"))
                : Encoding.UTF8.GetString(_projectReportsOperations.GetReportRendering(id, xsltPath, "html"));

                if (string.IsNullOrEmpty(html))
                {
                    reportPath = xmlPath;
                }
                else
                {
                    using (var sw = new StreamWriter(reportPath, false, Encoding.UTF8))
                    {
                        sw.Write(html);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }
            catch
            {
                //catch all; ignore
                return false;
            }

            return true;
        }

        private bool TransformXmlReport(string xmlPath, string xsltPath, string htmlPath)
        {
            try
            {
                var xsltSetting = new XsltSettings
                {
                    EnableDocumentFunction = true,
                    EnableScript = true
                };

                var myXPathDoc = new XPathDocument(xmlPath);

                var myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(xsltPath, xsltSetting, null);

                var myWriter = new XmlTextWriter(htmlPath, Encoding.UTF8);

                myXslTrans.Transform(myXPathDoc, null, myWriter);
                myWriter.Flush();
                myWriter.Close();
            }
            catch
            {
                //catch all; ignore
                return false;
            }

            return true;
        }

        private static void SetReportVirtualPath(Report report, string projectLocalFolder, string reportPath)
        {
            if (reportPath == null)
            {
                return;
            }

            if (reportPath.Contains(projectLocalFolder))
            {
                reportPath = reportPath.Substring(projectLocalFolder.Length + 1);
            }

            report.Path = reportPath;
        }

        private void UpdateCustomTemplateReports(IReadOnlyCollection<CustomReportTemplate> customTemplates)
        {
            var settingsBundle = Project.GetSettings();
            var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();

            reportViewerProject.CustomReportTemplatesJson.Value = JsonConvert.SerializeObject(customTemplates);

            Project.UpdateSettings(reportViewerProject.SettingsBundle);
            Project.Save();
        }

        private void RemoveCustomReportTemplate(Report report, List<CustomReportTemplate> customReportTemplates, ref bool shouldOverwrite)
        {
            var record = customReportTemplates?.FirstOrDefault(a => a.ReportId == report.Id.ToString());
            if (record != null)
            {
                shouldOverwrite = true;
                customReportTemplates.RemoveAll(a => a.ReportId == report.Id.ToString());
            }
        }

        private ReportTemplate GetReportTemplate(Report report, IReadOnlyCollection<ReportTemplate> reportTemplates)
        {
            var uniqueReportId = GetUniqueReportId(report.Group, report.Language, report.IsStudioReport
                    ? ReportTemplate.TemplateScope.StudioOnly.ToString()
                    : ReportTemplate.TemplateScope.NonStudioOnly.ToString());
            var reportTemplate = reportTemplates.FirstOrDefault(a => GetUniqueReportId(a) == uniqueReportId && File.Exists(a.Path));

            if (reportTemplate == null)
            {
                uniqueReportId = GetUniqueReportId(report.Group, string.Empty, report.IsStudioReport
                    ? ReportTemplate.TemplateScope.StudioOnly.ToString()
                    : ReportTemplate.TemplateScope.NonStudioOnly.ToString());
                reportTemplate = reportTemplates.FirstOrDefault(a => GetUniqueReportId(a) == uniqueReportId && File.Exists(a.Path));
            }

            if (reportTemplate == null)
            {
                uniqueReportId = GetUniqueReportId(report.Group, report.Language, ReportTemplate.TemplateScope.All.ToString());
                reportTemplate = reportTemplates.FirstOrDefault(a => GetUniqueReportId(a) == uniqueReportId && File.Exists(a.Path));
            }

            if (reportTemplate == null)
            {
                uniqueReportId = GetUniqueReportId(report.Group, string.Empty, ReportTemplate.TemplateScope.All.ToString());
                reportTemplate = reportTemplates.FirstOrDefault(a => GetUniqueReportId(a) == uniqueReportId && File.Exists(a.Path));
            }

            return reportTemplate;
        }

        private static string GetPreviousExtensionPath(string path)
        {
            var extension = Path.GetExtension(path);
            {
                if (extension != null)
                {
                    path = path.Substring(0, path.Length - extension.Length);
                }
            }

            return path;
        }

        private static ICollection<CustomReportTemplate> UpdateCustomTemplates(ICollection<CustomReportTemplate> customReportTemplates,
            Report studioReport, ReportTemplate reportTemplate)
        {
            var customReportTemplate = customReportTemplates?.FirstOrDefault(a => a.ReportId == studioReport.Id.ToString());
            if (customReportTemplate == null)
            {
                customReportTemplates?.Add(new CustomReportTemplate
                {
                    ReportId = studioReport.Id.ToString(),
                    ReportTemplateId = reportTemplate.Id,
                    IsStudioReport = studioReport.IsStudioReport
                });
            }
            else
            {
                customReportTemplate.ReportTemplateId = reportTemplate.Id;
            }

            return customReportTemplates;
        }

        private string GetUniqueReportId(ReportTemplate reportTemplate)
        {
            return GetUniqueReportId(reportTemplate?.Group
                                     ?? string.Empty, reportTemplate?.Language
                                                      ?? string.Empty, reportTemplate?.Scope.ToString());
        }

        private string GetUniqueReportId(string group, string language, string scope)
        {
            var id = (group + "-" + language + "-" + scope).ToLower();
            return id;
        }

        private void UpdateProjectReports()
        {
            var settingsBundle = Project.GetSettings();
            var reportViewerProject = settingsBundle.GetSettingsGroup<ReportsViewerSettings>();

            var reports = GetClonedReports().Where(a => !a.IsStudioReport).ToList();

            reportViewerProject.ReportsJson.Value = JsonConvert.SerializeObject(reports);
            reportViewerProject.UICultureName.Value = Thread.CurrentThread.CurrentUICulture.Name;

            Project.UpdateSettings(reportViewerProject.SettingsBundle);
            Project.Save();
        }

        private List<Report> GetClonedReports()
        {
            return GetClonedReports(Reports);
        }

        private Report GetClonedReport(ICloneable report)
        {
            var clonedReport = report.Clone() as Report;

            MoveReportFilesToRelativePath(clonedReport);

            return clonedReport;
        }

        private List<Report> GetClonedReports(IReadOnlyCollection<Report> reports)
        {
            var clonedReports = new List<Report>();
            if (reports != null)
            {
                foreach (var report in reports)
                {
                    clonedReports.Add(GetClonedReport(report));
                }
            }

            return clonedReports;
        }

        private void MoveReportFilesToRelativePath(Report report)
        {
            if (Project == null || string.IsNullOrEmpty(report.Path) || !File.Exists(report.Path))
            {
                return;
            }

            if (report.IsStudioReport)
            {
                if (report.Path.Contains(ProjectLocalFolder))
                {
                    report.Path = report.Path.Substring(ProjectLocalFolder.Length + 1);
                }

                return;
            }

            var reportsViewerFolder = GetReportsViewerFolder();
            var customReportsFolderName = Path.GetDirectoryName(report.Path);

            if (string.Compare(customReportsFolderName, Constants.ReportsViewerFolderName, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                report.Path = GetRelativePath(report.Path, reportsViewerFolder);
                if (!string.IsNullOrEmpty(report.XsltPath) && File.Exists(report.XsltPath))
                {
                    var templatesFolderPath = Path.Combine(ProjectLocalFolder, Constants.ReportsViewerFolderName, Constants.ReportsViewerTemplatesFolderName);
                    if (!Directory.Exists(templatesFolderPath))
                    {
                        Directory.CreateDirectory(templatesFolderPath);
                    }
                    var templatePath = Path.Combine(templatesFolderPath, Path.GetFileName(report.XsltPath));

                    if (!File.Exists(templatePath) && string.Compare(templatePath, report.XsltPath, StringComparison.CurrentCultureIgnoreCase) != 0)
                    {
                        File.Copy(report.XsltPath, templatePath);
                    }
                    report.XsltPath = $"{Constants.ReportsViewerFolderName}\\{Constants.ReportsViewerTemplatesFolderName}\\{Path.GetFileName(report.XsltPath)}";
                }
            }
        }

        private string GetReportsViewerFolder()
        {
            var reportsFolder = Path.Combine(ProjectLocalFolder, Constants.ReportsViewerFolderName);
            if (!Directory.Exists(reportsFolder))
            {
                Directory.CreateDirectory(reportsFolder);
            }

            return reportsFolder;
        }

        private string GetRelativePath(string path, string reportsFolder)
        {
            var reportName = Path.GetFileName(path);
            var reportFullPath = Path.Combine(reportsFolder, reportName);
            var index = 1;
            while (File.Exists(reportFullPath) && index < 1000)
            {
                var extension = Path.GetExtension(reportName);
                var fileName = reportName.Substring(0, reportName.Length - extension.Length);
                var newReportName = $"{fileName}({index++}){extension}";

                reportFullPath = Path.Combine(reportsFolder, newReportName);
            }

            File.Copy(path, reportFullPath);
            return $"{Constants.ReportsViewerFolderName}\\{Path.GetFileName(reportFullPath)}";
        }

        private List<ReportTemplate> GetCustomReportTemplates()
        {
            if (File.Exists(PathInfo.CustomReportTemplatesFilePath))
            {
                var json = File.ReadAllText(PathInfo.CustomReportTemplatesFilePath);
                return JsonConvert.DeserializeObject<List<ReportTemplate>>(json);
            }

            return new List<ReportTemplate>();
        }

        private static List<CustomReportTemplate> DeserializeCustomReportTemplates(string value)
        {
            try
            {
                var studioReportTemplates = JsonConvert.DeserializeObject<List<CustomReportTemplate>>(value);
                return studioReportTemplates?.ToList() ?? new List<CustomReportTemplate>();
            }
            catch
            {
                // catch all; ignore
            }

            return new List<CustomReportTemplate>();
        }

        private static List<Report> DeserializeReports(string value)
        {
            try
            {
                var reports = JsonConvert.DeserializeObject<List<Report>>(value);
                return reports?.ToList() ?? new List<Report>();
            }
            catch
            {
                // catch all; ignore
            }

            return new List<Report>();
        }

        private bool UICultureChanged(ReportsViewerSettings reportsViewerSettings)
        {
            var uiCultureName = reportsViewerSettings.UICultureName;

            if (Thread.CurrentThread.CurrentUICulture.Name != uiCultureName.Value)
            {
                uiCultureName.Value = Thread.CurrentThread.CurrentUICulture.Name;
                Project.UpdateSettings(reportsViewerSettings.SettingsBundle);
                Project.Save();

                return true;
            }

            return false;
        }
    }
}
