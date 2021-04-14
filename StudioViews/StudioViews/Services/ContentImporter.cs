using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentImporter : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentPairInfo> _updatedSegmentPairs;
		private readonly List<string> _excludeFilterIds;
		private readonly FilterItemService _filterItemService;
		private readonly List<AnalysisBand> _analysisBands;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;

		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;

		public ContentImporter(List<SegmentPairInfo> updatedSegmentPairs, List<string> excludeFilterIds,
			FilterItemService filterItemService, List<AnalysisBand> analysisBands, ParagraphUnitProvider paragraphUnitProvider)
		{
			_updatedSegmentPairs = updatedSegmentPairs;
			_excludeFilterIds = excludeFilterIds;
			_filterItemService = filterItemService;
			_analysisBands = analysisBands;
			_paragraphUnitProvider = paragraphUnitProvider;

			UpdatedSegments = 0;
			ExcludedSegments = 0;
			TotalSegments = 0;
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
			if (paragraphUnit.IsStructure || !paragraphUnit.SegmentPairs.Any())
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			var updatedSegmentPairs = _updatedSegmentPairs.Where(a =>
				a.ParagraphUnitId == paragraphUnit.Properties.ParagraphUnitId.Id).ToList();

			TotalSegments = paragraphUnit.SegmentPairs.Count();

			if (updatedSegmentPairs.Any())
			{
				foreach (var segmentPair in paragraphUnit.SegmentPairs)
				{
					var updatedSegmentPair = updatedSegmentPairs.FirstOrDefault(a =>
						a.SegmentId == segmentPair.Properties.Id.Id);

					if (updatedSegmentPair?.SegmentPair?.Target == null 
					    || IsSame(segmentPair.Target, updatedSegmentPair.SegmentPair.Target)
					    || IsEmpty(updatedSegmentPair.SegmentPair.Target))
					{
						continue;
					}
					
					
					if (_excludeFilterIds.Count > 0)
					{
						var status = segmentPair.Properties.ConfirmationLevel.ToString();
						var match = _filterItemService.GetTranslationOriginType(segmentPair.Target.Properties.TranslationOrigin,
							_analysisBands);

						if ((segmentPair.Properties.IsLocked && _excludeFilterIds.Exists(a => a == "Locked"))
							|| _excludeFilterIds.Exists(a => a == status)
							|| _excludeFilterIds.Exists(a => a == match))
						{
							ExcludedSegments++;
							continue;
						}
					}

					segmentPair.Target.Clear();
					foreach (var item in updatedSegmentPair.SegmentPair.Target)
					{
						segmentPair.Target.Add(item.Clone() as IAbstractMarkupData);
					}

					segmentPair.Properties = updatedSegmentPair.SegmentPair.Properties;

					UpdatedSegments++;
				}
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}

		private static bool IsSame(ISegment segment, ISegment updatedSegment)
		{
			var originalTarget = segment.ToString();
			var updatedTarget = updatedSegment.ToString();

			var isSame = (originalTarget == updatedTarget) &&
			             (segment.Properties.IsLocked == updatedSegment.Properties.IsLocked) &&
			             (segment.Properties.ConfirmationLevel == updatedSegment.Properties.ConfirmationLevel);

			return isSame;
		}

		private static bool IsEmpty(ISegment segment)
		{
			return segment.ToString().Trim() == string.Empty;
		}
	}
}
