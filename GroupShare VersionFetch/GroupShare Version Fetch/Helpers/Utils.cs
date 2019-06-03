using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;

namespace Sdl.Community.GSVersionFetch.Helpers
{
    public class Utils
    {
	    private readonly ProjectService _projectService;

	    public Utils()
	    {
		    _projectService = new ProjectService();
	    }

	    public async Task SetGsProjectsToWizard(WizardModel wizardModel, ProjectFilter projectFilter)
	    {
			try
			{
				var languageFlagsHelper = new LanguageFlags();

				var projectsResponse = await _projectService.GetGsProjects(projectFilter);
				if (projectsResponse?.Items != null)
				{
					wizardModel.ProjectsNumber = projectsResponse.Count;
					wizardModel.TotalPages = (projectsResponse.Count + projectFilter.PageSize - 1) / projectFilter.PageSize;

					foreach (var project in projectsResponse.Items)
					{
						var gsProject = new GsProject
						{
							Name = project.Name,
							DueDate = project.DueDate?.ToString(),
							Image = new Language(project.SourceLanguage).GetFlagImage(),
							TargetLanguageFlags = languageFlagsHelper.GetTargetLanguageFlags(project.TargetLanguage),
							ProjectId = project.ProjectId,
							SourceLanguage = project.SourceLanguage
						};

						if (Enum.TryParse<ProjectStatus.Status>(project.Status.ToString(), out _))
						{
							gsProject.Status = Enum.Parse(typeof(ProjectStatus.Status), project.Status.ToString()).ToString();
						}
						wizardModel.GsProjects?.Add(gsProject);
						wizardModel.ProjectsForCurrentPage?.Add(gsProject);
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"RefreshProjects method: {e.Message}\n {e.StackTrace}");
			}
		}

	    public void SegOrganizationsToWizard(WizardModel wizardModel, List<OrganizationResponse> organizations)
	    {
			//   var treeViewOrganization = new List<OrganizationHierarchy>();
			//   var parentOrganizations = organizations.Where(o => o.IsLibrary == false);

			//   var groupedOrganizationsById = parentOrganizations.GroupBy(p => p.ParentOrganizationId);
			//   foreach (var organizationGroup in groupedOrganizationsById)
			//   {
			//    var organizationId = organizationGroup.Key;
			//    var organizationName = organizations.FirstOrDefault(o => o.UniqueId.Equals(organizationId))?.Name;
			//	var chlidsList = new List<OrganizationHierarchy>();
			//    foreach (var organization in organizationGroup)
			//    {
			//	    var childOrg = new OrganizationHierarchy(organization.Name, null);
			//		chlidsList.Add(childOrg);
			//    }
			//	var parentOrg = new OrganizationHierarchy(organizationName,chlidsList);
			//    treeViewOrganization.Add(parentOrg);
			//}
			//wizardModel.OrganizationsTreeView.AddRange(treeViewOrganization);
		    var items13 = new List<OrganizationHierarchy>{
			    new OrganizationHierarchy("Item 1.3.1", null),
			    new OrganizationHierarchy("Item 1.3.2", null)};

		    var items1 = new List<OrganizationHierarchy>{
			    new OrganizationHierarchy("Item 1.1", null),
			    new OrganizationHierarchy("Item 1.2", null),
			    new OrganizationHierarchy("Item 1.3", items13)};
		    var items2 = new List<OrganizationHierarchy>{
			    new OrganizationHierarchy("Item 2.1", null),
			    new OrganizationHierarchy("Item 2.2", null)};

		    var outerItems = new List<OrganizationHierarchy>{
			    new OrganizationHierarchy("Item 1", items1),
			    new OrganizationHierarchy("Item 2", items2)};

			     wizardModel.OrganizationsTreeView.AddRange(outerItems);
		}
    }
}
