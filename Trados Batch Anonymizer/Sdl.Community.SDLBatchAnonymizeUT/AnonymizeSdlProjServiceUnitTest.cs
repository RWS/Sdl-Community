using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.Community.SDLBatchAnonymize.Service;
using Xunit;

namespace Sdl.Community.SDLBatchAnonymizeUT
{
	public class AnonymizeSdlProjServiceUnitTest
	{
		private readonly IAnonymizeSdlProj _anonymizeSdlProjService;
		public AnonymizeSdlProjServiceUnitTest()
		{
			_anonymizeSdlProjService = new AnonymizeSdlProjService();
		}

		[Theory]
		[InlineData(@"C:\Code\SDLCOM - Studio 2021\SDLBatchAnonymize\Sdl.Community.SDLBatchAnonymizeUT\Project 1\Project 1.sdlproj")]
		public void AnonymizeFileVersions(string projectPath)
		{
			_anonymizeSdlProjService.RemoveFileVersionComment(projectPath);
			_anonymizeSdlProjService.RemoveTraces(projectPath);
		}
	}
}
