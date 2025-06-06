﻿using DocumentFormat.OpenXml.Office2013.PowerPoint;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging.Components;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging
{
    public class ReportManager
    {
        private static string _postEditCompareBackupFolder;
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
                SaveSettings();
            }
        }

        public string PostEditCompareBackupFolder
        {
            get => _postEditCompareBackupFolder;
            set
            {
                _postEditCompareBackupFolder = value;
                SaveSettings();
            }
        }

        private string SettingsFile { get; } =
                            $"{Path.Combine(Constants.PostEditCompareSettingsFolder, "ReportFolders")}.txt";

        public async Task BackUpReport(ReportInfo selectedReport)
        {
            if (selectedReport is null) return;

            var destinationFile = "";
            try
            {
                var reportName = Path.GetFileNameWithoutExtension(selectedReport.ReportPath);
                var datetime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                destinationFile = $"{Path.Combine(PostEditCompareBackupFolder, reportName, datetime)}.html";

                var destinationDirectory = Path.GetDirectoryName(destinationFile);

                if (!Directory.Exists(destinationDirectory)) Directory.CreateDirectory(destinationDirectory);
                await Task.Run(() => File.Copy(selectedReport.ReportPath, destinationFile));
            }
            catch (IOException ex)
            {
                if (ex.Message != $"The file '{destinationFile}' already exists.") throw;
            }
        }

        public void ExportReport(string report, string projectName, string selectedReportName)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|HTML files (*.html)|*.html",
                DefaultExt = ".xlsx",
                FileName = selectedReportName,
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() != true) return;

            switch (saveFileDialog.FilterIndex)
            {
                case 1:
                    ExcelConverter.WriteExcelSpreadsheet(report, saveFileDialog.FileName, projectName);
                    if (MessageBox.Show("Would you like to open the newly created report?", "New Report Created",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Process.Start(saveFileDialog.FileName);
                    break;

                case 2:
                    File.WriteAllText(saveFileDialog.FileName, report);
                    if (MessageBox.Show("Would you like to open the newly created report?", "New Report Created",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        Process.Start(saveFileDialog.FileName);
                    break;
            }
        }

        public async Task<List<ReportInfo>> GetReports()
        {
            List<ReportInfo> reports = [];
            await Task.Run(() =>
            {
                List<string> reportList = [];

                if (ReportFolders.Any())
                    foreach (var reportFolder in ReportFolders)
                        reportList.AddRange(Directory.GetFiles(reportFolder, "*.html", SearchOption.AllDirectories)
                            .ToList());

                reportList = reportList.OrderByDescending(File.GetCreationTime).ToList();

                reports = [];
                foreach (var report in reportList)
                {
                    var projectId = ExtractProjectIdFromHtml(report);

                    var directoryName = new DirectoryInfo(Path.GetDirectoryName(report) ?? string.Empty).Name;
                    reports.Add(
                        new ReportInfo
                        {
                            ReportName = $@"{directoryName}\\{Path.GetFileName(report)}",
                            ReportPath = report,
                            ProjectId = projectId
                        });
                }
            });

            return reports;
        }

        public void OpenReportBackupFolder(string reportPath)
        {
            if (!string.IsNullOrWhiteSpace(reportPath))
            {
                var reportBackupFolder = Path.Combine(PostEditCompareBackupFolder,
                    Path.GetFileNameWithoutExtension(reportPath));

                if (Directory.Exists(reportBackupFolder)) { Process.Start(reportBackupFolder); return; }
            }

            Process.Start(PostEditCompareBackupFolder);
        }

        public void OpenReportFolder(string reportPath)
        {
            var defaultReportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PostEdit.Compare", "Reports");

            if (string.IsNullOrWhiteSpace(reportPath))
                Process.Start(defaultReportPath);
            else Process.Start("explorer.exe", $"/select,\"{reportPath}\"");
        }

        public void SaveReport(string reportFromMemory, string selectedReportReportPath) =>
                    File.WriteAllText(selectedReportReportPath, reportFromMemory);

        private static string ExtractProjectIdFromHtml(string htmlFilepath)
        {
            var doc = new HtmlDocument();
            doc.Load(htmlFilepath);

            var node = doc.DocumentNode.SelectSingleNode("//a[@data-project-id]");
            return node?.GetAttributeValue("data-project-id", string.Empty) ?? string.Empty;
        }

        private void CreateDefaultFoldersList()
        {
            File.Create(SettingsFile).Close();
            ReportFolders = [Constants.PostEditCompareDefaultReportsFolder];
            PostEditCompareBackupFolder = Constants.PostEditCompareBackupFolder;

            SaveSettings();
        }

        private void LoadReportFoldersList()
        {
            dynamic settings = JsonConvert.DeserializeObject(File.ReadAllText(SettingsFile));
            if (settings == null) return;

            foreach (var line in settings.ReportFolders)
            {
                var reportFolder = line.ToString();
                if (!string.IsNullOrEmpty(reportFolder.ToString()) && Directory.Exists(reportFolder))
                    ReportFolders.Add(reportFolder);
            }

            PostEditCompareBackupFolder = settings.PostEditCompareBackupFolder;
        }

        private void SaveSettings()
        {
            var settings = new { ReportFolders = ReportFolders, PostEditCompareBackupFolder = PostEditCompareBackupFolder };
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(settings));
        }
    }
}