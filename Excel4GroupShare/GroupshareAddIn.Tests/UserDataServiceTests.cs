using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Interfaces;
using GroupshareExcelAddIn.Services;
using Moq;
using Newtonsoft.Json;
using Sdl.Community.GroupShareKit.Models.Response;
using Xunit;
using File = System.IO.File;
using UserDetails = GroupshareExcelAddIn.Models.UserDetails;

namespace GroupshareAddIn.Tests
{
    public class UserDataServiceTests
    {
        private UserDataService _userDataService;
        private Mock<IGroupshareConnection> _groupshareConnectionMock;
        private readonly Mock<IExcelReporterService> _excelReporterServiceMock;
        private readonly List<UserDetails> _userDataResponse;
        private int _progressChangesCalledTotal;
        private List<Organization> _organizationsResponse;
        private readonly string _userDataReportEntries;

        public UserDataServiceTests()
        {
            //Arrange
            _excelReporterServiceMock = new Mock<IExcelReporterService>();
            _groupshareConnectionMock = new Mock<IGroupshareConnection>();

            var userDataResponse = File.ReadAllText("Resources/UserDataResponse_AnyOrg.json");
            var organizationsResponse = File.ReadAllText(@"Resources/OrganizationsResponse_All_Flat.json");
            _userDataReportEntries = File.ReadAllText(@"Resources/UserDataReportEntries.json");
            _userDataResponse = JsonConvert.DeserializeObject<List<UserDetails>>(userDataResponse);
            _organizationsResponse = JsonConvert.DeserializeObject<List<Organization>>(organizationsResponse);

            _groupshareConnectionMock
                .Setup(gc => gc.GetAllUsers())
                .Returns(Task.FromResult(_userDataResponse));

            _groupshareConnectionMock
                .Setup(gc => gc.GetAllFilesByProject(new CancellationToken()))
                .Returns(Task.FromResult(
                    new Dictionary<string, List<Sdl.Community.GroupShareKit.Models.Response.File>>()));

            _groupshareConnectionMock
                .Setup(gc => gc.GetOrganizations(true))
                .Returns(Task.FromResult(_organizationsResponse));

            _userDataService = new UserDataService(_groupshareConnectionMock.Object, _excelReporterServiceMock.Object);

            _progressChangesCalledTotal = 0;
            _userDataService.ProgressChanged += (progress, index) =>
            {
                _progressChangesCalledTotal++;
            };

            //Act
            _userDataService.CreateUserDataExcelSheet(new CancellationToken(), new CancellationToken(), null, null);
        }

        //Assert
        [Fact]
        public void ProgressChangedEvent_RaisedCorrectNumberOfTimes()
        {
            Assert.Equal(_progressChangesCalledTotal, _userDataResponse.Count);
        }

        [Fact]
        public void ProcessedData_PassedCorrectly_ToDependency()
        {
            _excelReporterServiceMock
                .Verify(ers =>
                    ers.PopulateExcelSheet(
                        It.Is<List<object[]>>(list => JsonConvert.SerializeObject(list) == _userDataReportEntries),
                        null));
        }

        [Fact]
        public void ShouldCall_ExcelReporterService_PopulateExcelSheet_ExactlyOnce()
        {
            _excelReporterServiceMock
                .Verify(ers => ers.PopulateExcelSheet(It.IsAny<List<object[]>>(), null), Times.Once);
        }

        [Fact]
        public void ShouldCall_GetAllUsers_Once()
        {
            _groupshareConnectionMock.Verify(gsc=>gsc.GetAllUsers(), Times.Once);
        }

        [Fact]
        public void ShouldCall_GetAllFilesByProject_Once()
        {
            _groupshareConnectionMock.Verify(gsc=>gsc.GetAllFilesByProject(new CancellationToken()), Times.Once);
        }
    }
}
