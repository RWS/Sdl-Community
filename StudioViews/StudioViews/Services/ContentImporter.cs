using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentImporter : AbstractBilingualContentProcessor
	{
		private readonly List<IParagraphUnit> _updatedParagraphUnits;

		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;

		public ContentImporter(List<IParagraphUnit> updatedParagraphUnits)
		{
			_updatedParagraphUnits = updatedParagraphUnits;
		}

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

			var updatedParagraphUnit = _updatedParagraphUnits.FirstOrDefault(a =>
				a.Properties.ParagraphUnitId.Id == paragraphUnit.Properties.ParagraphUnitId.Id);

			if (updatedParagraphUnit != null)
			{
				foreach (var segmentPair in paragraphUnit.SegmentPairs)
				{
					var updatedSegmentPair =
						updatedParagraphUnit.SegmentPairs.FirstOrDefault(a =>
							a.Properties.Id.Id == segmentPair.Properties.Id.Id);

					if (updatedSegmentPair != null)
					{
						segmentPair.Target.Clear();
						foreach (var item in updatedSegmentPair.Target)
						{
							segmentPair.Target.Add(item);
						}

						segmentPair.Properties = updatedSegmentPair.Properties;
					}
				}
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}
	}
}
