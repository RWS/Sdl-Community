using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Sdl.Community.DsiViewer.Commands;
using Sdl.Community.DsiViewer.Model;
using Sdl.Community.DsiViewer.Service;
using Sdl.Community.DsiViewer.Services;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.LanguagePlatform.Core;
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
		private List<DSITagModel> _segmentTags;
		private TranslationOriginData _translationOriginData;

		private string _collapseDsiButtonName;
		private string _collapseCommentsButtonName;
		private string _collapseTagsButtonName;
		private string _collapseLwButtonName;

		private bool _isDsiVisible;
		private bool _isCommentsVisible;
		private bool _isTagsVisible;
		private bool _isLwVisible;

		private ICommand _collapseCommentsCommand;
		private ICommand _collapseTagsCommand;
		private ICommand _collapseDsiCommand;
		private ICommand _collapseLwCommand;

		public DsiViewerViewModel()
		{
			_segmentVisitor = new SegmentVisitor(false);
			_comments = new List<IComment>();
			_documentStructureInformation = new List<DsiModel>();

			_editorController = DsiViewerInitializer.EditorController;
			_editorController.ActiveDocumentChanged += EditorController_ActiveDocumentChanged;

			CollapseDsiButtonName = "Collapse";
			CollapseCommentsButtonName = "Collapse";
			CollapseTagsButtonName = "Collapse";
			CollapseLwButtonName = "Collapse";

			IsDsiVisible = true;
			IsCommentsVisible = true;
			IsTagsVisible = true;
			IsLwVisible = true;

			SetActiveDocument(_editorController.ActiveDocument);
		}

		public ICommand ApplySdlMtCloudFilter => _applySdlMtCloudFilter ??= new CommandHandler(() => ApplyFilter(), true);
		public ICommand ClearSdlMtCloudFilter => _clearSdlMtCloudFilter ??= new CommandHandler(() => ApplyFilter(true), true);
		public ICommand CollapseDsiCommand => _collapseDsiCommand ??= new CommandHandler(CollapseDsi, true);
		public ICommand CollapseCommentsCommand => _collapseCommentsCommand ??= new CommandHandler(CollapseComments, true);
		public ICommand CollapseTagsCommand => _collapseTagsCommand ??= new CommandHandler(CollapseTags, true);
		public ICommand CollapseLwCommand => _collapseLwCommand ??= new CommandHandler(CollapseLw, true);

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
				OnPropertyChanged(nameof(HasComments));
			}
		}

		public bool HasComments => Comments?.Any() ?? false;

		public IOrderedEnumerable<DsiModel> DocumentStructureInformation
		{
			get
			{
				_documentStructureInformation ??= new List<DsiModel>();
				return _documentStructureInformation.OrderBy(a => a.DisplayName).ThenBy(a => a.Code);
			}
			set
			{
				_documentStructureInformation = value?.ToList();
				OnPropertyChanged(nameof(DocumentStructureInformation));
				OnPropertyChanged(nameof(HasDocumentStructureInformation));
			}
		}

		public bool HasDocumentStructureInformation => DocumentStructureInformation?.Any() ?? false;

		public FilterApplier FilterApplier { get; set; } = new();

		public bool HasSdlMtCloudRelatedInfo =>
				_activeDocument?.SegmentPairs?.Any(x => x.Properties?.TranslationOrigin?.MetaDataContainsKey("quality_estimation") ?? false) ?? false;

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

		public List<DSITagModel> SegmentTags
		{
			get => _segmentTags;
			set
			{
				_segmentTags = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(HasTags));
			}
		}

		public bool HasTags => SegmentTags?.Any() ?? false;

		public string CollapseDsiButtonName
		{
			get { return _collapseDsiButtonName; }
			set
			{
				if (_collapseDsiButtonName != value)
				{
					_collapseDsiButtonName = value;
					OnPropertyChanged();
				}
			}
		}

		public string CollapseCommentsButtonName
		{
			get { return _collapseCommentsButtonName; }
			set
			{
				if (_collapseCommentsButtonName != value)
				{
					_collapseCommentsButtonName = value;
					OnPropertyChanged();
				}
			}
		}

		public string CollapseTagsButtonName
		{
			get { return _collapseTagsButtonName; }
			set
			{
				if (_collapseTagsButtonName != value)
				{
					_collapseTagsButtonName = value;
					OnPropertyChanged();
				}
			}
		}

		public string CollapseLwButtonName
		{
			get { return _collapseLwButtonName; }
			set
			{
				if (_collapseLwButtonName != value)
				{
					_collapseLwButtonName = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IsDsiVisible
		{
			get { return _isDsiVisible; }
			set
			{
				if (_isDsiVisible != value)
				{
					_isDsiVisible = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IsCommentsVisible
		{
			get { return _isCommentsVisible; }
			set
			{
				if (_isCommentsVisible != value)
				{
					_isCommentsVisible = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IsTagsVisible
		{
			get { return _isTagsVisible; }
			set
			{
				if (_isTagsVisible != value)
				{
					_isTagsVisible = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IsLwVisible
		{
			get { return _isLwVisible; }
			set
			{
				if (_isLwVisible != value)
				{
					_isLwVisible = value;
					OnPropertyChanged();
				}
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
			UpdateTags();
			OnPropertyChanged(nameof(HasSdlMtCloudRelatedInfo));
		}

		private void ActiveDocument_ContentChanged(object sender, DocumentContentEventArgs e)
		{
			UpdateComments();
		}

		private void ActiveDocument_SegmentsTranslationOriginChanged(object sender, EventArgs e)
		{
			UpdateTranslationOriginInformation();
			UpdateTags();
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
				UpdateTags();
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

			if (translationOrigin.OriginSystem is null)
			{
				TranslationOriginData = null;
				return;
			}

			var qualityEstimation = translationOrigin.GetMetaData("quality_estimation");
			TranslationOriginData = new TranslationOriginData
			{
				QualityEstimation = qualityEstimation,
				Model = translationOrigin.GetMetaData("model"),
			};
		}

		private void UpdateTags()
		{
			SegmentTags = new();
			var activeSegmentSource = _activeDocument?.ActiveSegmentPair?.Source;
			if (activeSegmentSource is null)
			{
				return;
			}

			var tags = new List<DSITagModel>();
			ExtractTags(activeSegmentSource.AllSubItems, tags);
			SegmentTags = tags.Distinct().ToList();
		}

		private void ExtractTags(IEnumerable<IAbstractMarkupData> allSubItems, List<DSITagModel> tags)
		{
			if (allSubItems is null)
			{
				return;
			}

			foreach (var item in allSubItems)
			{
				if (item is ILockedContent lockedContent)
				{
					ExtractTags(lockedContent.Content.AllSubItems, tags);
					continue;
				}

				if (item is Text || item is not IAbstractTag)
				{
					continue;
				}

				var tagPair = item as ITagPair;
				var placeholderTag = item as IPlaceholderTag;
				var structureTag = item as IStructureTag;

				ExtractTagPairProperties(tagPair, tags);
				ExtractPlaceholderTagProperties(placeholderTag, tags);
				ExtractStructureTagProperties(structureTag, tags);

				if (tagPair is not null)
				{
					ExtractTags(tagPair, tags);
				}
			}
		}

		private void ExtractTagPairProperties(ITagPair tag, List<DSITagModel> tags)
		{
			if (tag is null)
			{
				return;
			}

			var tagId = tag.TagProperties.TagId.Id;
			if (tags.Any(x => x.Id == tagId))
			{
				return;
			}

			var pattern = @"<(\w+[-]?\w*)([^>]*)>(.*?)<\/\1>";
			var example = tag.ToString();
			var match = Regex.Match(example, pattern, RegexOptions.Singleline);

			var newTagModel = new DSITagModel()
			{
				Id = tagId,
				StartTag = $"<{match.Groups[1].Value}>",
				EndTag = $"</{match.Groups[1].Value}>",
				Attributes = match.Groups[2].Value,
				Content = match.Groups[3].Value,
			};

			tags.Add(newTagModel);
		}

		private void ExtractPlaceholderTagProperties(IPlaceholderTag tag, List<DSITagModel> tags)
		{
			if (tag is null)
			{
				return;
			}

			var tagId = tag.TagProperties.TagId.Id;
			if (tags.Any(x => x.Id == tagId))
			{
				return;
			}


			var tagString = tag.ToString();
			var pattern = @"<(?<TagName>\w+)(?<Attributes>.*?)>";

			var match = Regex.Match(tagString, pattern, RegexOptions.Singleline);

			var newTagModel = new DSITagModel()
			{
				Id = tagId,
				StartTag = $"<{match.Groups["TagName"].Value}\\>",
				Attributes = match.Groups["Attributes"].Value
			};

			tags.Add(newTagModel);
		}

		private void ExtractStructureTagProperties(IStructureTag tag, List<DSITagModel> tags)
		{
			if (tag is null)
			{
				return;
			}
		}

		private void CollapseDsi()
		{
			IsDsiVisible = !IsDsiVisible;
			CollapseDsiButtonName = IsDsiVisible ? "Collapse" : "Expand";
		}

		private void CollapseComments()
		{
			IsCommentsVisible = !IsCommentsVisible;
			CollapseCommentsButtonName = IsCommentsVisible ? "Collapse" : "Expand";
		}

		private void CollapseTags()
		{
			IsTagsVisible = !IsTagsVisible;
			CollapseTagsButtonName = IsTagsVisible ? "Collapse" : "Expand";
		}

		private void CollapseLw()
		{
			IsLwVisible = !IsLwVisible;
			CollapseLwButtonName = IsLwVisible ? "Collapse" : "Expand";
		}
	}
}