using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.Models;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SegmentPairInfo = Multilingual.Excel.FileType.Models.SegmentPairInfo;


namespace Multilingual.Excel.FileType.Services
{
	public class ContentReader : AbstractBilingualContentProcessor
	{
		private IFileProperties _fileProperties;

		private IParagraphUnit _paragraphUnit;

		private int _excelSheetIndex;
		private string _excelSheetName;
		private uint _excelRowIndex;
		private int _excelCharacterLimitationSource;
		private int _excelPixelLimitationSource;
		private string _excelPixelFontNameSource;
		private float _excelPixelFontSizeSource;
		private bool _isCATA;
		private string _excelFilterBackgroundColorSource;
		private bool _excelFilterLockSegments;

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
					a.ContextType == FiletypeConstants.MultilingualParagraphUnit);

			if (multilingualParagraphUnitContext != null)
			{
				_paragraphUnit = paragraphUnit;
				_excelSheetIndex = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelSheetIndex));
				_excelSheetName = multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelSheetName);
				_excelRowIndex = Convert.ToUInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelRowIndex));
				_excelCharacterLimitationSource = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelCharacterLimitationSource) ?? "0");
				_excelPixelLimitationSource = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelLimitationSource) ?? "0");
				_excelPixelFontNameSource = multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontNameSource ?? string.Empty);
				_excelPixelFontSizeSource = Convert.ToSingle(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontSizeSource) ?? "0");
				_isCATA = Convert.ToBoolean(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.IsCDATA));
				_excelFilterBackgroundColorSource = multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelFilterBackgroundColorSource ?? string.Empty);
				_excelFilterLockSegments = Convert.ToBoolean(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelFilterLockSegmentsSource));
			}

			if (paragraphUnit.IsStructure)
			{
				base.ProcessParagraphUnit(paragraphUnit);
				return;
			}

			var paragraphUnitInfo = new ParagraphUnitInfo
			{
				ExcelSheetIndex = _excelSheetIndex,
				ExcelSheetName = _excelSheetName,
				ExcelRowIndex = _excelRowIndex,
				ExcelCharacterLimitation = _excelCharacterLimitationSource,
				ExcelPixelLimitation = _excelPixelLimitationSource,
				ExcelPixelFontName = _excelPixelFontNameSource,
				ExcelPixelFontSize = _excelPixelFontSizeSource,
				ExcelFilterBackgroundColor = _excelFilterBackgroundColorSource,
				ExcelFilterLockSegments = _excelFilterLockSegments,
				IsCDATA = _isCATA,
				ParagraphUnit = paragraphUnit,
				ParagraphUnitId = paragraphUnit.Properties.ParagraphUnitId.Id,
				FileId = _fileProperties.FileConversionProperties.FileId.Id,
				SegmentPairs = new List<SegmentPairInfo>()
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
			if (isSubContent && !ParagraphUnitInfos.Exists(a => a.ExcelSheetIndex == _excelSheetIndex && a.ExcelRowIndex == _excelRowIndex))
			{
				var paragraphUnitInfo = new ParagraphUnitInfo
				{
					ExcelSheetIndex = _excelSheetIndex,
					ExcelSheetName = _excelSheetName,
					ExcelRowIndex = _excelRowIndex,
					ExcelCharacterLimitation = _excelCharacterLimitationSource,
					ExcelPixelLimitation = _excelPixelLimitationSource,
					ExcelPixelFontName = _excelPixelFontNameSource,
					ExcelPixelFontSize = _excelPixelFontSizeSource,
					ExcelFilterBackgroundColor = _excelFilterBackgroundColorSource,
					ExcelFilterLockSegments = _excelFilterLockSegments,
					IsCDATA = _isCATA,
					ParagraphUnit = _paragraphUnit,
					ParagraphUnitId = _paragraphUnit.Properties.ParagraphUnitId.Id,
					FileId = _fileProperties.FileConversionProperties.FileId.Id,
					SegmentPairs = new List<SegmentPairInfo>()
				};

				ParagraphUnitInfos.Add(paragraphUnitInfo);
			}
		}
	}
}
