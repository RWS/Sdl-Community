using System;
using System.Windows;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Ui;
using Sdl.Core.PluginFramework;
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
        private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		private static SearchResultsViewerController GetSearchResultsController()
		{
			return SdlTradosStudio.Application.GetController<SearchResultsViewerController>();
		}

		/// <summary>
		/// Construct URL based on the term/phrase selection using language pairs either from source segment, or from target segment.
		/// </summary>
		/// <param name="isSearchAll"></param>
		public static void NavigateToIATE(bool isSearchAll)
		{
			var editorController = GetEditorController();
			var activeDocument = editorController?.ActiveDocument;
			if (activeDocument != null)
			{
				var currentSelection = activeDocument.Selection?.Current.ToString().TrimEnd() ?? string.Empty;
				var activeFile = activeDocument.ActiveFile;
				if (activeFile != null && !string.IsNullOrEmpty(currentSelection))
				{
					var sourceLanguage = activeFile.SourceFile.Language.CultureInfo.TwoLetterISOLanguageName;
					var targetLanguage = activeFile.Language.CultureInfo.TwoLetterISOLanguageName;

					string url;
					if (activeDocument.FocusedDocumentContent.Equals(FocusedDocumentContent.Target))
					{
						// inverse the languages in case user wants to navigate to a term/phrase selected from the target segment
						url = isSearchAll
							? ApiUrls.SearchAllUri(currentSelection, targetLanguage)
							: ApiUrls.SearchSourceTargetUri(currentSelection, targetLanguage, sourceLanguage);
					}
					else
					{
						// set the language from the source side of the editor
						url = isSearchAll
							? ApiUrls.SearchAllUri(currentSelection, sourceLanguage)
							: ApiUrls.SearchSourceTargetUri(currentSelection, sourceLanguage, targetLanguage);
					}

					ShowBrowserResults(url);
				}
				else
				{
					MessageBox.Show(Constants.NoTermSelected, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
		}

		private static void ShowBrowserResults(string url)
		{
			var searchResultsController = GetSearchResultsController();

			var browser = searchResultsController.Browser;
			browser.ScriptErrorsSuppressed = true;
			browser.Navigate(url, false);
			searchResultsController.Show();
		}

		[Action("IATESearchAllAction", Name = "Search IATE (all)", Icon = "Iate_logo")]
		[ActionLayout(typeof(IATETerminologyProviderAction), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.EditorDocumentContextMenuLocation), 10, DisplayType.Large)]
		public class IATESearchAllAction : AbstractAction
		{
			// Navigate to the IATE search term based on the document source and all existing target languages(from IATE)
			protected override void Execute()
			{
				NavigateToIATE(true);
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
				NavigateToIATE(false);
			}
		}
	}
}