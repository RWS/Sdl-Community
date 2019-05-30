using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using Xunit;

namespace Sdl.Community.GSVersionFetch.Tests
{
	public class ProjectsTests
	{
		private readonly AuthenticationService _authentication = new AuthenticationService();
		private readonly  ProjectService _projectService = new ProjectService();

		[Fact]
		public async Task GetProjects()
		{
			await _authentication.Login().ConfigureAwait(true);
			var projectFilter = new ProjectFilter
			{
				Page = 1
			};
			var gsProjects = await _projectService.GetGsProjects(projectFilter);
			Assert.True(gsProjects!=null);
		}
	}
}
