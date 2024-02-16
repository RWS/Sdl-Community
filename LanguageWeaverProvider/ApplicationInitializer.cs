﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LanguageWeaverProvider.Model;
using LanguageWeaverProvider.Model.Interface;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace LanguageWeaverProvider
{
	[ApplicationInitializer]
	public class ApplicationInitializer : IApplicationInitializer
	{
		const string BatchProcessing = "batch processing";
		const string CreateNewProject = "create a new project";

		public static IList<RatedSegment> RatedSegments { get; set; }

		public static ITranslationProviderCredentialStore CredentialStore { get; set; }

		public static IDictionary<string, ITranslationOptions> TranslationOptions { get; set; }

		public void Execute()
		{
			RatedSegments = new List<RatedSegment>();
			TranslationOptions = new Dictionary<string, ITranslationOptions>();
		}

		public static Window GetBatchTaskWindow()
		{
			return Application
				.Current
				.Windows
				.Cast<Window>()
				.FirstOrDefault(window => window.Title.ToLower() == BatchProcessing
									   || window.Title.ToLower().Contains(CreateNewProject));
		}
	}
}