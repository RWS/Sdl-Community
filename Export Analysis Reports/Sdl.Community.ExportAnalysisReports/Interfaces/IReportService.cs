using System.ComponentModel;
using System.Xml;
using Sdl.Community.ExportAnalysisReports.Model;

namespace Sdl.Community.ExportAnalysisReports.Interfaces
{
	public interface IReportService
	{
		bool ReportFileExist(string reportFolderPath);
		void SetReportInformation(XmlDocument doc, ProjectDetails project);
		void SaveExportPath(string reportOutputPath);
		string GetJsonReportPath(string jsonPath);
		bool IsSameReportPath(string reportOutputPath);
		bool ReportFolderExist(XmlNode projectInfoNode, string projectXmlPath);
		bool GenerateReportFile(BindingList<ProjectDetails> projects, OptionalInformation optionalInformation, string reportOutputPath, bool isChecked);

		string JsonPath { get;  }
		string ReportsFolderPath { get; set; }
	}
}
