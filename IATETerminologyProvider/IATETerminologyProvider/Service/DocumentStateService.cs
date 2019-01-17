using System.Collections.Generic;
using System.Linq;
using IATETerminologyProvider.Model;
using IATETerminologyProvider.Ui;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace IATETerminologyProvider.Service
{
	public class DocumentStateService
	{
		private EditorController _editorController;
		private string _activeDocumentId;
		private readonly List<DocumentEntryState> _documentEntriesState;

		public DocumentStateService()
		{			
			_documentEntriesState = new List<DocumentEntryState>();
			_editorController = GetEditorController();
		}		

		public void UpdateDocumentEntriesState(IATETermsControl iateTermsControl)
		{
			if (iateTermsControl != null && _editorController != null)
			{
				_activeDocumentId = _editorController.ActiveDocument.Files?.ToList()[0].Id.ToString();

				var documentEntries = _documentEntriesState.FirstOrDefault(a => a.DocumentId == _activeDocumentId);
				if (documentEntries != null)
				{
					iateTermsControl.UpdateEntriesInView(documentEntries.Entries,
						_editorController.ActiveDocument.Project.GetProjectInfo().SourceLanguage, documentEntries.SelectedEntry);
				}
			}
		}

		public void SaveDocumentEntriesState(IATETermsControl iateTermsControl)
		{
			if (iateTermsControl != null && _editorController != null)
			{
				var entries = iateTermsControl.GetEntries();
				var selectedEntry = iateTermsControl.GetSelectedEntry();

				var documentEntries = _documentEntriesState.FirstOrDefault(a => a.DocumentId == _activeDocumentId);

				if (documentEntries != null)
				{
					documentEntries.Entries = entries;
					documentEntries.SelectedEntry = selectedEntry;
				}
				else
				{
					_documentEntriesState.Add(new DocumentEntryState
					{
						DocumentId = _activeDocumentId,
						Entries = entries,
						SelectedEntry = selectedEntry
					});
				}
			}
		}

		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}
	}
}
