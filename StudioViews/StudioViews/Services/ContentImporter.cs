using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.StudioViews.Providers;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentImporter : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentPairInfo> _updatedSegmentPairs;
		private readonly List<string> _excludeFilterIds;
		private readonly FilterItemService _filterItemService;
		private readonly ParagraphUnitProvider _paragraphUnitProvider;

		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;

		public ContentImporter(List<SegmentPairInfo> updatedSegmentPairs, List<string> excludeFilterIds,
			FilterItemService filterItemService, ParagraphUnitProvider paragraphUnitProvider)
		{
			_updatedSegmentPairs = updatedSegmentPairs;
			_excludeFilterIds = excludeFilterIds;
			_filterItemService = filterItemService;
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

			TotalSegments += paragraphUnit.SegmentPairs.Count();

			if (updatedSegmentPairs.Any())
			{
				var updatedParagraphUnit = updatedSegmentPairs.FirstOrDefault()?.ParagraphUnit;

				// Import segment pairs across multiple paragraph units from different import files
				foreach (var updatedSegmentPair in updatedSegmentPairs)
				{
					var segmentPair = updatedParagraphUnit?.GetSegmentPair(new SegmentId(updatedSegmentPair.SegmentId));
					if (segmentPair == null)
					{
						updatedParagraphUnit?.Source.Add(updatedSegmentPair.SegmentPair.Source.Clone() as ISegment);
						updatedParagraphUnit?.Target.Add(updatedSegmentPair.SegmentPair.Target.Clone() as ISegment);
					}
				}

				var result = _paragraphUnitProvider.GetUpdatedParagraphUnit(
					paragraphUnit, updatedParagraphUnit, _excludeFilterIds);

				ExcludedSegments += result.ExcludedSegments;
				UpdatedSegments += result.UpdatedSegments;

				base.ProcessParagraphUnit(result.Paragraph);
			}
			else
			{
				base.ProcessParagraphUnit(paragraphUnit);
			}
		}
	}
}
