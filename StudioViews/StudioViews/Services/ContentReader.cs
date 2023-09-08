using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentReader : AbstractBilingualContentProcessor
	{
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		
		public ContentReader()
		{
			SegmentPairInfos = new List<SegmentPairInfo>();
		}

		public List<SegmentPairInfo> SegmentPairInfos { get; }

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
				return;
			}

			foreach (var segmentPair in paragraphUnit.SegmentPairs)
			{
				var segmentPairInfo = new SegmentPairInfo
				{
					FileId = _fileProperties.FileConversionProperties.FileId.Id,
					ParagraphUnit = paragraphUnit,
					SegmentPair = segmentPair,
					ParagraphUnitId = paragraphUnit.Properties.ParagraphUnitId.Id,
					SegmentId = segmentPair.Properties.Id.Id
				};

				SegmentPairInfos.Add(segmentPairInfo);
			}
		}
	}
}
