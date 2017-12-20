using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.AutomaticTasks;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.AdaptiveMT
{
	[Action("AdaptiveMt",
		Name = "Adaptive MT Training",
		Description = "Adaptive MT Training"
		)]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 2, DisplayType.Default, "", true)]
	public class AdaptiveMtRibbon: AbstractAction
	{
		public ProjectsController GetProjectsController()
		{
			return SdlTradosStudio.Application.GetController<ProjectsController>();
		}
		private Document ActiveDocument { get; set; }
		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}
		protected override void Execute()
		{
			var editorController = GetEditorController();
			
			var projects = GetProjectsController().SelectedProjects;
			
			foreach (var project in projects)
			{
				var providerExist = false;
				var provider = project.GetTranslationProviderConfiguration();
				//foreach (var entry in provider.Entries)
				//{
				//	if (entry.MainTranslationProvider.Enabled && entry.MainTranslationProvider.Uri.AbsoluteUri.Contains("bmslanguagecloud"))
				//	{
				//		providerExist = true;
				//	}
				//}
				//if (providerExist)
				//{

				//}
				var files = project.GetTargetLanguageFiles();

				foreach (var file in files)
				{
					var document = editorController.Open(file.LocalFilePath);
					var segmentPairs = document.SegmentPairs.ToList();
					foreach (var segmentPair in segmentPairs)
					{
						if (segmentPair.Target.ToString() != string.Empty)
						{
							segmentPair.Properties.ConfirmationLevel = ConfirmationLevel.Translated;
							editorController.ActiveDocument.UpdateSegmentPairProperties(segmentPair, segmentPair.Properties);
						}

					}
				}
			}
			
		}
	}
}
