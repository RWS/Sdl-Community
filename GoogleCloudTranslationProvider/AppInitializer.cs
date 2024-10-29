using System;
using System.Collections.Generic;
using GoogleCloudTranslationProvider.Helpers;
using GoogleCloudTranslationProvider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace GoogleCloudTranslationProvider
{
	[ApplicationInitializer]
	public class AppInitializer : IApplicationInitializer
	{
		public static IDictionary<string, ITranslationOptions> TranslationOptions { get; set; } = new Dictionary<string, ITranslationOptions>();	

		public void Execute()
		{
			Log.Setup();
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.CurrentDomain_AssemblyResolve;
		}
	}
}