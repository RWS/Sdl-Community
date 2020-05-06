using System.Collections.Generic;
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

		string JsonPath { get; set; }
	}
}
