using System;
using System.Collections.Generic;
using System.Linq;
using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer.ViewModel
{
	public class DsiViewerViewModel : ModelBase, IDisposable
	{
		private List<DsiModel> _documentStructureInformation;
		private List<IComment> _comments;
		private Document _activeDocument;
		private readonly EditorController _editorController;
		private readonly SegmentVisitor _segmentVisitor;

		public DsiViewerViewModel()
		{
			_segmentVisitor = new SegmentVisitor(false);

			_editorController = GetEditorController();
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			SetActiveDocument(_editorController.ActiveDocument);
		}

		public IOrderedEnumerable<DsiModel> DocumentStructureInformation
		{
			get
			{
				if (_documentStructureInformation == null)
				{
					_documentStructureInformation = new List<DsiModel>();
				}

				return _documentStructureInformation.OrderBy(a => a.DisplayName).ThenBy(a => a.Code);
			}
			set
			{
				_documentStructureInformation = value?.ToList();
				OnPropertyChanged(nameof(DocumentStructureInformation));
			}
		}

		public IOrderedEnumerable<IComment> Comments
		{
			get
			{
				if (_comments == null)
				{
					_comments = new List<IComment>();
				}

				return _comments.OrderByDescending(a => (int)a.Severity).ThenByDescending(a => a.Date);
			}
			set
			{
				_comments = value?.ToList();
				OnPropertyChanged(nameof(Comments));
			}
		}

		public bool HasDocumentStructureInformation => DocumentStructureInformation.Any();

		public bool HasComments => Comments.Any();	

		private void UpdateDocumentStructureInformation()
		{
			_documentStructureInformation.Clear();

			var segment = _activeDocument?.ActiveSegmentPair;
			var contexts = segment?.GetParagraphUnitProperties().Contexts;

			if (!(contexts?.Contexts?.Count > 0))
			{
				OnPropertyChanged(nameof(DocumentStructureInformation));
				OnPropertyChanged(nameof(HasDocumentStructureInformation));
				return;
			}

			foreach (var context in contexts.Contexts)
			{
				var color = context.DisplayColor;

				var model = new DsiModel
				{
					DisplayName = context.DisplayName,
					Description = context.Description,
					Code = context.DisplayCode,
				};

				if (color.Name == "0") // it doesn't have a color set
				{
					model.RowColor = "White";
				}
				else
				{
					model.RowColor = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
				}

				_documentStructureInformation.Add(model);
			}

			OnPropertyChanged(nameof(DocumentStructureInformation));
			OnPropertyChanged(nameof(HasDocumentStructureInformation));
		}

		private void UpdateComments()
		{
			_comments.Clear();

			var segmentPair = _activeDocument?.ActiveSegmentPair;
			if (segmentPair == null)
			{
				OnPropertyChanged(nameof(Comments));
				OnPropertyChanged(nameof(HasComments));
				return;
			}

			AddComments(segmentPair);
			AddComments(segmentPair.Source);
			AddComments(segmentPair.Target);

			OnPropertyChanged(nameof(Comments));
			OnPropertyChanged(nameof(HasComments));
		}

		private void AddComments(ISegment segment)
		{
			_segmentVisitor.VisitSegment(segment);
			foreach (var comment in _segmentVisitor.Comments)
			{
				_comments.Add(comment);
			}
		}

		private void AddComments(ISegmentPair segmentPair)
		{
			var paragraphInfo = segmentPair?.GetParagraphUnitProperties();
			if (paragraphInfo?.Comments?.Comments == null)
			{
				return;
			}

			foreach (var comment in paragraphInfo.Comments.Comments)
			{
				_comments.Add(comment);
			}
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			SetActiveDocument(e.Document);
		}

		private void SetActiveDocument(Document document)
		{
			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			}

			_activeDocument = document;

			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;

				UpdateDocumentStructureInformation();
				UpdateComments();
			}
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			UpdateDocumentStructureInformation();
			UpdateComments();
		}

		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}

		public void Dispose()
		{
			if (_editorController != null)
			{
				_editorController.ActiveDocumentChanged -= EditorController_ActiveDocumentChanged;
			}

			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			}
		}
	}
}

