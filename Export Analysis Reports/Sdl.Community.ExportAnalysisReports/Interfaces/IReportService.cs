using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IReportService
	{
		bool ReportFileExist(string reportFolderPath);
		void LoadReports(XmlDocument doc, ProjectDetails project);
		Dictionary<string, LanguageDirection> LoadLanguageDirections(XmlDocument doc);
		void SaveExportPath(string reportOutputPath);
		string GetJsonReportPath(string jsonPath);
		bool IsSameReportPath(string reportOutputPath);
		bool ReportFolderExist(XmlNode projectInfoNode, string projectXmlPath);

		bool IsReportGenerated(string reportOutputPath, bool isChecked, OptionalInformation optionalInformation, BindingList<ProjectDetails> projects);

		string JsonPath { get; set; }
		string ReportsFolderPath { get; set; }
	}
}
