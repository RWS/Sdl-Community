using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Windows;

namespace Sdl.Community.IATETerminologyProvider.Actions
{
    public static class IATESearchActionHelper
    {
        /// <summary>
		/// Construct URL based on the term/phrase selection using language pairs either from source segment, or from target segment.
		/// </summary>
		/// <param name="isSearchAll"></param>
        public static void NavigateToIATE(bool isSearchAll)
        {
            var editorController = SdlTradosStudio.Application.GetController<EditorController>();
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

                    var searchResultsView = SearchResultsViewPart.ActiveInstance;
                    if (searchResultsView == null)
                    {
                        return;
                    }

                    searchResultsView.NavigateTo(url);

                }
                else
                {
                    MessageBox.Show(Constants.NoTermSelected, string.Empty, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
