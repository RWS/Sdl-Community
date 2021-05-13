using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StarTransit.Interface
{
	public interface IStudioService
	{
		List<ProjectTemplateInfo> GetProjectTemplates();
		Task<List<Customer>> GetCustomers();
		PackageModel GetModelBasedOnStudioTemplate(string templatePath);
	}
}
