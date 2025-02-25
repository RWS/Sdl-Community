using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging.Components;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging
{
    public class ReportManager
    {
        private List<string> _reportFolders = [];

        public ReportManager()
        {
            if (File.Exists(SettingsFile))
                LoadReportFoldersList();
            else
                CreateDefaultFoldersList();
        }

        public List<string> ReportFolders
        {
            get => _reportFolders;
            set
            {
                _reportFolders = value;
                SaveReportFoldersList();
            }
        }

        private string SettingsFile { get; } =
                            $"{Path.Combine(Constants.PostEditCompareSettingsFolder, "ReportFolders")}.txt";

        public void BackUpReport(ReportInfo selectedReport)
        {
            if (selectedReport is null) return;

            try
            {
                var reportName = Path.GetFileNameWithoutExtension(selectedReport.ReportPath);
                var datetime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                var destinationFile =
                    $"{Path.Combine(Constants.PostEditCompareBackupFolder, reportName, datetime)}.html";

                var destinationDirectory = Path.GetDirectoryName(destinationFile);

                if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);
                File.Copy(selectedReport.ReportPath, destinationFile);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while backing up the report", ex);
            }
        }

        public void ExportReport(string report)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|HTML files (*.html)|*.html",
                DefaultExt = ".xlsx",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() != true) return;

            switch (saveFileDialog.FilterIndex)
            {
                case 1:
                    ExcelConverter.WriteExcelSpreadsheet(report, saveFileDialog.FileName);
                    break;

                case 2:
                    File.WriteAllText(saveFileDialog.FileName, report);
                    break;
            }
        }

        public List<ReportInfo> GetReports()
        {
            List<string> reportList = [];

            if (ReportFolders.Any())
                foreach (var reportFolder in ReportFolders)
                    reportList.AddRange(Directory.GetFiles(reportFolder, "*.html", SearchOption.AllDirectories).ToList());

            List<ReportInfo> reports = [];
            foreach (var report in reportList)
            {
                var projectId = ExtractProjectIdFromHtml(report);

                var directoryName = new DirectoryInfo(Path.GetDirectoryName(report) ?? string.Empty).Name;
                reports.Insert(0,
                    new ReportInfo
                    {
                        ReportName = $@"{directoryName}\\{Path.GetFileName(report)}",
                        ReportPath = report,
                        ProjectId = projectId
                    });
            }

            return reports;
        }

        public void OpenReportBackupFolder(ReportInfo selectedReport)
        {
            if (selectedReport is not null)
            {
                var reportBackupFolder = Path.Combine(Constants.PostEditCompareBackupFolder,
                    Path.GetFileNameWithoutExtension(selectedReport.ReportPath));

                if (Directory.Exists(reportBackupFolder)) { Process.Start(reportBackupFolder); return; }
            }
            Process.Start(Constants.PostEditCompareBackupFolder);
        }

        public void OpenReportFolder() =>
                    Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PostEdit.Compare", "Reports"));

        public void SaveReport(string reportFromMemory, string selectedReportReportPath) =>
                    File.WriteAllText(selectedReportReportPath, reportFromMemory);

        private static string ExtractProjectIdFromHtml(string htmlFilepath)
        {
            var doc = new HtmlDocument();
            doc.Load(htmlFilepath);

            var rows = doc.DocumentNode.SelectSingleNode("//tr[@data-project-id]");
            if (rows == null) return string.Empty;

            var projectIdAttribute = rows.Attributes["data-project-id"];
            return projectIdAttribute != null ? projectIdAttribute.Value : string.Empty;
        }

        private void CreateDefaultFoldersList()
        {
            File.Create(SettingsFile).Close();
            ReportFolders = [Constants.PostEditCompareDefaultReportsFolder];
            File.WriteAllLines(SettingsFile, ReportFolders);
        }

        private void LoadReportFoldersList()
        {
            foreach (var line in File.ReadAllLines(SettingsFile))
                if (!string.IsNullOrEmpty(line) && Directory.Exists(line))
                    ReportFolders.Add(line);
        }

        private void SaveReportFoldersList() => File.WriteAllLines(SettingsFile, ReportFolders);
    }
}