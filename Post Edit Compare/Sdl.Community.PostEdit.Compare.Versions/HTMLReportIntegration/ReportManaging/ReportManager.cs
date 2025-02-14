using Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging.Components;
using System;
using System.Diagnostics;
using System.IO;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Sdl.Community.PostEdit.Versions.HTMLReportIntegration.ReportManaging
{
    public class ReportManager
    {
        public void ExportReport(string report)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|HTML files (*.html)|*.html",
                DefaultExt = ".xlsx",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
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
        }

        public void OpenReportFolder() =>
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PostEdit.Compare", "Reports"));

        public void SaveReport(string reportFromMemory, string selectedReportReportPath) =>
                    File.WriteAllText(selectedReportReportPath, reportFromMemory);
    }
}