using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging.Components;
using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportView.Model;
using System;
using System.Diagnostics;
using System.IO;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging
{
    public class ReportManager
    {
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

        public void OpenReportFolder() =>
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PostEdit.Compare", "Reports"));

        public void SaveReport(string reportFromMemory, string selectedReportReportPath) =>
                    File.WriteAllText(selectedReportReportPath, reportFromMemory);

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
    }
}