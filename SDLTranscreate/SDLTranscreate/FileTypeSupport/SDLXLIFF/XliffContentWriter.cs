using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Trados.Community.Toolkit.LanguagePlatform;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Versioning;
using Trados.Transcreate.Common;
using Trados.Transcreate.FileTypeSupport.MSOffice.Model;
using Trados.Transcreate.FileTypeSupport.XLIFF.Model;
using Trados.Transcreate.Model;

namespace Trados.Transcreate.FileTypeSupport.SDLXLIFF
{
	public class XliffContentWriter : AbstractBilingualContentProcessor
	{
		private readonly Xliff _xliff;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly ImportOptions _importOptions;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly bool _importBackTranslations;
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private SegmentVisitor _segmentVisitor;
		private SegmentPairProcessor _segmentPairProcessor;
		private string _productName;

		public XliffContentWriter(Xliff xliff, SegmentBuilder segmentBuilder,
			ImportOptions importOptions, List<AnalysisBand> analysisBands, List<string> tagIds,
			bool importBackTranslations)
		{
			_xliff = xliff;
			_segmentBuilder = segmentBuilder;
			_importOptions = importOptions;
			_analysisBands = analysisBands;
			_importBackTranslations = importBackTranslations;

			_segmentBuilder.ExistingTagIds = tagIds;

			Comments = _xliff.DocInfo.Comments ?? new Dictionary<string, List<IComment>>();
			ConfirmationStatistics = new ConfirmationStatistics();
			TranslationOriginStatistics = new TranslationOriginStatistics();
		}

		public ConfirmationStatistics ConfirmationStatistics { get; }

		public TranslationOriginStatistics TranslationOriginStatistics { get; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		private Dictionary<string, List<IComment>> Comments { get; }

		private SegmentVisitor SegmentVisitor => _segmentVisitor ?? (_segmentVisitor = new SegmentVisitor());

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;

			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;

			base.SetFileProperties(fileInfo);
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
						SegmentPairInfo segmentPairInfo = null;
						try
						{
							segmentPairInfo = SegmentPairProcessor.GetSegmentPairInfo(segmentPair);
						}
						catch
						{
							// catch all; ignore
						}

						segmentPair.Target.Properties.ConfirmationLevel = statusSegmentNotImported;

						var status = segmentPair.Properties.ConfirmationLevel.ToString();
						var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

						AddWordNotProcessedCounts(status, segmentPairInfo, match);
					}
				}

				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			if (importedTransUnit.Contexts.Count > 0)
			{
				var contexts = paragraphUnit.Properties.Contexts.Contexts;
				foreach (var importContext in importedTransUnit.Contexts)
				{
					var existingContext = contexts.FirstOrDefault(a =>
						string.Compare(a.ContextType, importContext.ContextType, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(a.DisplayCode, importContext.DisplayCode, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(a.DisplayName, importContext.DisplayName, StringComparison.CurrentCultureIgnoreCase) == 0 &&
						string.Compare(a.Description, importContext.Description, StringComparison.CurrentCultureIgnoreCase) == 0);

					if (existingContext == null)
					{
						var newContext = _segmentBuilder.CreateContextInfo(importContext);
						contexts.Add(newContext);
					}
				}
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				SegmentPairInfo segmentPairInfo = null;
				try
				{
					segmentPairInfo = SegmentPairProcessor.GetSegmentPairInfo(segmentPair);
				}
				catch
				{
					// catch all; ignore
				}

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

					AddWordNotProcessedCounts(status, segmentPairInfo, match);

					continue;
				}

				var noOverwrite = !_importOptions.OverwriteTranslations && segmentPair.Target.Any();
				var excludeFilter = false;
				if (_importOptions.ExcludeFilterIds?.Count > 0)
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
					else
					{
						segmentPair.Target.Properties.ConfirmationLevel = importedSegmentPair.ConfirmationLevel;
					}

					var status = segmentPair.Properties.ConfirmationLevel.ToString();
					var match = Enumerators.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin, _analysisBands);

					AddWordExcludedCounts(status, segmentPairInfo, match);

					continue;
				}

				UpdateTargetSegment(segmentPair, importedSegmentPair, segmentPairInfo);
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private string GetContextType(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.Properties.Contexts?.Contexts != null)
			{
				var contextType = "Recommended";
				foreach (var context in paragraphUnit.Properties.Contexts.Contexts)
				{
					if (context.ContextType.StartsWith("Alternative "))
					{
						return context.ContextType;
					}
				}

				return contextType;
			}

			return null;
		}

		private static void AddWordCounts(string category, ICollection<WordCount> wordCounts, SegmentPairInfo segmentPairInfo)
		{
			var count = wordCounts.FirstOrDefault(a => a.Category == category);
			if (count != null)
			{
				count.Segments++;
				count.Words += segmentPairInfo?.SourceWordCounts?.Words ?? 0;
				count.Characters += segmentPairInfo?.SourceWordCounts?.Characters ?? 0;
				count.Placeables += segmentPairInfo?.SourceWordCounts?.Placeables ?? 0;
				count.Tags += segmentPairInfo?.SourceWordCounts?.Tags ?? 0;
			}
			else
			{
				var wordCount = new WordCount
				{
					Category = category,
					Segments = 1,
					Words = segmentPairInfo?.SourceWordCounts?.Words ?? 0,
					Characters = segmentPairInfo?.SourceWordCounts?.Characters ?? 0,
					Placeables = segmentPairInfo?.SourceWordCounts?.Placeables ?? 0,
					Tags = segmentPairInfo?.SourceWordCounts?.Tags ?? 0
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
			var sourceSegment = segmentPair.Source;
			var targetSegment = segmentPair.Target;

			var originalSource = (ISegment)sourceSegment.Clone();
			var originalTarget = (ISegment)targetSegment.Clone();

			if (_importBackTranslations &&
				importedSegmentPair.TranslationOrigin != null &&
				importedSegmentPair.TranslationOrigin.MetaDataContainsKey("back-translation"))
			{
				var backTranslation = importedSegmentPair.TranslationOrigin.GetMetaData("back-translation");
				var backTranslationTokens = JsonConvert.DeserializeObject<List<Token>>(backTranslation);
				targetSegment = _segmentBuilder.GetUpdatedSegment(targetSegment,
					backTranslationTokens, sourceSegment);
			}
			else
			{
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
						lockedContentId = UpdateLockedContent(elementLocked, lockedContentId, originalTarget,
							originalSource, containers);
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
			}

			UpdateTranslationOrigin(originalTarget, targetSegment, segmentPairInfo, importedSegmentPair);
		}

		private void UpdateText(ElementText elementText, Stack<IAbstractMarkupDataContainer> containers)
		{
			var text = _segmentBuilder.Text(elementText.Text);
			var container = containers.Peek();
			container.Add(text);
		}

		private void UpdateTranslationOrigin(ISegment originalTarget, ISegment targetSegment, SegmentPairInfo segmentPairInfo, SegmentPair importedSegmentPair)
		{
			SegmentVisitor.VisitSegment(originalTarget);
			var originalText = SegmentVisitor.Text;

			SegmentVisitor.VisitSegment(targetSegment);
			var updatedText = SegmentVisitor.Text;

			if (string.Compare(originalText, updatedText, StringComparison.Ordinal) != 0)
			{
				if (!string.IsNullOrEmpty(_importOptions.StatusTranslationUpdatedId))
				{
					if (targetSegment.Properties.TranslationOrigin == null)
					{
						targetSegment.Properties.TranslationOrigin = importedSegmentPair.TranslationOrigin;
					}
					
					if (targetSegment.Properties.TranslationOrigin != null)
					{
						var currentTranslationOrigin = (ITranslationOrigin)targetSegment.Properties.TranslationOrigin.Clone();
						targetSegment.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;
					}
					else
					{
						targetSegment.Properties.TranslationOrigin = _segmentBuilder.CreateTranslationOrigin();
					}

					SetTranslationOrigin(targetSegment);

					var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationUpdatedId, true, out var result);
					var statusTranslationUpdated = success ? result : importedSegmentPair.ConfirmationLevel;

					targetSegment.Properties.ConfirmationLevel = statusTranslationUpdated;
					targetSegment.Properties.IsLocked = importedSegmentPair.IsLocked;
				}
				else
				{
					targetSegment.Properties.TranslationOrigin = importedSegmentPair.TranslationOrigin;
					targetSegment.Properties.IsLocked = importedSegmentPair.IsLocked;
					targetSegment.Properties.ConfirmationLevel = importedSegmentPair.ConfirmationLevel;
				}

				var status = targetSegment.Properties.ConfirmationLevel.ToString();
				var match = Enumerators.GetTranslationOriginType(targetSegment.Properties.TranslationOrigin, _analysisBands);

				AddWordProcessedCounts(status, segmentPairInfo, match);
			}
			else
			{
				if (!string.IsNullOrEmpty(_importOptions.StatusTranslationNotUpdatedId))
				{
					var success = Enum.TryParse<ConfirmationLevel>(_importOptions.StatusTranslationNotUpdatedId, true, out var result);
					var statusTranslationNotUpdated = success ? result : importedSegmentPair.ConfirmationLevel;

					targetSegment.Properties.ConfirmationLevel = statusTranslationNotUpdated;
				}
				else
				{
					targetSegment.Properties.ConfirmationLevel = importedSegmentPair.ConfirmationLevel;
				}
				
				targetSegment.Properties.TranslationOrigin = importedSegmentPair.TranslationOrigin;
				targetSegment.Properties.IsLocked = importedSegmentPair.IsLocked;

				var status = targetSegment.Properties.ConfirmationLevel.ToString();
				var match = Enumerators.GetTranslationOriginType(targetSegment.Properties.TranslationOrigin, _analysisBands);

				AddWordExcludedCounts(status, segmentPairInfo, match);
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

		private void AddWordProcessedCounts(string status, SegmentPairInfo segmentPairInfo, string match)
		{
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Processed, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Processed, segmentPairInfo);
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);
		}

		private void AddWordNotProcessedCounts(string status, SegmentPairInfo segmentPairInfo, string match)
		{
			AddWordCounts(status, ConfirmationStatistics.WordCounts.NotProcessed, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.NotProcessed, segmentPairInfo);
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);
		}

		private void AddWordExcludedCounts(string status, SegmentPairInfo segmentPairInfo, string match)
		{
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Excluded, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Excluded, segmentPairInfo);
			AddWordCounts(status, ConfirmationStatistics.WordCounts.Total, segmentPairInfo);
			AddWordCounts(match, TranslationOriginStatistics.WordCounts.Total, segmentPairInfo);
		}

		private void UpdatePlaceholder(ElementPlaceholder elementPlaceholder, ISegment originalTarget, ISegment originalSource,
			Stack<IAbstractMarkupDataContainer> containers)
		{
			var placeholder = GetElement(elementPlaceholder.TagId, originalTarget, originalSource, elementPlaceholder)
							  ?? _segmentBuilder.CreatePlaceholder(elementPlaceholder.TagId, elementPlaceholder.TagContent);

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
			if (elementLocked.Type == Element.TagType.TagOpen)
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
			else if (elementLocked.Type == Element.TagType.TagClose)
			{
				containers.Pop();
			}

			return lockedContentId;
		}

		private void UpdateTagPair(ElementTagPair elementTagPair, ISegment originalTarget, ISegment originalSource,
			Stack<IAbstractMarkupDataContainer> containers)
		{
			if (elementTagPair.Type == Element.TagType.TagOpen)
			{
				var tagPair = GetElement(elementTagPair.TagId, originalTarget, originalSource, elementTagPair)
							  ?? _segmentBuilder.CreateTagPair(elementTagPair.TagId, elementTagPair.TagContent);

				if (tagPair is IAbstractMarkupDataContainer tagPairContainer)
				{
					tagPairContainer.Clear();

					var container = containers.Peek();
					container.Add(tagPair);
					containers.Push(tagPairContainer);
				}
			}
			else if (elementTagPair.Type == Element.TagType.TagClose)
			{
				containers.Pop();
			}
		}

		private void UpdateComment(ElementComment elementComment, Stack<IAbstractMarkupDataContainer> containers)
		{
			var comments = Comments.FirstOrDefault(a => a.Key == elementComment.Id);
			var newestComment = comments.Value?.LastOrDefault();
			if (newestComment != null)
			{
				if (elementComment.Type == Element.TagType.TagOpen)
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
				else if (elementComment.Type == Element.TagType.TagClose)
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
