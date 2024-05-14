using System;
using System.Collections.Generic;
using MicrosoftTranslatorProvider.Helpers;
using MicrosoftTranslatorProvider.Interfaces;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace MicrosoftTranslatorProvider
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
		public static ITranslationProviderCredentialStore CredentialStore { get; set; }

		public static IDictionary<string, ITranslationOptions> TranslationOptions { get; set; }

		public void Execute()
		{
			Log.Setup();
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolver.CurrentDomain_AssemblyResolve;
			TranslationOptions = new Dictionary<string, ITranslationOptions>();
		}
	}
}