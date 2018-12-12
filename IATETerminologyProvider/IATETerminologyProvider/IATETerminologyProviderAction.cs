using System;
using System.Collections.Generic;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace IATETerminologyProvider
{
	[RibbonGroup("IATETermDefinition", Name = "IATE Term Definition")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class IATETerminologyProviderAction: AbstractRibbonGroup
	{
		[Action("IATESearchAllAction", Name = "Search IATE (all)", Icon = "Iate_logo")]
		[ActionLayout(typeof(IATETerminologyProviderAction), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
		public class IATESearchAllAction : AbstractAction
		{
			public EditorController GetEditorController()
			{
				return SdlTradosStudio.Application.GetController<EditorController>();
			}

			// Navigate to the IATE search term based on the document source and all existing target languages(from IATE)
			protected override void Execute()
			{
				var editorController = GetEditorController();
				var activeDocument = editorController != null ? editorController.ActiveDocument : null;
				if (activeDocument != null)
				{
					var currentSelection = activeDocument.Selection != null ? activeDocument.Selection.Current.ToString().TrimEnd()	: string.Empty;
					var sourceLanguage = activeDocument.ActiveFile.SourceFile.Language.CultureInfo.TwoLetterISOLanguageName;

					if (!string.IsNullOrEmpty(currentSelection))
					{
						var url = @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=all";
						System.Diagnostics.Process.Start(url);
					}
				}
			}
		}

		[Action("IATESearchSourceTargetAction", Name = "Search IATE (source and target only)", Icon = "Iate_logo")]
		[ActionLayout(typeof(IATETerminologyProviderAction), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
		public class IATESearchSourceLanguageAction : AbstractAction
		{
			public EditorController GetEditorController()
			{
				return SdlTradosStudio.Application.GetController<EditorController>();
			}

			// Navigate to the IATE search term based on only source and target languages set on the active document
			protected override void Execute()
			{
				var editorController = GetEditorController();
				var activeDocument = editorController != null ? editorController.ActiveDocument : null;
				if (activeDocument != null)
				{
					var targetLanguages = string.Empty;
					var currentSelection = activeDocument.Selection != null ? activeDocument.Selection.Current.ToString().TrimEnd() : string.Empty;
					var sourceLanguage = activeDocument.ActiveFile.SourceFile.Language.CultureInfo.TwoLetterISOLanguageName;
					var targetFiles = activeDocument.ActiveFile.TargetFiles;

					foreach (var targetFile in targetFiles)
					{
						var targetLanguage = targetFile.Language.CultureInfo.TwoLetterISOLanguageName;
						targetLanguages += $"{targetLanguage},";
					}
										
					if (!string.IsNullOrEmpty(currentSelection))
					{
						var url = @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=" + targetLanguages.TrimEnd(',');
						System.Diagnostics.Process.Start(url);
					}
				}
			}
		}
	}
}