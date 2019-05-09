using System.Threading.Tasks;
using Sdl.Community.GSVersionFetch.Service;
using Xunit;

namespace Sdl.Community.GSVersionFetch.Tests
{
	public class FilesTests
	{
		private readonly AuthenticationService _authentication = new AuthenticationService();
		private readonly ProjectService _projectService = new ProjectService();

		[Theory]
		[InlineData("c7aada55-c09d-437c-8565-a7548e39ee7d")]
		public async Task GetProjectFile(string projectId)
		{
			await _authentication.Login();
			var projectFiles = await _projectService.GetProjectFiles(projectId);
			Assert.True(projectFiles != null);
		}

		[Theory]
		[InlineData("f6014cdc-0956-462d-9381-7c01c489747e")]
		public async Task GetFileVersions(string projectId)
		{
			await _authentication.Login();
			var fileVersions = await _projectService.GetFileVersion(projectId);
			Assert.True(fileVersions != null);
		}
	}
}
