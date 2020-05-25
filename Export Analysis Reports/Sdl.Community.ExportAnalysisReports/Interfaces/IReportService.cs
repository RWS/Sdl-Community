using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IReportService
	{
		string JsonPath { get; }

		string ReportsFolderPath { get; set; }

		bool GenerateReportFile(BindingList<ProjectDetails> projects, OptionalInformation optionalInformation, string reportOutputPath, bool isChecked);

		string GetJsonReportPath(string jsonPath);

		bool IsSameReportPath(string reportOutputPath);

		bool ReportFolderExist(XmlNode projectInfoNode, string projectXmlPath);

		void SaveExportPath(string reportOutputPath);

		void SetReportInformation(ProjectDetails project);

		List<ProjectDetails> GetExternalProjectReportInfo(string fileName);

		void PrepareAnalysisReport(string pathToXmlReport);

		string GetCsvContent(bool includeHeader, OptionalInformation aditionalHeaders);
	}
}