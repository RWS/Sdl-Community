using System;
using GoogleCloudTranslationProvider.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace GoogleCloudTranslationProvider
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