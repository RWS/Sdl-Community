using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Services;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace GroupshareAddIn.Tests
{
    public class ProjectReportServiceTests
    {
        private readonly Mock<IGroupshareConnection> _groupshareConnectionMock;
        private readonly Mock<IExcelReporterService> _excelReporterServiceMock;
        private ProjectReportService _projectReportService;
        private readonly List<ProjectDetailsResponse> _projectDetailsResponse;
        private readonly string _reportEntries;
        private int _count;

        public ProjectReportServiceTests()
        {

            //Arrange
            _groupshareConnectionMock = new Mock<IGroupshareConnection>();
            _excelReporterServiceMock = new Mock<IExcelReporterService>();

            var projectDataResponse = File.ReadAllText("Resources/GetGsProjectsExampleResponse.json");
            _projectDetailsResponse = JsonConvert.DeserializeObject<List<ProjectDetailsResponse>>(projectDataResponse);
            _reportEntries = File.ReadAllText("Resources/ProjectReportEntries.json");

            _groupshareConnectionMock
                .Setup(gc => gc.GetGsProjects(ProjectFilter.NoRestrictionFilter))
                .Returns(Task.FromResult(_projectDetailsResponse));
            _groupshareConnectionMock
                .Setup(gc => gc.GetProjectFiles(It.IsAny<ProjectDetailsResponse>()))
                .Returns(Task.FromResult(new List<Sdl.Community.GroupShareKit.Models.Response.File>{new Sdl.Community.GroupShareKit.Models.Response.File()}));


            _projectReportService = new ProjectReportService(_groupshareConnectionMock.Object, _excelReporterServiceMock.Object);

            _count = 0;
            _projectReportService.ProgressChanged += (progress, index) =>
            {
                _count++;
            };

            //Act
            _projectReportService.CreateFilteredProjectDataExcelSheet(ProjectFilter.NoRestrictionFilter, new CancellationToken(),
                new CancellationToken(), It.IsAny<object>());
        }

        //Assert
        [Fact]
        public void ProcessedData_PassedCorrectly_ToDependency()
        {
            _excelReporterServiceMock
                .Verify(ers => ers.PopulateExcelSheet(
                    It.Is<List<object[]>>(list => JsonConvert.SerializeObject(list) == _reportEntries),
                    It.IsAny<object>()));
        }

        [Fact]
        public void ProgressChangedEvent_RaisedCorrectNumberOfTimes()
        {
            Assert.Equal(_projectDetailsResponse.Count*3, _count);
        }

        [Fact]
        public void ShouldCall_GetProjectMethod_Once()
        {
            _groupshareConnectionMock.Verify(gsc=>gsc.GetGsProjects(It.IsAny<ProjectFilter>()), Times.Once);
        }

        [Fact]
        public void ShouldCall_GSCGetFiles_MultipleTimes()
        {
            _groupshareConnectionMock
                .Verify(gc=>gc.GetProjectFiles(It.IsAny<ProjectDetailsResponse>()), Times.Exactly(_projectDetailsResponse.Count));
        }

        [Fact]
        public void ShouldCall_ExcelReporterService_PopulateExcelSheet_ExactlyOnce()
        {
            _excelReporterServiceMock
                .Verify(ers => ers.PopulateExcelSheet(It.IsAny<List<object[]>>(), null), Times.Once);
        }
    }
}