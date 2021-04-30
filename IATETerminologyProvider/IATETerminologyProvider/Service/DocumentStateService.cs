using System.Collections.Generic;
using System.Linq;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class DocumentStateService
	{
		private readonly EditorController _editorController;
		private string _activeDocumentId;
		private readonly List<DocumentEntryState> _documentEntriesState;

		public DocumentStateService()
		{			
			_documentEntriesState = new List<DocumentEntryState>();
			_editorController = SdlTradosStudio.Application.GetController<EditorController>();
		}		

		public void UpdateDocumentEntriesState(IATETermsControl iateTermsControl)
		{
			if (iateTermsControl != null && _editorController != null)
			{
				var activeFile = _editorController.ActiveDocument.Files?.ToList()[0];

				if (activeFile is null)
				{
					return;
				}

				_activeDocumentId = activeFile.Id.ToString();				

				var documentEntries = _documentEntriesState.FirstOrDefault(a => a.DocumentId == _activeDocumentId);
				if (documentEntries is null) return;
				var projectInfo = _editorController.ActiveDocument.Project.GetProjectInfo();
				iateTermsControl.UpdateEntriesInView(documentEntries.Entries,
					projectInfo.SourceLanguage, activeFile.Language, documentEntries.SelectedEntry);
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
	}
}
