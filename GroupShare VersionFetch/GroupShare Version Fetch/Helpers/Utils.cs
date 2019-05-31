using System;
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
    }
}
