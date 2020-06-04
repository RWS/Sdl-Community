using System;
using System.Collections.Generic;
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

		private Dictionary<string, List<IComment>> Comments { get; set; }

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
				var segment = paragraph.SegmentPairs.FirstOrDefault(a => a.Id == segmentPair.Properties.Id.Id);
				if (segment == null)
				{
					continue;
				}

				var targetSegment = segmentPair.Target;

				var originalSource = (ISegment) segmentPair.Source.Clone();
				var originalTarget = (ISegment)targetSegment.Clone();

				// clear the existing content from the target segment
				targetSegment.Clear();

				var containers = new Stack<IAbstractMarkupDataContainer>();
				containers.Push(targetSegment);

				var lockedContentId = 0;

				foreach (var element in segment.Target.Elements)
				{
					if (element is ElementComment elementComment)
					{
						var comments = Comments.FirstOrDefault(a => a.Key == elementComment.Id);
						var newestComment = comments.Value.LastOrDefault();
						if (newestComment == null)
						{
							continue;
						}

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

					if (element is ElementTagPair elementTagPair)
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

					if (element is ElementLocked elementLocked)
					{						
						if (elementLocked.Type == Element.TagType.OpeningTag)
						{
							var lockedContent = GetElement(lockedContentId.ToString(), originalTarget, 
								originalSource, elementLocked);
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
					}

					if (element is ElementPlaceholder elementPlaceholder)
					{
						var placeholder = GetElement(elementPlaceholder.TagId, originalTarget, 
							originalSource, elementPlaceholder);
						if (placeholder == null)
						{
							placeholder = _segmentBuilder.CreatePlaceholder(elementPlaceholder.TagId, 
								elementPlaceholder.TagContent);
						}

						var container = containers.Peek();
						container.Add(placeholder);
					}

					if (element is ElementText elementText && !string.IsNullOrEmpty(elementText.Text))
					{
						var text = _segmentBuilder.Text(elementText.Text);
						var container = containers.Peek();
						container.Add(text);
					}
				}

				//TODO finalize setting the translation origin and properties
				if (targetSegment.Properties.TranslationOrigin != null)
				{
					var currentTranslationOrigin = (ITranslationOrigin)targetSegment.Properties.TranslationOrigin.Clone();
					targetSegment.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;
					targetSegment.Properties.TranslationOrigin.MatchPercent = byte.Parse("0");
				}

				targetSegment.Properties.ConfirmationLevel = ConfirmationLevel.Draft;				
			}

			base.ProcessParagraphUnit(paragraphUnit);
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
