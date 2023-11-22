using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Sdl.Community.DsiViewer.Commands;
using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Service;
using Sdl.Community.DsiViewer.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer.ViewModel
{
	public class DsiViewerViewModel : ModelBase, IDisposable
	{
		private readonly EditorController _editorController;
		private readonly SegmentVisitor _segmentVisitor;
		private IStudioDocument _activeDocument;
		private ICommand _applySdlMtCloudFilter;
		private ICommand _clearSdlMtCloudFilter;
		private List<IComment> _comments;
		private List<DsiModel> _documentStructureInformation;
		private TranslationOriginData _translationOriginData;

		public DsiViewerViewModel()
		{
			_segmentVisitor = new SegmentVisitor(false);
			_comments = new List<IComment>();
			_documentStructureInformation = new List<DsiModel>();

			_editorController = DsiViewerInitializer.EditorController;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			SetActiveDocument(_editorController.ActiveDocument);
		}

		public ICommand ApplySdlMtCloudFilter => _applySdlMtCloudFilter ??= new CommandHandler(() => ApplyFilter(), true);
		public ICommand ClearSdlMtCloudFilter => _clearSdlMtCloudFilter ??= new CommandHandler(() => ApplyFilter(true), true);

		public IOrderedEnumerable<IComment> Comments
		{
			get
			{
				_comments ??= new List<IComment>();
				return _comments.OrderByDescending(a => (int)a.Severity).ThenByDescending(a => a.Date);
			}
			set
			{
				_comments = value?.ToList();
				OnPropertyChanged(nameof(Comments));
			}
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

		public FilterApplier FilterApplier { get; set; } = new();

		public bool HasComments => Comments.Any();

		public bool HasDocumentStructureInformation => DocumentStructureInformation.Any();

		public bool HasSdlMtCloudRelatedInfo
													=>
				_editorController?.ActiveDocument?.SegmentPairs.Any(
					sp => sp.Properties.TranslationOrigin.MetaDataContainsKey("quality_estimation")) ?? false;

		public bool HasTranslationOriginMetadata => TranslationOriginData != null;

		public object SelectedItem { get; set; }

		public TranslationOriginData TranslationOriginData
		{
			get => _translationOriginData;
			set
			{
				_translationOriginData = value;
				OnPropertyChanged(nameof(TranslationOriginData));
				OnPropertyChanged(nameof(HasTranslationOriginMetadata));
			}
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
				_activeDocument.SegmentsTranslationOriginChanged -= ActiveDocument_SegmentsTranslationOriginChanged;
				_activeDocument.ContentChanged -= ActiveDocument_ContentChanged;
			}
		}

		private static string GetMetaValue(string metaValue)
		{
			var containsNumber = int.TryParse(metaValue, out _);

			return containsNumber ? metaValue : PluginResources.ResourceManager.GetString("StructureContextInfo_MetaValue_" + metaValue);
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			UpdateDocumentStructureInformation();
			UpdateComments();
			UpdateTranslationOriginInformation();
		}

		private void ActiveDocument_ContentChanged(object sender, DocumentContentEventArgs e)
		{
			UpdateComments();
		}

		private void ActiveDocument_SegmentsTranslationOriginChanged(object sender, EventArgs e)
		{
			UpdateTranslationOriginInformation();
			OnPropertyChanged(nameof(HasSdlMtCloudRelatedInfo));
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

		private void ApplyFilter(bool isClearing = false)
		{
			if (isClearing)
			{
				FilterApplier.ClearFilter();
			}

			FilterApplier.ApplyFilter();
		}

		private void EditorController_ActiveDocumentChanged(object sender, DocumentEventArgs e)
		{
			SetActiveDocument(e.Document);
			OnPropertyChanged(nameof(HasSdlMtCloudRelatedInfo));
		}

		private string GetAdditionalInformation(IContextInfo context)
		{
			var additionalInfo = new StringBuilder();
			foreach (var metaPair in context.MetaData)
			{
				var metaDataInfoDescriptionKey = PluginResources.ResourceManager.GetString("StructureContextInfo_MetaKey_" + metaPair.Key);
				if (!string.IsNullOrEmpty(metaDataInfoDescriptionKey))
				{
					additionalInfo.Append($"{metaDataInfoDescriptionKey}: {GetMetaValue(metaPair.Value)}; ");
				}
			}
			return additionalInfo.ToString();
		}

		private void SetActiveDocument(IStudioDocument document)
		{
			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged -= ActiveDocument_ActiveSegmentChanged;
			}

			_activeDocument = document;

			if (_activeDocument != null)
			{
				_activeDocument.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
				_activeDocument.SegmentsTranslationOriginChanged += ActiveDocument_SegmentsTranslationOriginChanged;
				_activeDocument.ContentChanged += ActiveDocument_ContentChanged;

				UpdateDocumentStructureInformation();
				UpdateComments();
				UpdateTranslationOriginInformation();
			}
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
				if (context.DisplayName == null)
				{
					continue;
				}

				var color = context.DisplayColor;
				var model = new DsiModel
				{
					DisplayName = context.DisplayName,
					Code = context.DisplayCode,
					Description = context.ContextType == TMMatchContextTypes.LengthInformation
						? GetAdditionalInformation(context)
						: context.Description
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

		private void UpdateTranslationOriginInformation()
		{
			TranslationOriginData = null;
			var translationOrigin = _activeDocument.ActiveSegmentPair?.Properties.TranslationOrigin;
			if (translationOrigin is null || string.IsNullOrWhiteSpace(translationOrigin.GetMetaData("quality_estimation")))
			{
				OnPropertyChanged(nameof(TranslationOriginData));
				OnPropertyChanged(nameof(HasTranslationOriginMetadata));
				return;
			}

			switch (translationOrigin.OriginSystem)
			{
				case not null when translationOrigin.OriginSystem.ToLower().Contains(PluginResources.ProviderId):
					{
						var qualityEstimation = translationOrigin.GetMetaData("quality_estimation");
						TranslationOriginData = new TranslationOriginData
						{
							QualityEstimation = qualityEstimation,
							Model = translationOrigin.GetMetaData("model"),
						};
						break;
					}

				default:
					TranslationOriginData = null;
					break;
			}
		}
	}
}