using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Multilingual.Excel.FileType.BatchTasks.Settings;
using Multilingual.Excel.FileType.Constants;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Services
{
	public class ContentImporter : AbstractBilingualContentProcessor
	{
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private List<IParagraphUnit> _updatedParagraphUnits;
		private readonly MultilingualExcelImportSettings _settings;
		private readonly FilterItemService _filterItemService;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly SegmentVisitor _segmentVisitor;


		public ContentImporter(List<IParagraphUnit> updatedParagraphUnits, MultilingualExcelImportSettings settings,
			FilterItemService filterItemService, SegmentBuilder segmentBuilder, SegmentVisitor segmentVisitor)
		{
			_updatedParagraphUnits = updatedParagraphUnits;
			_settings = settings;
			_filterItemService = filterItemService;
			_segmentBuilder = segmentBuilder;
			_segmentVisitor = segmentVisitor;
		}

		public int UpdatedSegments { get; private set; }

		public int ExcludedSegments { get; private set; }

		public int TotalSegments { get; private set; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

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
			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			var updatedParagraphUnit = _updatedParagraphUnits.FirstOrDefault(a =>
				a.Properties.ParagraphUnitId.Id == paragraphUnit.Properties.ParagraphUnitId.Id);

			TotalSegments += paragraphUnit.SegmentPairs.Count();

			if (updatedParagraphUnit != null)
			{

				// PAH 2022/02/08: Excluded - paragraphUnitProvider needs to be revised!
				//if (paragraphUnit.SegmentPairs.Any() && updatedParagraphUnit.SegmentPairs.Any())
				//{
				//	// both source and target are segmented; 
				//	var result = _paragraphUnitProvider.GetUpdatedParagraphUnit(
				//		paragraphUnit, updatedParagraphUnit, _settings);

				//	ExcludedSegments += result.ExcludedSegments;
				//	UpdatedSegments += result.UpdatedSegments;

				//	base.ProcessParagraphUnit(result.Paragraph);
				//	return;
				//}

				var multilingualContextInfo = paragraphUnit.Properties.Contexts?.Contexts?.FirstOrDefault(a =>
					a.ContextType == FiletypeConstants.MultilingualParagraphUnit);
				var targetMultilingualContextInfo = updatedParagraphUnit.Properties.Contexts.Contexts?.FirstOrDefault(a =>
					a.ContextType == FiletypeConstants.MultilingualParagraphUnit);
				if (targetMultilingualContextInfo != null)
				{
					var characterLimitation = targetMultilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelCharacterLimitationTarget) ?? "0";
					var pixelLimitation = targetMultilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelPixelLimitationTarget) ?? "0";
					var pixelFontName = targetMultilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontNameTarget) ?? string.Empty;
					var pixelFontSize = targetMultilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontSizeTarget) ?? "0";
					var filterBackgroundColor = targetMultilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelFilterBackgroundColorTarget) ?? "0";
					var lockSegments = targetMultilingualContextInfo.GetMetaData(FiletypeConstants.MultilingualExcelFilterLockSegmentsTarget) ?? string.Empty;

					multilingualContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelCharacterLimitationTarget, characterLimitation);
					multilingualContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelPixelLimitationTarget, pixelLimitation);
					multilingualContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelPixelFontNameTarget, pixelFontName);
					multilingualContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelPixelFontSizeTarget, pixelFontSize);
					multilingualContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelFilterBackgroundColorTarget, filterBackgroundColor);
					multilingualContextInfo.SetMetaData(FiletypeConstants.MultilingualExcelFilterLockSegmentsTarget, lockSegments);
				}

				if (paragraphUnit.SegmentPairs.Any())
				{
					// source is segmented; target is not!
					var firstSegmentPair = paragraphUnit.SegmentPairs.First();
					var firstUpdatedSegmentPair = updatedParagraphUnit.SegmentPairs.First();

					var isExcluded = SegmentIsExcluded(_settings.ExcludeFilterIds, firstSegmentPair);

					var originalTargetText = GetSegmentTextToPlain(firstSegmentPair.Target, false);
					var updatedTargetSegment = GetSegmentFromParagraph(firstSegmentPair.Properties, updatedParagraphUnit.Target);
					var updatedTargetText = GetSegmentTextToPlain(updatedTargetSegment, false);

					if (string.Compare(originalTargetText, updatedTargetText, StringComparison.Ordinal) == 0)
					{
						isExcluded = true;
					}

					if (!isExcluded && (string.IsNullOrEmpty(originalTargetText) || _settings.OverwriteTranslations))
					{
						UpdatedSegments++;

						SetTranslationOrigin(firstSegmentPair, _settings.OriginSystem);
						SetConfirmationLevel(firstSegmentPair, _settings.StatusTranslationUpdatedId);

						SetIsLocked(firstSegmentPair, firstUpdatedSegmentPair, paragraphUnit.SegmentPairs);
						ImportTargetContent(firstSegmentPair.Target, updatedTargetSegment);
						UpdateContextInfo(paragraphUnit, updatedParagraphUnit);
						//UpdateComments(paragraphUnit, updatedParagraphUnit);
					}
					else
					{
						ExcludedSegments++;
					}
				}
				//else
				//{
				//	UpdatedSegments++;

				//	paragraphUnit.Target.Clear();


				//	// source and target are not segmented; this should never happen and not expected by the framework!
				//	foreach (var abstractMarkupData in updatedParagraphUnit.Target)
				//	{
				//		paragraphUnit.Target.Add(abstractMarkupData.Clone() as IAbstractMarkupData);
				//	}
				//}
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private void UpdateContextInfo(IParagraphUnit paragraphUnit, IParagraphUnit updatedParagraphUnit)
		{
			if (updatedParagraphUnit.Properties?.Contexts?.Contexts?.Count > 0)
			{
				foreach (var contextInfo in updatedParagraphUnit.Properties.Contexts.Contexts)
				{
					if (contextInfo.ContextType == Constants.FiletypeConstants.MultilingualExcelContextInformation)
					{
						var existingEpContext = paragraphUnit.Properties?.Contexts?.Contexts?.FirstOrDefault(a =>
							a.ContextType == FiletypeConstants.MultilingualExcelContextInformation
							&& a.DisplayName == contextInfo.DisplayName && a.Description == contextInfo.Description);

						if (existingEpContext == null)
						{
							paragraphUnit.Properties?.Contexts?.Contexts?.Add(contextInfo);
						}
					}
				}
			}
		}

		private void UpdateComments(IParagraphUnit paragraphUnit, IParagraphUnit updatedParagraphUnit)
		{
			if (updatedParagraphUnit.Properties?.Comments?.Comments != null)
			{
				foreach (var comment in updatedParagraphUnit.Properties.Comments.Comments)
				{
					var commentsClone = paragraphUnit.Properties.Comments?.Clone() as ICommentProperties;

					paragraphUnit.Properties.Comments = PropertiesFactory.CreateCommentProperties();
					if (commentsClone != null)
					{
						paragraphUnit.Properties.Comments.AddComments(commentsClone);
					}

					var existingComment = paragraphUnit.Properties.Comments.Comments.FirstOrDefault(a => a.Text == comment.Text);
					if (existingComment == null)
					{
						paragraphUnit.Properties.Comments.Add(comment.Clone() as IComment);
					}
				}
			}
		}

		private static void ImportTargetContent(ISegment originalSegment, ISegment updatedSegment)
		{
			originalSegment.Clear();
			foreach (var abstractMarkupData in updatedSegment)
			{
				originalSegment.Add(abstractMarkupData.Clone() as IAbstractMarkupData);
			}
		}

		private bool SegmentIsExcluded(List<string> excludeFilterIds, ISegmentPair segmentPair)
		{
			var segmentIsExcluded = false;
			if (segmentPair != null && excludeFilterIds?.Count > 0)
			{
				var status = segmentPair.Properties.ConfirmationLevel.ToString();
				var match = _filterItemService.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin);

				if ((segmentPair.Properties.IsLocked && excludeFilterIds.Exists(a => a == "Locked"))
					|| excludeFilterIds.Exists(a => a == status)
					|| excludeFilterIds.Exists(a => a == match))
				{
					segmentIsExcluded = true;
				}
			}

			return segmentIsExcluded;
		}

		private string GetSegmentTextToPlain(ISegment segment, bool ignoreTags)
		{
			_segmentVisitor.IgnoreTags = ignoreTags;
			_segmentVisitor.VisitSegment(segment);

			return _segmentVisitor.Text;
		}


		private ISegment GetSegmentFromParagraph(ISegmentPairProperties segmentPairProperties, IParagraph paragraphUnit)
		{
			var segment = _segmentBuilder.CreateSegment(segmentPairProperties);
			foreach (var abstractMarkupData in paragraphUnit)
			{
				if (abstractMarkupData is ISegment subSegment)
				{
					foreach (var item in subSegment)
					{
						segment.Add(item.Clone() as IAbstractMarkupData);
					}
				}
				else
				{
					segment.Add(abstractMarkupData.Clone() as IAbstractMarkupData);
				}
			}

			return segment;
		}

		private static void SetIsLocked(ISegmentPair firstSegmentPair, ISegmentPair firstUpdatedSegmentPair, IEnumerable<ISegmentPair> segmentPairs)
		{
			if (firstUpdatedSegmentPair.Properties.IsLocked)
			{
				foreach (var segmentPair in segmentPairs)
				{
					segmentPair.Properties.IsLocked = true;
				}
			}
			else
			{
				firstSegmentPair.Target.Properties.IsLocked = firstSegmentPair.Properties.IsLocked;
			}
		}

		private void SetConfirmationLevel(ISegmentPair firstSegmentPair, string statusTranslationUpdatedId)
		{
			if (!string.IsNullOrEmpty(statusTranslationUpdatedId))
			{
				var success =
					Enum.TryParse<ConfirmationLevel>(statusTranslationUpdatedId, true, out var confirmationLevel);
				var statusTranslationUpdated = success
					? confirmationLevel
					: firstSegmentPair.Properties.ConfirmationLevel;

				firstSegmentPair.Target.Properties.ConfirmationLevel = statusTranslationUpdated;
			}
		}

		private void SetTranslationOrigin(ISegmentPair segmentPair, string originSystem)
		{
			if (segmentPair.Properties.TranslationOrigin != null)
			{
				var currentTranslationOrigin = (ITranslationOrigin)segmentPair.Properties.TranslationOrigin.Clone();
				segmentPair.Properties.TranslationOrigin.OriginBeforeAdaptation = currentTranslationOrigin;
			}
			else
			{
				segmentPair.Properties.TranslationOrigin = _segmentBuilder.CreateTranslationOrigin();
			}

			segmentPair.Target.Properties.TranslationOrigin.MatchPercent = byte.Parse("0");
			segmentPair.Target.Properties.TranslationOrigin.OriginSystem = originSystem;
			segmentPair.Target.Properties.TranslationOrigin.OriginType = DefaultTranslationOrigin.Interactive;
			segmentPair.Target.Properties.TranslationOrigin.IsStructureContextMatch = false;
			segmentPair.Target.Properties.TranslationOrigin.TextContextMatchLevel = TextContextMatchLevel.None;

			segmentPair.Target.Properties.TranslationOrigin.SetMetaData("last_modified_by", originSystem);
			segmentPair.Target.Properties.TranslationOrigin.SetMetaData("modified_on", FormatAsInvariantDateTime(DateTime.UtcNow));
		}

		private string FormatAsInvariantDateTime(DateTime date)
		{
			return date.ToString(DateTimeFormatInfo.InvariantInfo);
		}
	}
}