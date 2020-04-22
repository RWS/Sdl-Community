using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.SDLBatchAnonymize.Interface;
using Sdl.Community.SDLBatchAnonymize.Service;
using Sdl.Core.Globalization;
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
		[InlineData(@"C:\Users\aghisa\Documents\Studio 2019\Projects\AnonDel\AnonDel.sdlproj")]
		public void AnonymizeFileVersions(string projectPath)
		{
			_anonymizeSdlProjService.RemoveFileVersionComment(projectPath);
			_anonymizeSdlProjService.RemoveTemplateId(projectPath);
		}
	}
}
