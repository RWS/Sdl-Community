using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Interface
{
	public interface IStudioService
	{
		ProjectsController GetProjectController();
		List<ProjectTemplateInfo> GetProjectTemplates();
		Task<List<Customer>> GetCustomers();
	}
}
