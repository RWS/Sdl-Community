using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Trados.Community.Toolkit.LanguagePlatform;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Community.XLIFF.Manager.Common;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Community.XLIFF.Manager.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Versioning;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class ContentWriter : AbstractBilingualContentProcessor
	{
		private readonly Xliff _xliff;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly ImportOptions _importOptions;
		private readonly List<AnalysisBand> _analysisBands;
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private SegmentVisitor _segmentVisitor;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;

		public ContentWriter(Xliff xliff, SegmentBuilder segmentBuilder,
			ImportOptions importOptions, List<AnalysisBand> analysisBands)
		{
			_xliff = xliff;
			_segmentBuilder = segmentBuilder;
			_importOptions = importOptions;
			_analysisBands = analysisBands;

			Comments = _xliff.DocInfo.Comments;
			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		private Dictionary<string, List<IComment>> Comments { get; }

		private SegmentVisitor SegmentVisitor => _segmentVisitor ?? (_segmentVisitor = new SegmentVisitor());

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;
			base.SetFileProperties(fileInfo);
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			base.Initialize(documentInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			var importedTransUnit = GetTransUnit(paragraphUnit);
			if (importedTransUnit == null)
			{
				if (!string.IsNullOrEmpty(_importOptions.StatusSegmentNotImportedId))
				{
					var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusSegmentNotImportedId, true, out var result);
					var statusSegmentNotImported = success ? result : ConfirmationLevel.Unspecified;

					foreach (var segmentPair in paragraphUnit.SegmentPairs)
					{
						var segmentPairInfo = SegmentPairProcessor.GetSegmentPairInfo(segmentPair);

						segmentPair.Target.Properties.ConfirmationLevel = statusSegmentNotImported;

						var status = segmentPair.Properties.ConfirmationLevel.ToString();
						var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

						AddWordCounts(status, ConfirmationStatistics.WordCounts.NotProcessed, segmentPairInfo);
						AddWordCounts(match, TranslationOriginStatistics.WordCounts.NotProcessed, segmentPairInfo);
					}
				}

				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				var segmentPairInfo = SegmentPairProcessor.GetSegmentPairInfo(segmentPair);

				var importedSegmentPair = importedTransUnit.SegmentPairs.FirstOrDefault(a => a.Id == segmentPair.Properties.Id.Id);
				if (importedSegmentPair == null)
				{
					if (!string.IsNullOrEmpty(_importOptions.StatusSegmentNotImportedId))
					{
						var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusSegmentNotImportedId, true, out var result);
						var statusSegmentNotImported = success ? result : ConfirmationLevel.Unspecified;

						segmentPair.Target.Properties.ConfirmationLevel = statusSegmentNotImported;
					}

					var status = segmentPair.Properties.ConfirmationLevel.ToString();
					var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

					AddWordCounts(status, ConfirmationStatistics.WordCounts.NotProcessed, segmentPairInfo);
					AddWordCounts(match, TranslationOriginStatistics.WordCounts.NotProcessed, segmentPairInfo);

					continue;
				}

				var noOverwrite = !_importOptions.OverwriteTranslations && segmentPair.Target.Any();
				var excludeFilter = false;
				if (_importOptions.ExcludeFilterIds != null)
				{
					var status = segmentPair.Properties.ConfirmationLevel.ToString();
					var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

					excludeFilter = (segmentPair.Properties.IsLocked && _importOptions.ExcludeFilterIds.Exists(a => a == "Locked"))
									|| _importOptions.ExcludeFilterIds.Exists(a => a == status)
									|| _importOptions.ExcludeFilterIds.Exists(a => a == match);
				}

				if (noOverwrite || excludeFilter)
				{
					if (!string.IsNullOrEmpty(_importOptions.StatusTranslationNotUpdatedId))
					{
						var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
						var statusTranslationNotUpdated = success ? result : ConfirmationLevel.Unspecified;

						segmentPair.Target.Properties.ConfirmationLevel = statusTranslationNotUpdated;
					}

					var status = segmentPair.Properties.ConfirmationLevel.ToString();
					var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

					AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
					AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);

					continue;
				}

				UpdateTargetSegment(segmentPair, importedSegmentPair, segmentPairInfo);
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private static void AddWordCounts(string category, ICollection<WordCount> wordCounts, SegmentPairInfo segmentPairInfo)
		{
			var count = wordCounts.FirstOrDefault(a => a.Category == category);
			if (count != null)
			{
				count.Segments++;
				count.Words += segmentPairInfo.SourceWordCounts.Words;
				count.Characters += segmentPairInfo.SourceWordCounts.Characters;
				count.Placeables += segmentPairInfo.SourceWordCounts.Placeables;
				count.Tags += segmentPairInfo.SourceWordCounts.Tags;
			}
			else
			{
				var wordCount = new WordCount
				{
					Category = category,
					Segments = 1,
					Words = segmentPairInfo.SourceWordCounts.Words,
					Characters = segmentPairInfo.SourceWordCounts.Characters,
					Placeables = segmentPairInfo.SourceWordCounts.Placeables,
					Tags = segmentPairInfo.SourceWordCounts.Tags
				};

				wordCounts.Add(wordCount);
			}
		}

		private TransUnit GetTransUnit(IParagraphUnit paragraphUnit)
		{
			foreach (var xliffFile in _xliff.Files)
			{
				foreach (var transUnit in xliffFile.Body.TransUnits)
				{
					if (transUnit.Id == paragraphUnit.Properties.ParagraphUnitId.Id)
					{
						return transUnit;
					}
				}
			}

			return null;
		}

		private void UpdateTargetSegment(ISegmentPair segmentPair, SegmentPair importedSegmentPair, SegmentPairInfo segmentPairInfo)
		{
			var targetSegment = segmentPair.Target;

			var originalSource = (ISegment)segmentPair.Source.Clone();
			var originalTarget = (ISegment)targetSegment.Clone();

			// clear the existing content from the target segment
			targetSegment.Clear();

			var containers = new Stack<IAbstractMarkupDataContainer>();
			containers.Push(targetSegment);

			var lockedContentId = 0;
			foreach (var element in importedSegmentPair.Target.Elements)
			{
				if (element is ElementComment elementComment)
				{
					UpdateComment(elementComment, containers);
				}

				if (element is ElementTagPair elementTagPair)
				{
					UpdateTagPair(elementTagPair, originalTarget, originalSource, containers);
				}

				if (element is ElementLocked elementLocked)
				{
					lockedContentId = UpdateLockedContent(elementLocked, lockedContentId, originalTarget, originalSource, containers);
				}

				if (element is ElementPlaceholder elementPlaceholder)
				{
					UpdatePlaceholder(elementPlaceholder, originalTarget, originalSource, containers);
				}

				if (element is ElementGenericPlaceholder genericPlaceholder)
				{
					UpdateGenericPlaceholder(genericPlaceholder, originalTarget, originalSource, containers);
				}

				if (element is ElementText elementText && !string.IsNullOrEmpty(elementText.Text))
				{
					UpdateText(elementText, containers);
				}
			}

			UpdateTranslationOrigin(originalTarget, targetSegment, segmentPairInfo);
		}

		private void UpdateText(ElementText elementText, Stack<IAbstractMarkupDataContainer> containers)
		{
			var text = _segmentBuilder.Text(elementText.Text);
			var container = containers.Peek();
			container.Add(text);
		}

		private void UpdateTranslationOrigin(ISegment originalTarget, ISegment targetSegment, SegmentPairInfo segmentPairInfo)
		{
			SegmentVisitor.VisitSegment(originalTarget);
			var originalText = SegmentVisitor.Text;

			SegmentVisitor.VisitSegment(targetSegment);
			var updatedText = SegmentVisitor.Text;

			if (string.Compare(originalText, updatedText, StringComparison.Ordinal) != 0)
			{
				if (!string.IsNullOrEmpty(_importOptions.StatusTranslationUpdatedId))
				{
					if (targetSegment.Properties.TranslationOrigin != null)
					{
						var currentTranslationOrigin = (ITranslationOrigin)targetSegment.Properties.TranslationOrigin.Clone();
						targetSegment.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;
						SetTranslationOrigin(targetSegment);
					}
					else
					{
						targetSegment.Properties.TranslationOrigin = _segmentBuilder.CreateTranslationOrigin();
						SetTranslationOrigin(targetSegment);
					}

					var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationUpdatedId, true, out var result);
					var statusTranslationUpdated = success ? result : ConfirmationLevel.Unspecified;

					targetSegment.Properties.ConfirmationLevel = statusTranslationUpdated;
				}

				var status = targetSegment.Properties.ConfirmationLevel.ToString();
				var match = Enumerators.GetTranslationOriginType(targetSegment.Properties.TranslationOrigin, _analysisBands);

				AddWordCounts(status, ConfirmationStatistics.WordCounts.Processed, segmentPairInfo);
				AddWordCounts(match, TranslationOriginStatistics.WordCounts.Processed, segmentPairInfo);

			}
			else
			{
				if (!string.IsNullOrEmpty(_importOptions.StatusTranslationNotUpdatedId))
				{
					var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
					var statusTranslationNotUpdated = success ? result : ConfirmationLevel.Unspecified;

					targetSegment.Properties.ConfirmationLevel = statusTranslationNotUpdated;
				}

				var status = targetSegment.Properties.ConfirmationLevel.ToString();
				var match = Enumerators.GetTranslationOriginType(targetSegment.Properties.TranslationOrigin, _analysisBands);

				AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
				AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);
			}
		}

		private void SetTranslationOrigin(ISegment targetSegment)
		{
			targetSegment.Properties.TranslationOrigin.MatchPercent = byte.Parse("0");
			targetSegment.Properties.TranslationOrigin.OriginSystem = _importOptions.OriginSystem;
			targetSegment.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			targetSegment.Properties.TranslationOrigin.IsStructureContextMatch = false;
			targetSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;

			targetSegment.Properties.TranslationOrigin.SetMetaData("last_modified_by", _importOptions.OriginSystem);
			targetSegment.Properties.TranslationOrigin.SetMetaData("modified_on", FormatAsInvariantDateTime(DateTime.UtcNow));
		}

		private void UpdatePlaceholder(ElementPlaceholder elementPlaceholder, ISegment originalTarget, ISegment originalSource,
			Stack<IAbstractMarkupDataContainer> containers)
		{
			var placeholder = GetElement(elementPlaceholder.TagId, originalTarget, originalSource, elementPlaceholder) 
			                  ?? _segmentBuilder.CreatePlaceholder(elementPlaceholder.TagId, elementPlaceholder.DisplayText, elementPlaceholder.TagContent);

			var container = containers.Peek();
			container.Add(placeholder);
		}

		private void UpdateGenericPlaceholder(ElementGenericPlaceholder elementGenericPlaceholder,
			ISegment originalTarget, ISegment originalSource, Stack<IAbstractMarkupDataContainer> containers)
		{
			if (string.Compare(elementGenericPlaceholder.CType, "lb", StringComparison.CurrentCultureIgnoreCase) == 0)
			{
				UpdateLineBreak(elementGenericPlaceholder, containers);
			}

			//TODO provide support for other generic placeholders
		}

		private void UpdateLineBreak(ElementGenericPlaceholder elementGenericPlaceholder, Stack<IAbstractMarkupDataContainer> containers)
		{
			var textEquivalent = elementGenericPlaceholder.TextEquivalent.Replace("\\r", "\r").Replace("\\n", "\n");
			if (string.IsNullOrEmpty(textEquivalent))
			{
				textEquivalent = "\n";
			}

			var text = _segmentBuilder.Text(textEquivalent);
			var container = containers.Peek();

			container.Add(text);
		}

		private int UpdateLockedContent(ElementLocked elementLocked, int lockedContentId, ISegment originalTarget,
			ISegment originalSource, Stack<IAbstractMarkupDataContainer> containers)
		{
			if (elementLocked.Type == Element.TagType.OpeningTag)
			{
				var lockedContent = GetElement(lockedContentId.ToString(), originalTarget, originalSource, elementLocked);
				if (lockedContent == null)
				{
					lockedContent = _segmentBuilder.CreateLockedContent();
				}

				if (lockedContent is IAbstractMarkupDataContainer lockedContentContainer)
				{
					lockedContentContainer.Clear();

					var container = containers.Peek();
					container.Add(lockedContent);
					containers.Push(lockedContentContainer);

					lockedContentId++;
				}
			}
			else if (elementLocked.Type == Element.TagType.ClosingTag)
			{
				containers.Pop();
			}

			return lockedContentId;
		}

		private void UpdateTagPair(ElementTagPair elementTagPair, ISegment originalTarget, ISegment originalSource,
			Stack<IAbstractMarkupDataContainer> containers)
		{
			if (elementTagPair.Type == Element.TagType.OpeningTag)
			{
				var tagPair = GetElement(elementTagPair.TagId, originalTarget, originalSource, elementTagPair);
				if (tagPair == null)
				{
					tagPair = _segmentBuilder.CreateTagPair(elementTagPair.TagId, elementTagPair.TagContent);
				}

				if (tagPair is IAbstractMarkupDataContainer tagPairContainer)
				{
					tagPairContainer.Clear();

					var container = containers.Peek();
					container.Add(tagPair);
					containers.Push(tagPairContainer);
				}
			}
			else if (elementTagPair.Type == Element.TagType.ClosingTag)
			{
				containers.Pop();
			}
		}

		private void UpdateComment(ElementComment elementComment, Stack<IAbstractMarkupDataContainer> containers)
		{
			var comments = Comments.FirstOrDefault(a => a.Key == elementComment.Id);
			var newestComment = comments.Value.LastOrDefault();
			if (newestComment != null)
			{
				if (elementComment.Type == Element.TagType.OpeningTag)
				{
					var comment = _segmentBuilder.CreateCommentContainer(newestComment.Text,
						newestComment.Author, newestComment.Severity,
						newestComment.Date, newestComment.Version);

					if (comment is IAbstractMarkupDataContainer commentContainer)
					{
						commentContainer.Clear();

						var container = containers.Peek();
						container.Add(comment);

						containers.Push(commentContainer);
					}
				}
				else if (elementComment.Type == Element.TagType.ClosingTag)
				{
					containers.Pop();
				}
			}
		}

		private string WindowsUserId => (Environment.UserDomainName + "\\" + Environment.UserName).Trim();

		private string FormatAsInvariantDateTime(DateTime date)
		{
			return date.ToString(DateTimeFormatInfo.InvariantInfo);
		}

		private IAbstractMarkupData GetElement(string tagId, IAbstractMarkupDataContainer originalTargetSegment,
			IAbstractMarkupDataContainer sourceSegment, Element element)
		{
			var extractor = new ElementExtractor();
			extractor.GetTag(tagId, originalTargetSegment, element);
			if (extractor.FoundElement != null)
			{
				return (IAbstractMarkupData)extractor.FoundElement.Clone();
			}

			extractor.GetTag(tagId, sourceSegment, element);
			if (extractor.FoundElement != null)
			{
				return (IAbstractMarkupData)extractor.FoundElement.Clone();
			}

			//switch (element)
			//{
			//	case ElementTagPair tagPair:
			//	case ElementPlaceholder placeholder:
			//		throw new Exception("Tags in segment ID " + originalTargetSegment.Properties.Id.Id + " are corrupted!");
			//	case ElementLocked locked:
			//		throw new Exception("Locked contents in segment ID " + originalTargetSegment.Properties.Id.Id + " are corrupted!");
			//}

			//throw new Exception("Problem when reading segment #" + originalTargetSegment.Properties.Id.Id);

			return null;
		}

		private SegmentPairProcessor SegmentPairProcessor
		{
			get
			{
				if (_segmentPairProcessor != null)
				{
					return _segmentPairProcessor;
				}

				if (SourceLanguage == null || TargetLanguage == null)
				{
					throw new Exception(string.Format("Unable to parse the file; {0} langauge cannot be null!", SourceLanguage == null ? "Source" : "Target"));
				}

				var productName = GetProductName();
				var pathInfo = new Trados.Community.Toolkit.LanguagePlatform.Models.PathInfo(productName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Trados.Community.Toolkit.LanguagePlatform.Models.Settings(SourceLanguage, TargetLanguage), pathInfo);

				return _segmentPairProcessor;
			}
		}

		private string GetProductName()
		{
			if (!string.IsNullOrEmpty(_productName))
			{
				return _productName;
			}

			var studioVersionService = new StudioVersionService();
			var studioVersion = studioVersionService.GetStudioVersion();
			if (studioVersion != null)
			{
				_productName = studioVersion.StudioDocumentsFolderName;
			}

			return _productName;
		}
	}
}
