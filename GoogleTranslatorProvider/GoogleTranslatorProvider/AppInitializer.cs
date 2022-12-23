using System;
using GoogleTranslatorProvider.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace GoogleTranslatorProvider
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