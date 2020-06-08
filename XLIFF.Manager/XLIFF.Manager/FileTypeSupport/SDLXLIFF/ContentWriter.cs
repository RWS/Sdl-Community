using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using File = Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model.File;

namespace Sdl.Community.XLIFF.Manager.FileTypeSupport.SDLXLIFF
{
	public class ContentWriter : AbstractBilingualContentProcessor
	{
		private readonly Xliff _xliff;
		private readonly SegmentBuilder _segmentBuilder;
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private SegmentVisitor _segmentVisitor;
		private File _file;

		public ContentWriter(Xliff xliff, SegmentBuilder segmentBuilder)
		{
			_xliff = xliff;
			_segmentBuilder = segmentBuilder;

			Comments = _xliff.DocInfo.Comments;
		}

		private Dictionary<string, List<IComment>> Comments { get; }

		private SegmentVisitor SegmentVisitor => _segmentVisitor ?? (_segmentVisitor = new SegmentVisitor());

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;

			var fileName = Path.GetFileName(fileInfo.FileConversionProperties.OriginalFilePath);

			_file = _xliff.Files.FirstOrDefault(a =>
				string.Compare(Path.GetFileName(a.Original), fileName, StringComparison.CurrentCultureIgnoreCase) == 0);

			base.SetFileProperties(fileInfo);
		}

		public override void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;
			base.Initialize(documentInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			var paragraph = _file.Body.TransUnits.FirstOrDefault(a => a.Id == paragraphUnit.Properties.ParagraphUnitId.Id);
			if (paragraph == null)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				var importedSegmentPair = paragraph.SegmentPairs.FirstOrDefault(a => a.Id == segmentPair.Properties.Id.Id);
				if (importedSegmentPair == null)
				{
					continue;
				}

				if (segmentPair.Properties.IsLocked)
				{
					continue;
				}

				UpdateTargetSegment(segmentPair, importedSegmentPair);
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private void UpdateTargetSegment(ISegmentPair segmentPair, SegmentPair importedSegmentPair)
		{
			var targetSegment = segmentPair.Target;

			var originalSource = (ISegment) segmentPair.Source.Clone();
			var originalTarget = (ISegment) targetSegment.Clone();

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

				if (element is ElementText elementText && !string.IsNullOrEmpty(elementText.Text))
				{
					UpdateText(elementText, containers);
				}
			}

			UpdateTranslationOrigin(originalTarget, targetSegment);
		}

		private void UpdateText(ElementText elementText1, Stack<IAbstractMarkupDataContainer> containers)
		{
			var text = _segmentBuilder.Text(elementText1.Text);
			var container = containers.Peek();
			container.Add(text);
		}

		private void UpdateTranslationOrigin(ISegment originalTarget, ISegment targetSegment)
		{
			// TODO compare if updated
			SegmentVisitor.VisitSegment(originalTarget);
			var originalText = SegmentVisitor.Text;

			SegmentVisitor.VisitSegment(targetSegment);
			var updatedText = SegmentVisitor.Text;

			if (string.Compare(originalText, updatedText, StringComparison.Ordinal) != 0)
			{
				// TODO: everything related to the TranslationOrigin will be revised at a later
				// development phase... assigning default values for now!
				if (targetSegment.Properties.TranslationOrigin != null)
				{
					var currentTranslationOrigin = (ITranslationOrigin)targetSegment.Properties.TranslationOrigin.Clone();

					targetSegment.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;

					targetSegment.Properties.TranslationOrigin.MatchPercent = byte.Parse("0");
					targetSegment.Properties.TranslationOrigin.OriginSystem = string.Empty;
					targetSegment.Properties.TranslationOrigin.OriginType = string.Empty;
					targetSegment.Properties.TranslationOrigin.IsStructureContextMatch = false;
					targetSegment.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;

					targetSegment.Properties.TranslationOrigin.SetMetaData("last_modified_by", "Polyglot");
					targetSegment.Properties.TranslationOrigin.SetMetaData("modified_on", FormatAsInvariantDateTime(DateTime.UtcNow));
				}

				targetSegment.Properties.ConfirmationLevel = ConfirmationLevel.Draft;
			}
		}

		private void UpdatePlaceholder(ElementPlaceholder elementPlaceholder, ISegment originalTarget, ISegment originalSource,
			Stack<IAbstractMarkupDataContainer> containers)
		{
			var placeholder = GetElement(elementPlaceholder.TagId, originalTarget, originalSource, elementPlaceholder);
			if (placeholder == null)
			{
				placeholder = _segmentBuilder.CreatePlaceholder(elementPlaceholder.TagId,
					elementPlaceholder.TagContent);
			}

			var container = containers.Peek();
			container.Add(placeholder);
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
	}
}
