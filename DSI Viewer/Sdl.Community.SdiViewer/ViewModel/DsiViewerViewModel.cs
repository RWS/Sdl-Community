﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Sdl.Community.DsiViewer.Commands;
using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Services;
using Sdl.Community.DsiViewer.Studio.DisplayFilters;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer.ViewModel
{
	public class DsiViewerViewModel : ModelBase, IDisposable
	{
		private List<DsiModel> _documentStructureInformation;
		private List<IComment> _comments;
		private TranslationOriginData _translationOriginData;

		private IStudioDocument _activeDocument;
		private readonly EditorController _editorController;
		private readonly SegmentVisitor _segmentVisitor;
		private ICommand _applySdlMtCloudFilter;

		public DsiViewerViewModel()
		{
			_segmentVisitor = new SegmentVisitor(false);
			_comments = new List<IComment>();
			_documentStructureInformation = new List<DsiModel>();

			_editorController = GetEditorController();
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			SetActiveDocument(_editorController.ActiveDocument);
		}

		public ICommand ApplySdlMtCloudFilter => _applySdlMtCloudFilter ??= new CommandHandler(ApplyFilter, true);

		private void ApplyFilter()
		{
			//DsiViewerInitializer.EditorController.ActiveDocument.ApplyFilterOnSegments();
		}

		public SdlMtCloudFilterSettings SdlMtCloudFilterSettings { get; } = new SdlMtCloudFilterSettings();

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
		public bool HasTranslationOriginMetadata => TranslationOriginData != null;

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

		private string GetEstimationColorLabel(string estimation)
		{
			Color color;
			switch (estimation)
			{
				case "Good":
					color = Color.FromArgb(0, 128, 64);
					break;
				case "Adequate":
					color = Color.FromArgb(0, 128, 255);
					break;
				case "Poor":
					color = Color.FromArgb(255, 72, 72);
					break;
				default:
					color = Color.FromArgb(183, 183, 219);
					break;
			}

			return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
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

				UpdateDocumentStructureInformation();
				UpdateComments();
				UpdateTranslationOriginInformation();
			}
		}

		private void ActiveDocument_SegmentsTranslationOriginChanged(object sender, EventArgs e)
		{
			UpdateTranslationOriginInformation();
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			UpdateDocumentStructureInformation();
			UpdateComments();
			UpdateTranslationOriginInformation();
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

			var qualityEstimation = translationOrigin.GetMetaData("quality_estimation");
			TranslationOriginData = new TranslationOriginData
			{
				QualityEstimation = qualityEstimation,
				Model = translationOrigin.GetMetaData("model"),
				ColorCode = GetEstimationColorLabel(qualityEstimation)
			};
		}

		private static EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
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
		private static string GetMetaValue(string metaValue)
		{
			var containsNumber = int.TryParse(metaValue, out _);

			return containsNumber ? metaValue : PluginResources.ResourceManager.GetString("StructureContextInfo_MetaValue_" + metaValue);
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
			}
		}
	}
}

