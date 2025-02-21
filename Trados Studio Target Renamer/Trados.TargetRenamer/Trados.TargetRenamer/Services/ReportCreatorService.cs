using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using NLog;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Trados.TargetRenamer.BatchTask;

namespace Trados.TargetRenamer.Services
{
	public class ReportCreatorService
    {
        public string ReportFile;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void CreateReport(
            IProject project,
            List<ProjectFile> projectFiles,
            Dictionary<(ProjectFile, LanguageDirection), Tuple<string, string>> renamedFiles,
            TargetRenamerSettings renamerSettings,
            LanguageDirection languageDirection)
        {
            var languageCode = languageDirection.TargetLanguage.IsoAbbreviation;
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false };
            var tempFile = Path.GetTempFileName();

            using (var writer = XmlWriter.Create(tempFile, settings))
            {
                writer.WriteStartElement("task");
                writer.WriteAttributeString("name", Constants.PluginName);
                writer.WriteAttributeString("created",
                    project.GetProjectInfo().CreatedAt.ToString("MM/dd/yyyy hh:mm tt"));
                var location = project.GetProjectInfo().LocalProjectFolder;

                WriteReportTaskInfo(project, location, languageCode, writer, renamerSettings);

                WriteReportFilesInfo(projectFiles, renamedFiles, renamerSettings, writer);

                writer.WriteEndElement(); // task end tag
            }

            string reportData;
            using (var r = new StreamReader(tempFile, Encoding.UTF8))
            {
                reportData = r.ReadToEnd();
                r.Close();
            }

            ReportFile = reportData;

            try
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);
            }
            catch (Exception exception)
            {
                _logger.Error($"{exception.Message}\n {exception.StackTrace}");
            }
        }

        private static void WriteReportCustomerInfo(IProject project, XmlWriter writer)
        {
            if (!string.IsNullOrEmpty(project.GetProjectInfo().OrganizationPath))
            {
                writer.WriteStartElement("customer");
                writer.WriteAttributeString("name", project.GetProjectInfo().OrganizationPath);
                writer.WriteEndElement(); //customer end tag
            }
        }

        private static void WriteReportFilesInfo(List<ProjectFile> projectFiles,
                    Dictionary<(ProjectFile, LanguageDirection), Tuple<string, string>> renamedFiles,
            TargetRenamerSettings renamerSettings,
            XmlWriter writer)
        {
            writer.WriteStartElement("files");
            foreach (var projectFile in projectFiles)
            {
                writer.WriteStartElement("file");

                var file = renamedFiles.Keys.SingleOrDefault(x => x.Item1.Id == projectFile.Id);
                renamedFiles.TryGetValue(file, out var renamedFileNames);
                writer.WriteAttributeString("name", projectFile.Name);
                writer.WriteAttributeString("originalName", renamedFileNames.Item1);
                writer.WriteAttributeString("newName", renamedFileNames.Item2);
                writer.WriteAttributeString("location", file.Item1.Folder);
                writer.WriteAttributeString("newLocation",
                    renamerSettings.UseCustomLocation
                        ? Path.Combine(renamerSettings.CustomLocation, projectFile.Folder)
                        : Path.Combine(file.Item2.TargetLanguage.ToString(), projectFile.Folder));
                writer.WriteEndElement(); // file end tag
            }

            writer.WriteEndElement(); // files end tag
        }

        private static void WriteReportLanguageInfo(string languageCode, XmlWriter writer)
        {
            writer.WriteStartElement("language");
            writer.WriteAttributeString("id", languageCode);
            writer.WriteAttributeString("name", new CultureInfo(languageCode).DisplayName);
            writer.WriteEndElement(); //language end tag
        }

        private static void WriteReportProjectInfo(IProject project, XmlWriter writer)
        {
            writer.WriteStartElement("project");
            writer.WriteAttributeString("name", project.GetProjectInfo().Name);
            writer.WriteAttributeString("number", project.GetProjectInfo().Id.ToString());
            if (project.GetProjectInfo().DueDate != DateTime.MinValue &&
                project.GetProjectInfo().DueDate != DateTime.MaxValue)
                writer.WriteAttributeString("dueDate", project.GetProjectInfo().DueDate.ToString());

            writer.WriteEndElement(); // project end tag
        }

        private static void WriteReportSettingsInfo(string location, XmlWriter writer, TargetRenamerSettings settings)
        {
            writer.WriteStartElement("settings");
            writer.WriteAttributeString("path", location);
            writer.WriteAttributeString("delimiter", settings.Delimiter);
            writer.WriteAttributeString("targetLanguage", settings.AppendTargetLanguage.ToString());
            writer.WriteAttributeString("shortLocales", settings.UseShortLocales.ToString());
            writer.WriteAttributeString("useCustomString", settings.AppendCustomString.ToString());
            writer.WriteAttributeString("customString", settings.CustomString);
            writer.WriteAttributeString("useRegExpr", settings.UseRegularExpression.ToString());
            writer.WriteAttributeString("regExprSearchFor", settings.RegularExpressionSearchFor);
            writer.WriteAttributeString("regExprReplaceWith", settings.RegularExpressionReplaceWith);
            writer.WriteAttributeString("suffix", settings.AppendAsSuffix.ToString());
            writer.WriteAttributeString("prefix", settings.AppendAsPrefix.ToString());
            writer.WriteEndElement(); // settings end tag
        }

        private static void WriteReportTaskInfo(IProject project, string location, string languageCode,
                                    XmlWriter writer, TargetRenamerSettings settings)
        {
            writer.WriteStartElement("taskInfo");
            writer.WriteAttributeString("taskId", Guid.NewGuid().ToString());
            writer.WriteAttributeString("runAt", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

            WriteReportProjectInfo(project, writer);

            WriteReportLanguageInfo(languageCode, writer);

            WriteReportCustomerInfo(project, writer);

            WriteReportSettingsInfo(location, writer, settings);

            writer.WriteEndElement(); // taskInfo end tag
        }
    }
}