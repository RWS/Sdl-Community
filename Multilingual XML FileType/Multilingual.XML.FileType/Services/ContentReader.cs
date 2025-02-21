using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Multilingual.XML.FileType.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SegmentPairInfo = Multilingual.XML.FileType.Models.SegmentPairInfo;


namespace Multilingual.XML.FileType.Services
{
	public class ContentReader : AbstractBilingualContentProcessor
	{
		private IFileProperties _fileProperties;

		private bool _isCDATA;

		private int _paragraphIndex;

		private IParagraphUnit _paragraphUnit;

		public ContentReader()
		{
			ParagraphUnitInfos = new List<ParagraphUnitInfo>();
		}

		public List<ParagraphUnitInfo> ParagraphUnitInfos { get; }

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
			CheckAddMissingEmptySubContentMarkers();

			_fileProperties = fileInfo;
			base.SetFileProperties(fileInfo);
		}

		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			var multilingualParagraphUnitContext =
				paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a =>
					a.ContextType == Constants.MultilingualParagraphUnit);
			if (multilingualParagraphUnitContext != null)
			{
				_paragraphUnit = paragraphUnit;
				_paragraphIndex = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(Constants.MultilingualParagraphUnitIndex));
				_isCDATA = Convert.ToBoolean(multilingualParagraphUnitContext.GetMetaData(Constants.IsCDATA));
			}

			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}


			var paragraphUnitInfo = new ParagraphUnitInfo
			{
				ParagraphUnitIndex = _paragraphIndex,
				ParagraphUnit = paragraphUnit,
				ParagraphUnitId = paragraphUnit.Properties.ParagraphUnitId.Id,
				FileId = _fileProperties.FileConversionProperties.FileId.Id,
				SegmentPairs = new List<SegmentPairInfo>(),
				IsCDATA = _isCDATA
			};

			if (paragraphUnit.SegmentPairs.Any())
			{
				foreach (var segmentPair in paragraphUnit.SegmentPairs)
				{
					var segmentPairInfo = new SegmentPairInfo
					{
						SegmentPair = segmentPair,
						SegmentId = segmentPair.Properties.Id.Id
					};

					paragraphUnitInfo.SegmentPairs.Add(segmentPairInfo);
				}
			}

			ParagraphUnitInfos.Add(paragraphUnitInfo);


			base.ProcessParagraphUnit(paragraphUnit);
		}



		private void CheckAddMissingEmptySubContentMarkers()
		{
			if (_fileProperties?.FileConversionProperties == null)
			{
				return;
			}

			var isSubContent = _fileProperties.FileConversionProperties.MetaDataContainsKey("SDL_SUBCONTENT_STREAM_LEVEL");
			if (isSubContent && !ParagraphUnitInfos.Exists(a => a.ParagraphUnitIndex == _paragraphIndex))
			{
				var paragraphUnitInfo = new ParagraphUnitInfo
				{
					ParagraphUnitIndex = _paragraphIndex,
					ParagraphUnit = _paragraphUnit,
					ParagraphUnitId = _paragraphUnit.Properties.ParagraphUnitId.Id,
					FileId = _fileProperties.FileConversionProperties.FileId.Id,
					SegmentPairs = new List<SegmentPairInfo>(),
					IsCDATA = _isCDATA
				};
				ParagraphUnitInfos.Add(paragraphUnitInfo);
			}
		}
	}
}
