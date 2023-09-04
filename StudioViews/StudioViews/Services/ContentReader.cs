using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Trados.Community.Toolkit.LanguagePlatform;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.Versioning;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentReader : AbstractBilingualContentProcessor
	{
		private IFileProperties _fileProperties;
		private IDocumentProperties _documentProperties;
		private string _productName;
		private SegmentPairProcessor _segmentPairProcessor;

		public ContentReader()
		{
			SegmentPairInfos = new List<SegmentPairInfo>();
			IgnoreWordCountInfo = false;
		}

		public List<SegmentPairInfo> SegmentPairInfos { get; }

		public CultureInfo SourceLanguage { get; private set; }

		public CultureInfo TargetLanguage { get; private set; }

		public bool IgnoreWordCountInfo { get; set; }

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

				try
				{
					if (!IgnoreWordCountInfo)
					{
						segmentPairInfo.SourceWordCounts =
							SegmentPairProcessor.GetSegmentPairInfo(segmentPair)?.SourceWordCounts;
					}
				}
				catch
				{
					// catch all; ignore
				}

				SegmentPairInfos.Add(segmentPairInfo);
			}
		}

		private SegmentPairProcessor SegmentPairProcessor
		{
			get
			{
				if (_segmentPairProcessor != null)
				{
					return _segmentPairProcessor;
				}

				if (SourceLanguage == null || TargetLanguage == null)
				{
					throw new Exception(
						string.Format(PluginResources.Error_Message_Unable_To_Parse_File_Language_Null, SourceLanguage == null
							? "Source" : "Target"));
				}

				var productName = GetProductName();
				var pathInfo = new Trados.Community.Toolkit.LanguagePlatform.Models.PathInfo(productName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Trados.Community.Toolkit.LanguagePlatform.Models.Settings(SourceLanguage, TargetLanguage), pathInfo);

				return _segmentPairProcessor;
			}
		}

		private string GetProductName()
		{
			if (!string.IsNullOrEmpty(_productName))
			{
				return _productName;
			}

			var studioVersionService = new StudioVersionService();
			var studioVersion = studioVersionService.GetStudioVersion();
			if (studioVersion != null)
			{
				_productName = studioVersion.StudioDocumentsFolderName;
			}

			return _productName;
		}
	}
}
