using System.ComponentModel;
using System.Windows.Forms;
using NSubstitute;
using Sdl.Community.ExportAnalysisReports.Model;
using Sdl.Community.ExportAnalysisReports.Service;
using Xunit;

namespace Sdl.Community.ExportAnalysisReports.UnitTests
{
	public class ReportServiceUnitTests
	{
		private readonly PathInfo _pathInfo;
		private readonly SettingsService _settingsService;
		private readonly ReportService _reportService;
		private ExportAnalysisReportsConfiguration _exportReportsConfiguration;
		private readonly MessageBoxService _messageBoxService;
		private readonly ProjectService _projectService;
		private readonly string _exportTempFolder;

		public ReportServiceUnitTests()
		{
			_pathInfo = new PathInfo();
			_settingsService = new SettingsService(_pathInfo);
			_messageBoxService = Substitute.For<MessageBoxService>();
			_projectService = Substitute.For<ProjectService>();
			_exportReportsConfiguration = new ExportAnalysisReportsConfiguration();
			_reportService = Substitute.For<ReportService>(_messageBoxService, _projectService);
			_exportTempFolder = _exportReportsConfiguration.GetTempExportFolder();
		}

		/*[Fact]
		private void GetReportPath_IsSuccessfullyReturned_UnitTest()
		{
			_reportService.SaveExportPath(_exportTempFolder);

			var jsonPath = _reportService.GetJsonReportPath(_reportService.JsonPath);

			Assert.NotNull(jsonPath);
		}

		[Theory]
		[InlineData("")]
		private void GetReportPath_IsNotSuccessfullyReturned_UnitTest(string filePath)
		{
			_reportService.SaveExportPath(_exportTempFolder);
			var jsonPath = _reportService.GetJsonReportPath(filePath);

			Assert.True(string.IsNullOrEmpty(jsonPath));
		}*/

		[Theory]
		[InlineData(false, true, "Project 2")]
		private void GenerateReport_IsSuccessfullyCreated_UnitTest(bool isSingleFileProject, bool isChecked, string projectName)
		{
			var projects = new BindingList<ProjectDetails>();
			var projectDetails = _exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName);
			var optionalInformation = _exportReportsConfiguration.GetOptionalInformation();

			projects.Add(projectDetails);

			var isReportGenerated = _reportService.GenerateReportFile(projects, optionalInformation, _exportTempFolder, isChecked);

			Assert.True(isReportGenerated);
		}

		[Theory]
		[InlineData(false, false, true, "Project 2")]
		private void GenerateReport_IsNotSuccessfullyCreated_UnitTest(bool isSingleFileProject, bool shouldExport, bool isChecked, string projectName)
		{
			var projects = new BindingList<ProjectDetails>();
			var projectDetails = _exportReportsConfiguration.GetProjectDetails(isSingleFileProject, projectName);
			var optionalInformation = _exportReportsConfiguration.GetOptionalInformation();

			projectDetails.ShouldBeExported = shouldExport;
			projects.Add(projectDetails);

			_messageBoxService.ShowInformationMessage("Please select at least one language to export the report!", "Informative message").Returns(DialogResult.OK);
			var isReportGenerated = _reportService.GenerateReportFile(projects, optionalInformation, _exportTempFolder, isChecked);

			Assert.False(isReportGenerated);
		}
	}
}