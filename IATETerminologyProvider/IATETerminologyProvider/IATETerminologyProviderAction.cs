using System;
using System.Windows;
using IATETerminologyProvider.Helpers;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace IATETerminologyProvider
{
	[RibbonGroup("IATETermDefinition", Name = "IATE Term Definition")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultViews.TradosStudioViewsLocation))]
	public class IATETerminologyProviderAction: AbstractRibbonGroup
	{
		public static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		[Action("IATESearchAllAction", Name = "Search IATE (all)", Icon = "Iate_logo")]
		[ActionLayout(typeof(IATETerminologyProviderAction), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
		public class IATESearchAllAction : AbstractAction
		{
			// Navigate to the IATE search term based on the document source and all existing target languages(from IATE)
			protected override void Execute()
			{
				var editorController = GetEditorController();
				var activeDocument = editorController != null ? editorController.ActiveDocument : null;
				if (activeDocument != null)
				{
					var url = string.Empty;
					var currentSelection = activeDocument.Selection != null ? activeDocument.Selection.Current.ToString().TrimEnd()	: string.Empty;
					var sourceLanguage = activeDocument.ActiveFile.SourceFile.Language.CultureInfo.TwoLetterISOLanguageName;

					if (!string.IsNullOrEmpty(currentSelection))
					{
						if (activeDocument.FocusedDocumentContent.Equals(FocusedDocumentContent.Target))
						{
							// set the URL source language from the target segment of the editor
							sourceLanguage = activeDocument.ActiveFile.Language.CultureInfo.TwoLetterISOLanguageName;
							url = @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=all";
						}
						else
						{
							// set the language from the source side of the editor
							url = @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=all";
						}
						System.Diagnostics.Process.Start(url);
					}
					else
					{
						MessageBox.Show(Constants.NoTermSelected, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
		}

		[Action("IATESearchSourceTargetAction", Name = "Search IATE (source and target only)", Icon = "Iate_logo")]
		[ActionLayout(typeof(IATETerminologyProviderAction), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
		public class IATESearchSourceLanguageAction : AbstractAction
		{	
			// Navigate to the IATE search term based on only source and target languages set on the active document
			protected override void Execute()
			{
				var editorController = GetEditorController();
				var activeDocument = editorController != null ? editorController.ActiveDocument : null;
				if (activeDocument != null)
				{
					var url = string.Empty;
					var currentSelection = activeDocument.Selection != null ? activeDocument.Selection.Current.ToString().TrimEnd() : string.Empty;
					var activeFile = activeDocument.ActiveFile;
					if (activeFile != null && !string.IsNullOrEmpty(currentSelection))
					{
						var sourceLanguage = activeFile.SourceFile.Language.CultureInfo.TwoLetterISOLanguageName;
						var targetLanguage = activeFile.Language.CultureInfo.TwoLetterISOLanguageName;

						if (!string.IsNullOrEmpty(currentSelection))
						{
							if (activeDocument.FocusedDocumentContent.Equals(FocusedDocumentContent.Target))
							{
								// inverse the languages in case user wants to navigate to a term/phrase selected from the target segment
								url = @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + targetLanguage + "&tl=" + sourceLanguage;
							}
							else
							{
								// set the language from the source side of the editor
								url = @"http://iate.europa.eu/search/byUrl?term=" + currentSelection + "&sl=" + sourceLanguage + "&tl=" + targetLanguage;
							}
							System.Diagnostics.Process.Start(url);
						}
					}
					else
					{
						MessageBox.Show(Constants.NoTermSelected, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
					}
				}
			}
		}
	}
}