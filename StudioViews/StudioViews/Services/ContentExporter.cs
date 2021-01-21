using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sdl.Community.StudioViews.Model;
using Sdl.Community.Toolkit.LanguagePlatform;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.Versioning;

namespace Sdl.Community.StudioViews.Services
{
	public class ContentExporter : AbstractBilingualContentProcessor
	{
		private readonly List<SegmentPairInfo> _selectedSegments;
		private readonly SegmentBuilder _segmentBuilder;
		private readonly List<string> _projectFilesFiltered;

		private IFileProperties _fileProperties;
		private string _productName;
		private SegmentPairProcessor _segmentPairProcessor;

		public ContentExporter(List<SegmentPairInfo> selectedSegments, SegmentBuilder segmentBuilder)
		{
			_selectedSegments = selectedSegments;
			_segmentBuilder = segmentBuilder;

			_projectFilesFiltered = new List<string>();
			SegmentPairInfos = new List<SegmentPairInfo>();
		}

		public List<SegmentPairInfo> SegmentPairInfos { get; }
		
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
						segmentPairInfo.SourceWordCounts = SegmentPairProcessor.GetSegmentPairInfo(segmentPair)?.SourceWordCounts;
					}
					catch
					{
						// catch all; ignore
					}

					SegmentPairInfos.Add(segmentPairInfo);
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
				var pathInfo = new Toolkit.LanguagePlatform.Models.PathInfo(productName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Toolkit.LanguagePlatform.Models.Settings(SourceLanguage, TargetLanguage), pathInfo);

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
