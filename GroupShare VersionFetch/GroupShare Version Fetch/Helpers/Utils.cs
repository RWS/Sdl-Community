using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Sdl.Core.Globalization;

namespace Sdl.Community.GSVersionFetch.Helpers
{
    public class Utils
    {
	    private readonly ProjectService _projectService;
	    public readonly string SettingsFolderPath = $@"C:\Users\{Environment.UserName}\AppData\Roaming\SDL Community\GSVFSettings";
	    public readonly string JsonFileName = "GsvfSettings.json";


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

						var projectExistInWizard = wizardModel.GsProjects.FirstOrDefault(p => p.ProjectId.Equals(gsProject.ProjectId));
						if (projectExistInWizard == null)
						{
							wizardModel.GsProjects?.Add(gsProject);
						}
						var projectExistInCurrentPage = wizardModel.ProjectsForCurrentPage.FirstOrDefault(p => p.ProjectId.Equals(gsProject.ProjectId));
						if (projectExistInCurrentPage == null)
						{
							wizardModel.ProjectsForCurrentPage?.Add(gsProject);
						}
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
		    foreach (var organization in organizations)
		    {
			    wizardModel.Organizations.Add(organization);
		    }
	    }

	    public void SetUserDetails(Credentials credentials)
	    {
		    if (!Directory.Exists(SettingsFolderPath))
		    {
			    Directory.CreateDirectory(SettingsFolderPath);
		    }
		    var docPath = Path.Combine(SettingsFolderPath, JsonFileName);
		    var jsonResult = JsonConvert.SerializeObject(credentials);

		    if (File.Exists(docPath))
		    {
			    File.Delete(docPath);
		    }
		    File.Create(docPath).Dispose();

		    using (var tw = new StreamWriter(docPath, true))
		    {
			    tw.WriteLine(jsonResult);
			    tw.Close();
		    }
		}
	    public Credentials GetStoredUserDetails()
	    {
		    var docPath = Path.Combine(SettingsFolderPath, JsonFileName);
			if (File.Exists(docPath))
		    {
			    using (var r = new StreamReader(docPath))
			    {
				    var json = r.ReadToEnd();
				    var credentials = JsonConvert.DeserializeObject<Credentials>(json);
				    return credentials;
			    }
		    }
		    return null;
	    }
	}
}
