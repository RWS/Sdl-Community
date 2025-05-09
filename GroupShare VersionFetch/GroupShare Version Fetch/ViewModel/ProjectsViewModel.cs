using Newtonsoft.Json;
using NLog;
using Sdl.Community.GSVersionFetch.Commands;
using Sdl.Community.GSVersionFetch.Events;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Interface;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sdl.Community.GSVersionFetch.ViewModel
{
    public class ProjectsViewModel : ProjectWizardViewModelBase
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly MethodInfo _getUserCredentialsMethod;
        private readonly Type _identityInfoCacheHelpers;
        private readonly IMessageBoxService _messageBoxService;
        private readonly dynamic _serverLogon;
        private readonly Utils _utils;
        private readonly WizardModel _wizardModel;
        private ICommand _clearCommand;
        private int _currentPageNumber;
        private string _displayName;
        private bool _isNextEnabled;
        private bool _isPreviousEnabled;
        private bool _isValid;
        private ICommand _newlogin;
        private ICommand _nextPageCommand;
        private ICommand _previousPageCommand;
        private ICommand _refreshProjectsCommand;
        private string _searchByProjectNameText;
        private string _textMessageVisibility;

        public ProjectsViewModel(WizardModel wizardModel, IMessageBoxService messageBoxService)
        {
            _utils = new Utils();
            _currentPageNumber = 1;
            _wizardModel = wizardModel;
            _displayName = "GroupShare Projects";
            _searchByProjectNameText = string.Empty;
            _isPreviousEnabled = false;
            _isNextEnabled = true;
            _wizardModel.GsProjects.CollectionChanged += GsProjects_CollectionChanged;
            _messageBoxService = messageBoxService;

            var assembly = Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.Desktop.Platform.ServerConnectionPlugin.dll"));
            var serverLogonType = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.ServerLogon");
            _identityInfoCacheHelpers = assembly.GetType("Sdl.Desktop.Platform.ServerConnectionPlugin.Implementation.IdentityInfoCacheHelpers");
            _getUserCredentialsMethod = _identityInfoCacheHelpers.GetMethod("GetUserCredentials");
            _serverLogon = Activator.CreateInstance(serverLogonType);

            AuthenticateUser();
        }

        public bool AllFilesChecked
        {
            get => AreAllFilesSelected();
            set
            {
                ToggleCheckAllFiles(value);
                OnPropertyChanged(nameof(AllFilesChecked));
            }
        }

        public ICommand ClearCommand => _clearCommand ?? (_clearCommand = new ParameterCommand(Clear));

        public int CurrentPageNumber
        {
            get => _currentPageNumber;
            set
            {
                _currentPageNumber = value;
                OnPropertyChanged(nameof(CurrentPageNumber));
            }
        }

        public override string DisplayName
        {
            get => _displayName;
            set
            {
                if (_displayName == value)
                {
                    return;
                }

                _displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        public ObservableCollection<GsProject> GsProjects
        {
            get => _wizardModel?.GsProjects;
            set
            {
                _wizardModel.GsProjects = value;
                OnPropertyChanged(nameof(GsProjects));
            }
        }

        public bool IsNextEnabled
        {
            get => _isNextEnabled;
            set
            {
                if (_isNextEnabled == value)
                    return;

                _isNextEnabled = value;
                OnPropertyChanged(nameof(IsNextEnabled));
            }
        }

        public bool IsPreviousEnabled
        {
            get => _isPreviousEnabled;
            set
            {
                if (_isPreviousEnabled == value)
                    return;

                _isPreviousEnabled = value;
                OnPropertyChanged(nameof(IsPreviousEnabled));
            }
        }

        public override bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid == value)
                    return;

                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        public ICommand NewLoginCommand => _newlogin ?? (_newlogin = new CommandHandler(NewLogin, true));

        public ICommand NextPageCommand => _nextPageCommand ?? (_nextPageCommand = new AwaitableDelegateCommand(DisplayNextPage));

        public ObservableCollection<OrganizationResponse> Organizations
        {
            get => _wizardModel?.Organizations;
            set
            {
                if (_wizardModel.Organizations == value)
                {
                    return;
                }
                _wizardModel.Organizations = value;
                OnPropertyChanged(nameof(Organizations));
            }
        }

        public int PagesNumber
        {
            get => _wizardModel.TotalPages;
            set
            {
                _wizardModel.TotalPages = value;
                OnPropertyChanged(nameof(PagesNumber));
            }
        }

        public ICommand PreviousPageCommand => _previousPageCommand ?? (_previousPageCommand = new AwaitableDelegateCommand(DisplayPreviousPage));

        public ObservableCollection<GsProject> ProjectsForCurrentPage
        {
            get => _wizardModel?.ProjectsForCurrentPage;
            set
            {
                _wizardModel.ProjectsForCurrentPage = value;
                OnPropertyChanged(nameof(ProjectsForCurrentPage));
            }
        }

        public int ProjectsNumber
        {
            get => _wizardModel.ProjectsNumber;
            set
            {
                if (_wizardModel?.ProjectsNumber == value)
                    return;
                if (_wizardModel != null)
                {
                    _wizardModel.ProjectsNumber = value;
                }
                OnPropertyChanged(nameof(ProjectsNumber));
            }
        }

        public ICommand RefreshProjectsCommand => _refreshProjectsCommand ?? (_refreshProjectsCommand = new AwaitableDelegateCommand(RefreshProjects));

        public string SearchByProjectNameText
        {
            get => _searchByProjectNameText;
            set
            {
                _searchByProjectNameText = value;
                OnPropertyChanged(SearchByProjectNameText);
                _wizardModel?.GsProjects?.Clear();
                _wizardModel?.ProjectsForCurrentPage?.Clear();
                LoadProjectsForCurrentPage();
            }
        }

        public OrganizationResponse SelectedOrganization
        {
            get => _wizardModel?.SelectedOrganization;
            set
            {
                if (_wizardModel != null)
                {
                    _wizardModel?.GsProjects?.Clear();
                    _wizardModel?.ProjectsForCurrentPage?.Clear();
                    _wizardModel.SelectedOrganization = value;

                    LoadProjectsForCurrentPage();
                }
                OnPropertyChanged(nameof(SelectedOrganization));
            }
        }

        public string TextMessageVisibility
        {
            get => _textMessageVisibility;
            set
            {
                _textMessageVisibility = value;
                OnPropertyChanged(nameof(TextMessageVisibility));
            }
        }

        public Credentials GetCredentials(dynamic source = null)
        {
            var credentialType = new CredentialType();
            if (source == null)
            {
                source = _serverLogon;
            }
            else
            {
                credentialType = MapCredentialFromStudio(source.UserType?.ToString());
            }

            var ssoData = GetSsoData(source);
            var credentials = new Credentials
            {
                CredentialType = credentialType,
                UserName = source.UserName,
                Password = source.Password,
                SsoCredentials = ssoData
            };
            return credentials;
        }

        public override bool OnChangePage(int position, out string message)
        {
            message = string.Empty;

            var pagePosition = PageIndex - 1;
            if (position == pagePosition)
            {
                return false;
            }

            if (!IsValid && position > pagePosition)
            {
                message = "Please select at least one project before moving to next page";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Studio location
        /// </summary>
        /// <returns></returns>
        private static string ExecutingStudioLocation()
        {
            var entryAssembly = Assembly.GetEntryAssembly()?.Location;
            var location = entryAssembly?.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

            return location;
        }

        private static void GetWindowsSsoData(string serviceUriString, Credentials credentials)
        {
            if (credentials.CredentialType == CredentialType.WindowsSSO)
            {
                credentials.WindowsSsoCredentials = new WindowsSsoData(serviceUriString);
            }
        }

        private static void ShowProgress(string message = null, bool showRing = false)
        {
            AppInitializer.PublishEvent(new ProgressEvent(message, showRing));
        }

        private bool AreAllFilesSelected()
        {
            return ProjectsForCurrentPage?.Count > 0 && ProjectsForCurrentPage.All(f => f.IsSelected);
        }

        private async void AuthenticateUser(bool showLogon = false)
        {
            try
            {
                var credentials = GetIdentity(showLogon, out var cancelled);

                if (cancelled)
                {
                    return;
                }

                _wizardModel.UserCredentials = credentials;

                var statusCode = await SetCredentials();

                ClearView();

                if (statusCode == HttpStatusCode.OK)
                {
                    _utils.SetUserDetails(_wizardModel.UserCredentials);
                    await RetrieveData();
                }
                else
                {
                    InformUser(statusCode);
                    ClearView();
                    Authentication.Token = string.Empty;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"AuthenticateUser method: {e.Message}\n {e.StackTrace}");
            }
        }

        private void Clear(object obj)
        {
            if (!(obj is string objectName)) return;

            switch (objectName)
            {
                case "SearchByProjectName":
                    SearchByProjectNameText = string.Empty;
                    break;
            }
        }

        private void ClearView()
        {
            _wizardModel?.GsProjects?.Clear();
            _wizardModel?.ProjectsForCurrentPage?.Clear();
            _wizardModel?.Organizations?.Clear();
        }

        private async Task DisplayNextPage()
        {
            CurrentPageNumber++;
            await GetProjects();
        }

        private async Task DisplayPreviousPage()
        {
            CurrentPageNumber--;
            await GetProjects();
        }

        private bool ExistsProjectsForCurrentPage()
        {
            var page = CurrentPageNumber - 1;
            var projectsList = _wizardModel?.GsProjects.Skip(page * 50).Take(50).ToList();

            if (projectsList?.Count > 0 && _wizardModel != null)
            {
                foreach (var project in projectsList)
                {
                    _wizardModel.ProjectsForCurrentPage.Add(project);
                }
                return true;
            }
            return false;
        }

        private Credentials GetCachedCredentials(string serviceUriString)
        {
            var serviceUri = !string.IsNullOrEmpty(serviceUriString) ? new Uri(serviceUriString) : null;

            if (serviceUri == null)
                return null;

            var cachedCredentials = GetDataFromCache(serviceUri);

            if (cachedCredentials == null)
                return null;

            Credentials credentials = GetCredentials(cachedCredentials);

            GetWindowsSsoData(serviceUriString, credentials);
            credentials.ServiceUrl = serviceUriString;
            return credentials;
        }

        private dynamic GetDataFromCache(Uri serviceUri)
        {
            dynamic cachedCredentials =
                _getUserCredentialsMethod.Invoke(_identityInfoCacheHelpers, new object[] { serviceUri });
            return cachedCredentials;
        }

        private Credentials GetIdentity(bool showLogon, out bool cancelled)
        {
            cancelled = false;
            var credentials = _utils.GetStoredUserDetails();
            credentials = GetCachedCredentials(credentials?.ServiceUrl);
            if (!showLogon && (!string.IsNullOrEmpty(credentials?.Password)
                               || !string.IsNullOrEmpty(credentials?.SsoCredentials?.AuthToken)
                               || (credentials?.CredentialType) == CredentialType.WindowsSSO))
            {
                return credentials;
            }

            _serverLogon.ServerUrl = credentials?.ServiceUrl;
            credentials = GetUserInput(out cancelled);
            if (credentials?.SsoCredentials is not null)
            {
                return credentials;
            }

            var data = GetDataFromCache(new Uri(credentials?.ServiceUrl));
            credentials.SsoCredentials = new SsoData(JsonConvert.SerializeObject(data?.SamlToken), data?.AuthToken);
            return credentials;
        }

        private async Task GetProjects()
        {
            _wizardModel?.ProjectsForCurrentPage.Clear();

            if (!ExistsProjectsForCurrentPage())
            {
                await LoadProjectsForCurrentPage();
            }
            else
            {
                UpdateNavigationButtons();
            }
        }

        private SsoData GetSsoData(dynamic source)
        {
            SsoData ssoData = null;
            if (PropertyExists(source, "SsoCredentials") && source.SsoCredentials?.SamlToken != null &&
                source.SsoCredentials?.AuthToken != null)
            {
                ssoData = new SsoData(source.SsoCredentials.SamlToken, source.SsoCredentials.AuthToken);
            }

            return ssoData;
        }

        public static bool PropertyExists(dynamic obj, string name)
        {
            if (obj == null) return false;
            if (obj is IDictionary<string, object> dict)
            {
                return dict.ContainsKey(name);
            }
            return obj.GetType().GetProperty(name) != null;
        }

        private Credentials GetUserInput(out bool cancelled)
        {
            cancelled = !_serverLogon.ShowLogonDialog(null);
            if (cancelled) return null;

            var credentials = GetCredentials();
            credentials.DetermineCredentialType(_serverLogon.IsWindowsUser);

            credentials.ServiceUrl = _serverLogon.ServerUrl;
            GetWindowsSsoData(credentials.ServiceUrl, credentials);
            return credentials;
        }

        private void GsProject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsSelected"))
            {
                IsValid = _wizardModel.GsProjects.Any(p => p.IsSelected);
            }
        }

        private void GsProjects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (GsProject gsProject in e.OldItems)
                {
                    gsProject.PropertyChanged -= GsProject_PropertyChanged;
                }
            }

            if (e.NewItems == null) return;
            foreach (GsProject project in e.NewItems)
            {
                project.PropertyChanged += GsProject_PropertyChanged;
            }
        }

        private void InformUser(HttpStatusCode statusCode)
        {
            var message = statusCode == HttpStatusCode.Unauthorized ? PluginResources.InvalidCredentials : statusCode.ToString();
            _messageBoxService.ShowWarningMessage(message, PluginResources.InvalidCredentials_Title);
        }

        private async Task LoadProjectsForCurrentPage()
        {
            try
            {
                var filter = new ProjectFilter
                {
                    Filter = new Filter
                    {
                        ProjectName = SearchByProjectNameText,
                        OrgPath = "/",
                        Status = 7,
                        IncludeSubOrgs = true
                    },
                    PageSize = 50,
                    Page = CurrentPageNumber
                };
                if (SelectedOrganization != null)
                {
                    filter.Filter.OrgPath = SelectedOrganization.Path;
                }

                await SetGsProjectsToWizard(filter);

                IsValid = false;
                UpdateNavigationButtons();
                OnPropertyChanged(nameof(ProjectsNumber));
                OnPropertyChanged(nameof(PagesNumber));

                //if (ProjectsNumber == 0) Te
            }
            catch (Exception e)
            {
                Logger.Error($"RefreshProjects method: {e.Message}\n {e.StackTrace}");
            }
        }

        private CredentialType MapCredentialFromStudio(string credentialTypeFromStudio)
        {
            var credentialType = CredentialType.Unknown;
            switch (credentialTypeFromStudio)
            {
                case "CustomUser":
                case "CustomWindowsUser":
                    credentialType = CredentialType.Normal;
                    break;

                case "Saml2User":
                    credentialType = CredentialType.SSO;
                    break;

                case "WindowsUser":
                    credentialType = CredentialType.WindowsSSO;
                    break;
            }

            return credentialType;
        }

        private void NewLogin()
        {
            AuthenticateUser(true);
        }

        private async Task RefreshProjects()
        {
            _wizardModel?.GsProjects?.Clear();
            _wizardModel?.ProjectsForCurrentPage?.Clear();

            await LoadProjectsForCurrentPage();
        }

        private async Task RetrieveData()
        {
            ShowProgress(PluginResources.WaitMessage_ProjectsView, true);

            var organizationService = new OrganizationService();
            var filter = new ProjectFilter
            {
                PageSize = 50,
                Page = 1
            };

            await SetGsProjectsToWizard(filter);

            var organizations = await organizationService.GetOrganizations();
            _utils.SegOrganizationsToWizard(_wizardModel,
                organizations.OrderBy(o => o.Name).ToList());
            if (organizations.Count > 0)
            {
                foreach (var organization in organizations)
                {
                    _wizardModel?.Organizations.Add(organization);
                }
            }

            if (GsProjects.Count == 0)
                ShowProgress(PluginResources.ProgressBar_ProjectsVM_NoProjectsMessage);
            else
                ShowProgress();
        }

        private async Task<HttpStatusCode> SetCredentials()
        {
            var credentialType = _wizardModel.UserCredentials.CredentialType;
            var credentials = _wizardModel.UserCredentials;

            if (credentialType == CredentialType.SSO || credentialType == CredentialType.WindowsSSO)
            {
                return await Authentication.SetCredentials(credentials);
            }

            if (credentialType == CredentialType.Normal)
            {
                return await Authentication.Login(credentials);
            }

            return new HttpStatusCode();
        }

        private async Task SetGsProjectsToWizard(ProjectFilter filter)
        {
            await _utils.SetGsProjectsToWizard(_wizardModel, filter);
            OnPropertyChanged(nameof(PagesNumber));
        }

        private void ToggleCheckAllFiles(bool value)
        {
            foreach (var gsFile in ProjectsForCurrentPage)
            {
                gsFile.IsSelected = value;
            }
        }

        private void UpdateNavigationButtons()
        {
            IsPreviousEnabled = !CurrentPageNumber.Equals(1);
            if (PagesNumber > 0)
            {
                IsNextEnabled = !CurrentPageNumber.Equals(PagesNumber);
            }
        }
    }
}