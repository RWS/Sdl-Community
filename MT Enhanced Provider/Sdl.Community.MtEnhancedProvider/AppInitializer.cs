using System;
using Sdl.Community.MtEnhancedProvider.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.MtEnhancedProvider
{
	[ApplicationInitializer]
	public class AppInitializer : IApplicationInitializer
	{
		public void Execute()
		{
			Log.Setup();
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.CurrentDomain_AssemblyResolve;
		}
	}
}