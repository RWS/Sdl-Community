using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using StudioViews.Model;

namespace StudioViews.Services
{
	public class ContentExporter : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentPairContext> _selectedSegments;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly List<string> _projectFilesFiltered;

		private IFileProperties _fileProperties;
		
		public ContentExporter(List<SegmentPairContext> selectedSegments, SegmentBuilder segmentBuilder)
		{
			_selectedSegments = selectedSegments;
			_segmentBuilder = segmentBuilder;

			_projectFilesFiltered = new List<string>();
		}

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public override void Initialize(IDocumentProperties documentInfo)
		{
			SourceLanguage = documentInfo.SourceLanguage.CultureInfo;
			TargetLanguage = documentInfo.TargetLanguage?.CultureInfo ?? SourceLanguage;

			base.Initialize(documentInfo);
		}

		public override void SetFileProperties(IFileProperties fileInfo)
		{
			_fileProperties = fileInfo;
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (paragraphUnit.IsStructure )
			{
				
				UpdateParagraphUnit(paragraphUnit);
				return;
			}

			if (!paragraphUnit.SegmentPairs.Any())
			{
				return;
			}

			var segmentPairs = new List<ISegmentPair>();
			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				if (_selectedSegments.Exists(a =>
					a.ParagraphUnitId == paragraphUnit.Properties.ParagraphUnitId.Id &&
					a.SegmentId == segmentPair.Properties.Id.Id))
				{
					segmentPairs.Add(segmentPair);
				}
			}

			AddSegmentPairs(paragraphUnit, segmentPairs);
		}

		private void AddSegmentPairs(IParagraphUnit paragraphUnit, IReadOnlyCollection<ISegmentPair> segmentPairs)
		{
			if (segmentPairs?.Count <= 0 || segmentPairs?.FirstOrDefault() == null)
			{
				return;
			}

			var newParagraphUnit = _segmentBuilder.CreateParagraphUnit(paragraphUnit.Properties.LockType);
			newParagraphUnit.Properties = paragraphUnit.Properties;

			foreach (var segmentPair in segmentPairs)
			{
				newParagraphUnit.Source.Add(segmentPair.Source.Clone() as ISegment);
				newParagraphUnit.Target.Add(segmentPair.Target.Clone() as ISegment);
			}

			UpdateParagraphUnit(newParagraphUnit);
		}

		private void UpdateParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (!_selectedSegments.Exists(a => a.FileId == _fileProperties.FileConversionProperties.FileId.Id))
			{
				return;
			}
			
			if (!_projectFilesFiltered.Exists(a => a == _fileProperties.FileConversionProperties.FileId.Id))
			{
				base.SetFileProperties(_fileProperties);
				_projectFilesFiltered.Add(_fileProperties.FileConversionProperties.FileId.Id);
			}
			
			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
