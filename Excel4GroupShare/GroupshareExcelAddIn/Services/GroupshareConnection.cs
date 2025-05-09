using GroupshareExcelAddIn.Helper;
using GroupshareExcelAddIn.Models;
using GroupshareExcelAddIn.Properties;
using GroupshareExcelAddIn.Services.EventHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Community.GroupShareKit;
using Sdl.Community.GroupShareKit.Models.Response;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GroupshareExcelAddIn.Interfaces;
using NLog;
using Sdl.Community.GroupShareKit.Clients;
using Language = Sdl.Community.GroupShareKit.Models.Response.Language;
using Termbase = GroupshareExcelAddIn.Models.Termbase;
using UserDetails = GroupshareExcelAddIn.Models.UserDetails;
using System.Net.Http;

namespace GroupshareExcelAddIn.Services
{
    public class GroupshareConnection : IGroupshareConnection
    {
        private readonly Logger _logger = Log.GetLogger(nameof(GroupshareConnection));

        private Dictionary<string, List<File>> _filesByProject;
        private bool _fileDataRetrievalCancelled;
        private GroupShareClient _groupShareClient;
        private bool _isUnfilteredInfo;

        public event EventHandler ConnectionChanged;

        public event ProgressChangedEventHandler ProgressChanged;

        public GroupShareClient GroupShareClient
        {
            get => _groupShareClient;
            set
            {
                _groupShareClient = value;
                ClearFields();
                ConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsConnected => GroupShareClient != null;

        private string GroupShareToken { get; set; }

        public async Task ConnectToGroupShare(Credentials credentials)
        {
            ApiUrl.BaseUrl = credentials.ServerURI;
            GroupShareToken = await GroupShareClient.GetRequestToken(credentials.UserName,
                credentials.Password,
                new Uri(credentials.ServerURI),
                GroupShareClient.AllScopes);
            GroupShareClient = await GroupShareClient.AuthenticateClient(GroupShareToken, credentials.UserName,
                credentials.Password,
                null, new Uri(credentials.ServerURI),
                GroupShareClient.AllScopes);
        }

        public void Disconnect()
        {
            GroupShareClient = null;
            GroupShareToken = null;
        }

        public async Task<Dictionary<string, List<File>>> GetAllFilesByProject(CancellationToken dataRetrievalCancellationToken)
        {
            if (_filesByProject != null && _isUnfilteredInfo && !_fileDataRetrievalCancelled)
            {
                return _filesByProject;
            }

            _filesByProject = new Dictionary<string, List<File>>();
            var projects = await GetGsProjects(ProjectFilter.NoRestrictionFilter);
            for (var index = 0; index < projects.Count && !dataRetrievalCancellationToken.IsCancellationRequested; index++)
            {
                ProgressChanged?.Invoke(new Progress(index + 1, projects.Count), 1);
                var files = await GetProjectFiles(projects[index]);
                AddFilesToDictionary(projects[index].Name, files);
            }
            _fileDataRetrievalCancelled = dataRetrievalCancellationToken.IsCancellationRequested;

            return _filesByProject;
        }

        public async Task<List<ResourceType>> GetResourceTypes()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.DefinitionsUrl));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);
                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var resourcesResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JObject.Parse(resourcesResponse)["resourceTypes"].ToObject<List<ResourceType>>();
                    }

                    _logger.Error($"Get All Organizations StatusCode:{responseMessage.StatusCode}");
                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"GetAllOrganizations service method: {e.Message}\n {e.StackTrace}");
            }

            return new List<ResourceType>();
        }

        private async Task<List<Organization>> GetAllOrganizations(bool flatten)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var queryString = ApiUrl.GetOrganizationsQueryString(flatten);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(queryString));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var organizationResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<List<Organization>>(organizationResponse);
                    }

                    _logger.Error($"Get All Organizations StatusCode:{responseMessage.StatusCode}");
                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"GetAllOrganizations service method: {e.Message}\n {e.StackTrace}");
            }

            //TODO: return empty lists everywhere instead of null
            return new List<Organization>();
        }

        public void GetAllSubOrganizations(Organization organization, List<Organization> listOfOrganizations)
        {
            foreach (var org in organization.ChildOrganizations)
            {
                listOfOrganizations.Add(org);
                if (org.ChildOrganizations.Count > 0)
                {
                    GetAllSubOrganizations(org, listOfOrganizations);
                }
            }
        }

        public async Task<List<UserDetails>> GetAllUsers()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.UsersUrl));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var userResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<List<UserDetails>>(userResponse);
                    }

                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"GetAllUsers service method: {e.Message}\n {e.StackTrace}");
            }

            return new List<UserDetails>();
        }

        public async Task<List<CustomField>> GetCustomFields(ResourceFilter filter)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var queryString = ApiUrl.GetCustomFieldsQueryString(filter);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(queryString));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var customFieldsResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JObject.Parse(customFieldsResponse)["items"].ToObject<List<CustomField>>();
                    }
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetCustomFields service method: {ex.Message}\n{ex.StackTrace}");
            }

            return new List<CustomField>();
        }

        public async Task<List<ProjectDetailsResponse>> GetGsProjects(ProjectFilter filter)
        {
            //TODO: Move all data retrieval and filtering to GSConnection for all controls
            var noOfPages = 0;
            var ind = 0;

            //this is done as a way of ensuring that all the projects are taken from GS so we can determine whether we have all files in the dictionary
            //finally, to be able to reuse this info in UserData form and not retake it, since it is a very slow call
            CheckIfUnfiltered(filter);

            var projects = new List<ProjectDetailsResponse>();
            do
            {
                var projectResponse = await GetGsProjects(new ProjectQuery
                {
                    Page = ind + 1,
                    StartItem = ind * 1000,
                    PageSize = 1000,
                    Filter = filter
                }).ConfigureAwait(false);

                if (ind < 1)
                {
                    var noOfProjects = projectResponse.Count;
                    noOfPages = noOfProjects / 1000 + (noOfProjects % 1000 > 0 ? 1 : 0);
                    projects = new List<ProjectDetailsResponse>(noOfProjects);
                }

                if (projectResponse?.Items != null)
                {
                    projects.AddRange(projectResponse.Items);
                }

                ind++;
            } while (ind < noOfPages);

            return projects;
        }

        private async Task<ProjectResponse> GetGsProjects(ProjectQuery projectFilter)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var queryString = ApiUrl.GetProjectsQueryString(projectFilter);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(queryString));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var projectsResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<ProjectResponse>(projectsResponse);
                    }

                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"GetGsProjects service method: {e.Message}\n {e.StackTrace}");
            }

            return new ProjectResponse();
        }

        public async Task<TranslationMemory> GetGsTms(ResourceFilter filter)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var queryString = ApiUrl.GetTmsQueryString(filter);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(queryString));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var tmsResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JsonConvert.DeserializeObject<TranslationMemory>(tmsResponse);
                    }

                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"GetGsTms service method: {e.Message}\n {e.StackTrace}");
            }

            return new TranslationMemory();
        }

        public async Task<List<Language>> GetLanguages()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.LanguagesUrl));

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var languageResponse = await responseMessage.Content.ReadAsStringAsync();

                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        var dictionaryOfLanguages = JObject.Parse(languageResponse)["SDL.common.data.Languages"]["i18n"].ToObject<Dictionary<string, string>>().Where(entry => entry.Key.Contains('-'));
                        var listOfLanguages = dictionaryOfLanguages.Select(entry => new Language
                        {
                            Code = entry.Key,
                            Name = entry.Value
                        });

                        return listOfLanguages.ToList();
                    }

                    throw new HttpRequestException(responseMessage.StatusCode.ToString());
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.Error($"GetGsProjects service method: {e.Message}\n {e.StackTrace}");
            }

            return new List<Language>();
        }

        public async Task<List<File>> GetProjectFiles(ProjectDetailsResponse project)
        {
            var files = new List<File>();
            try
            {
                files =
                    (await GroupShareClient.Project.GetAllFilesForProject(project.ProjectId)).ToList();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    string.Format(Resources.We_couldn_t_get_the_files_for_project__0__with_ID___1_, project.Name,
                        project.ProjectId), Resources.Files_retrieval, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return files;
        }

        private List<Organization> _organizations;
        private List<Organization> _organizationsFlattened;

        public async Task<List<Organization>> GetOrganizations(bool flatten = false)
        {
            return flatten == false
                ? _organizations ?? (_organizations = await GetAllOrganizations(false))
                : _organizationsFlattened ??
                  (_organizationsFlattened = await GetAllOrganizations(true));
        }

        private void ClearFields()
        {
            _filesByProject = null;
            _organizations = null;
            _organizationsFlattened = null;
        }

        public async Task<List<Termbase>> GetTermbases(ResourceFilter filter, CancellationToken dataRetrievalCancellationToken)
        {
            if (filter == null)
            {
                filter = new ResourceFilter();
            }

            var termbases = new List<Termbase>();
            var organizations = new List<Organization>();
            if (filter.Organization == null)
            {
                organizations = await GetOrganizations(true);
            }
            else
            {
                organizations.Add(filter.Organization);
            }

            for (var index = 0; index < organizations.Count && !dataRetrievalCancellationToken.IsCancellationRequested; index++)
            {
                filter.Organization = organizations[index];
                termbases.AddRange(await GetTermbasesOfOrganization(filter));
                ProgressChanged?.Invoke(new Progress(index+1, organizations.Count),1);
            }

            return termbases;
        }

        private async Task<List<Termbase>> GetTermbasesOfOrganization(ResourceFilter filter)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    var queryString = ApiUrl.GetTermbasesQueryString(filter);
                    var request = new HttpRequestMessage(HttpMethod.Get, new Uri(queryString));
                    ApiUrl.AddRequestHeaders(httpClient, request, GroupShareToken);

                    var responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
                    var termbaseResponse = await responseMessage.Content.ReadAsStringAsync();
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        return JObject.Parse(termbaseResponse)["Items"].ToObject<List<Termbase>>();
                    }
                }
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"GetTermbases service method: {ex.Message}\n{ex.StackTrace}");
            }

            return new List<Termbase>();
        }

        private void AddFilesToDictionary(string projectId, List<File> files)
        {
            try
            {
                _filesByProject.Add(projectId, files);
            }
            catch { }
        }

        private void CheckIfUnfiltered(ProjectFilter filter)
        {
            _isUnfilteredInfo = filter.IsUnrestrictive;
        }
    }
}