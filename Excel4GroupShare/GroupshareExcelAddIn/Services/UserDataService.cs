using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Models;
using Sdl.Community.GroupShareKit.Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Interfaces;
using Newtonsoft.Json;
using File = System.IO.File;
using UserDetails = GroupshareExcelAddIn.Models.UserDetails;

namespace GroupshareExcelAddIn.Services
{
    public class UserDataService
    {
        private readonly IExcelReporterService _excelReporter;
        private readonly IGroupshareConnection _groupshareConnection;
        private CancellationToken _dataRetrievalCancellationToken;
        private List<object[]> _report;
        private dynamic _userWorksheet;

        public UserDataService(IGroupshareConnection groupshareConnection, IExcelReporterService excelReporter)
        {
            _groupshareConnection = groupshareConnection;
            _excelReporter = excelReporter;
            _groupshareConnection.ConnectionChanged += (o, args) =>
            {
                UserDataSourceChanged?.Invoke(o, EventArgs.Empty);
            };
            _groupshareConnection.ProgressChanged += (progress, barIndex) => ProgressChanged?.Invoke(progress, barIndex);

            _excelReporter.ProgressChanged += progress => ProgressChanged?.Invoke(progress, 3);
        }

        public delegate void UserDataProgressChangedEventHandler(Progress progress, int barIndex);

        public event UserDataProgressChangedEventHandler ProgressChanged;

        public event EventHandler UserDataSourceChanged;

        public async Task CreateUserDataExcelSheet(CancellationToken dataRetrievalCancellationToken, CancellationToken dataWritingCancellationToken, dynamic userWorksheet, Organization filter)
        {
            _dataRetrievalCancellationToken = dataRetrievalCancellationToken;
            _userWorksheet = userWorksheet;
            await CreateReport(dataWritingCancellationToken, filter);
        }

        private void AddHeaderToReport()
        {
            _report = new List<object[]>();
            var reportHeader = Constants.UsersColumns.ToArray<object>();
            _report.Add(reportHeader);
        }

        private async Task CreateReport(CancellationToken dataWritingCancellationToken, Organization organization)
        {
            var entries = await PrepareData(organization);
            GenerateReportData(entries);
            _excelReporter.Token = dataWritingCancellationToken;
            await _excelReporter.PopulateExcelSheet(_report, _userWorksheet);
        }

        private async Task<List<UserEntry>> PrepareData(Organization organization)
        {
            var users = await GetUsers(organization);

            var filesByProject = await _groupshareConnection.GetAllFilesByProject(_dataRetrievalCancellationToken);
            var relevantFilesByProject = filesByProject.Where(fileByProjectEntry =>
                fileByProjectEntry.Value.Any(f => f.Assignees.Any(a => users.Any(u => u.UniqueId == a.UserId)))).ToList();
            var assigneesByProject = relevantFilesByProject.Select(entry => (entry.Key,
                entry.Value.SelectMany(f => f.Assignees).ToList())).ToList();

            var organizations = (await _groupshareConnection.GetOrganizations(true));
            var organizationsById =
                organizations.ToDictionary(org => org.UniqueId.ToString(),
                    org => org.Name);

            var usersByOrganization = users.GroupBy(u => u.OrganizationId).ToList();

            var entries = new List<UserEntry>();
            foreach (var userGroup in usersByOrganization)
            {
                foreach (var user in userGroup)
                {
                    var currentProjects =
                        assigneesByProject.Where(entry => entry.Item2.Any(a => a.UserId == user.UniqueId)).ToList();

                    var currentProjectsAndCurrentAssignee = currentProjects.Select(entry =>
                        (entry.Item1,
                            entry.Item2.Where(ud => ud.UserId == user.UniqueId).SelectMany(ud => ud.Role.Split(',').ToList())
                                .ToList())).ToList();
                    entries.Add(new UserEntry
                    {
                        UserDetails = user,
                        Organization = organizationsById[userGroup.Key],
                        RolesByProject = currentProjectsAndCurrentAssignee
                    });
                }
            }

            return entries;
        }

        private void GenerateReportData(List<UserEntry> entries)
        {
            AddHeaderToReport();

            var index = 0;
            foreach (var entry in entries)
            {
                index++;
                var projectIndex = 0;
                do
                {
                    var projectWithRoles = entry.RolesByProject?.Count > 0
                        ? entry.RolesByProject[projectIndex]
                        : (string.Empty, null);
                    var roleIndex = 0;
                    do
                    {
                        var role = projectWithRoles.Item2?.Count > 0 ? projectWithRoles.Item2[roleIndex] : null;
                        _report.Add(new object[]
                        {
                            entry.UserDetails?.DisplayName,
                            entry.UserDetails?.EmailAddress,
                            entry.UserDetails?.Description,
                            entry.UserDetails?.PhoneNumber,
                            role,
                            projectWithRoles.Item1,
                            entry.Organization
                        });
                        roleIndex++;
                    } while (roleIndex < projectWithRoles.Item2?.Count);

                    projectIndex++;
                } while (projectIndex < entry.RolesByProject?.Count);

                ProgressChanged?.Invoke(new Progress(index, entries.Count), 2);
            }
        }

        private async Task<List<UserDetails>> GetUsers(Organization organization)
        {
            var users = await _groupshareConnection.GetAllUsers();
            if (organization != null)
            {
                users = users.Where(ud => ud.OrganizationId == organization.UniqueId.ToString()).ToList();
            }

            return users;
        }

        private struct UserEntry
        {
            public string Organization { get; set; }
            public List<(string, List<string>)> RolesByProject { get; set; }
            public UserDetails UserDetails { get; set; }
        }
    }
}