using GroupshareExcelAddIn.Models;
using Sdl.Community.GroupShareKit;
using Sdl.Community.GroupShareKit.Models.Response;
using Sdl.Community.GroupShareKit.Models.Response.TranslationMemory;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GroupshareExcelAddIn.Services.EventHandlers;
using Termbase = GroupshareExcelAddIn.Models.Termbase;
using UserDetails = GroupshareExcelAddIn.Models.UserDetails;

namespace GroupshareExcelAddIn.Interfaces
{
    public interface IGroupshareConnection
    {
        event EventHandler ConnectionChanged;

        GroupShareClient GroupShareClient { get; set; }

        bool IsConnected { get; }

        Task ConnectToGroupShare(Credentials credentials);

        void Disconnect();

        void GetAllSubOrganizations(Organization organization, List<Organization> listOfOrganizations);

        Task<List<CustomField>> GetCustomFields(ResourceFilter filter);

        Task<List<ProjectDetailsResponse>> GetGsProjects(ProjectFilter filter);

        Task<TranslationMemory> GetGsTms(ResourceFilter filter);

        Task<List<File>> GetProjectFiles(ProjectDetailsResponse project);

        Task<List<Organization>> GetOrganizations(bool flatten = false);

        Task<List<Termbase>> GetTermbases(ResourceFilter filter, CancellationToken cancellationToken);

        Task<List<Language>> GetLanguages();
        event ProgressChangedEventHandler ProgressChanged;
        Task<Dictionary<string, List<File>>> GetAllFilesByProject(CancellationToken dataRetrievalCancellationToken);
        Task<List<UserDetails>> GetAllUsers();
        Task<List<ResourceType>> GetResourceTypes();
    }
}