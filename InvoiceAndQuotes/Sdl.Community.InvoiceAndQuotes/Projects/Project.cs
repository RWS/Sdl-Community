using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Sdl.Community.InvoiceAndQuotes.Customers;
using Sdl.Community.InvoiceAndQuotes.ResourceManager;
using Sdl.Community.InvoiceAndQuotes.Templates;

namespace Sdl.Community.InvoiceAndQuotes.Projects
{
    public enum ReportType
    {
        Detailed,
        Summary
    }

    public class Project
    {
        public List<ProjectFile> ProjectFiles { get; set; }

        private ITemplateRates _template;
        private readonly ReportType _reportType;

        public Project(String projectAnalyseFile, ITemplateRates template, ReportType reportType)
        {
            _template = template;
            _reportType = reportType;
            PrepareFilesList(projectAnalyseFile);
        }

        private void PrepareFilesList(String projectAnalyseFile)
        {
            ProjectFiles = new List<ProjectFile>();
            var projectsFile = new XPathDocument(projectAnalyseFile);
            var nav = projectsFile.CreateNavigator();
            const string expression = "task/file";
            var projectNodes = nav.Select(expression);

            ProjectFile file;
            while (projectNodes.MoveNext())
            {
                file = new ProjectFile(_template)
                    {
                        AnalyseFile = projectAnalyseFile,
                        IsSummary = false,
                        XMLNode = projectNodes.Current
                    };
                file.Prepare();
                ProjectFiles.Add(file);
            }

            file = new ProjectFile(_template)
            {
                AnalyseFile = projectAnalyseFile,
                IsSummary = true,
                FileName = (new UIResources(Settings.GetSavedCulture())).Summary,
                XMLNode = nav.SelectSingleNode("task/batchTotal")
            };
            file.Prepare();
            ProjectFiles.Add(file);
        }

        private List<ProjectFile> Files
        {
            get
            {
                return _reportType == ReportType.Summary
                    ? ProjectFiles.Where(file => file.IsSummary).ToList()
                    : ProjectFiles;
            }
        }

        public void GenerateClipboardData(Customer customer, User user)
        {
            _template.GenerateClipboardData(Files, customer, user);
        }

        public void GenerateExcelData(Customer customer, User user, String fileToSave, String template)
        {
            _template.GenerateExcelData(Files, customer, user, fileToSave, template);

        }

        public void GenerateWordData(Customer customer, User user, string fileToSave, string template)
        {
            _template.GenerateWordData(Files, customer, user, fileToSave, template);
        }
    }
}
