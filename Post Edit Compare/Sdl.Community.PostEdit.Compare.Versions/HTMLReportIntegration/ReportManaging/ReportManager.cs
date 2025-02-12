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
                Filter = "HTML files (*.html)|*.html",
                DefaultExt = ".html",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true) File.WriteAllText(saveFileDialog.FileName, report);
        }

        public void SaveReport(string reportFromMemory, string selectedReportReportPath) =>
            File.WriteAllText(selectedReportReportPath, reportFromMemory);

        public void OpenReportFolder() =>
            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "PostEdit.Compare", "Reports"));
    }
}