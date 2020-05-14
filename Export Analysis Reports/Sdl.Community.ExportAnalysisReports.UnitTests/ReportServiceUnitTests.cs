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
		private readonly ReportService _reportService;
		private ExportAnalysisReportsConfiguration _exportReportsConfiguration;
		private readonly MessageBoxService _messageBoxService;
		private readonly ProjectService _projectService;
		private readonly string _exportTempFolder;

		public ReportServiceUnitTests()
		{
			_messageBoxService = Substitute.For<MessageBoxService>();
			_projectService = Substitute.For<ProjectService>();
			_exportReportsConfiguration = new ExportAnalysisReportsConfiguration();
			_reportService = Substitute.For<ReportService>(_messageBoxService, _projectService);
			_exportTempFolder = _exportReportsConfiguration.GetTempExportFolder();
		}

		[Fact]
		private void GetReportPath_IsSuccessfullyReturned_UnitTest()
		{			
			_reportService.SaveExportPath(_exportTempFolder);

			var jsonPath = _reportService.GetJsonReportPath(_reportService.JsonPath);

			Assert.NotNull(jsonPath);
		}

		[Fact]
		private void GetReportPath_IsNotSuccessfullyReturned_UnitTest()
		{
			_reportService.SaveExportPath(_exportTempFolder);
			var jsonPath = _reportService.GetJsonReportPath(string.Empty);

			Assert.False(!string.IsNullOrEmpty(jsonPath));
		}

		[Fact]
		private void GenerateReport_IsSuccessfullyCreated_UnitTest()
		{
			var projects = new BindingList<ProjectDetails>();
			var projectDetails = _exportReportsConfiguration.GetProjectDetails();
			var optionalInformation = _exportReportsConfiguration.GetOptionalInformation();

			projects.Add(projectDetails);

			var isReportGenerated = _reportService.GenerateReportFile(projects, optionalInformation, _exportTempFolder, true);

			Assert.True(isReportGenerated);
		}

		[Fact]
		private void GenerateReport_IsNotSuccessfullyCreated_UnitTest()
		{
			var projects = new BindingList<ProjectDetails>();
			var projectDetails = _exportReportsConfiguration.GetProjectDetails();
			var optionalInformation = _exportReportsConfiguration.GetOptionalInformation();
			
			projectDetails.ShouldBeExported = false;
			projects.Add(projectDetails);

			_messageBoxService.ShowInformationMessage("Please select at least one language to export the report!", "Informative message").Returns(DialogResult.OK);
			var isReportGenerated = _reportService.GenerateReportFile(projects, optionalInformation, _exportTempFolder, true);

			Assert.False(isReportGenerated);
		}
	}
}