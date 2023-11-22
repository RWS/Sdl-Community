using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.MTCloud.Provider.Model;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.MTCloud.Provider.Extensions
{
	public static class StudioProjectContextExtensions
	{
		public static TranslationProviderReference GetLwTpReference(this FileBasedProject project) => project
			.GetTranslationProviderConfiguration().Entries.FirstOrDefault(e =>
				e.MainTranslationProvider.Uri.ToString().Contains(PluginResources.SDLMTCloudUri))
			?.MainTranslationProvider;

		public static LanguageMappingModel GetCurrentLanguageMappingModel(this List<LanguageMappingModel> models)
		{
			var activeDocument = MtCloudApplicationInitializer.EditorController?.ActiveDocument;

			if (activeDocument is null)
				return null;

			var currentProject = activeDocument.Project.GetProjectInfo();

			var activeFileId = activeDocument.ActiveFile;
			if (activeFileId is null)
				return null;

			var model = models?.FirstOrDefault(l =>
				l.SourceTradosCode.Equals(currentProject.SourceLanguage.IsoAbbreviation,
					StringComparison.InvariantCultureIgnoreCase) &&
				l.TargetTradosCode.Equals(activeFileId.Language.IsoAbbreviation,
					StringComparison.InvariantCultureIgnoreCase));
			return model;
		}

		public static bool IsActiveModelQeEnabled(this List<LanguageMappingModel> models)
			=> models.GetCurrentLanguageMappingModel()?.SelectedModel.Model?.ToLower().Contains("qe") ?? false;
	}
}