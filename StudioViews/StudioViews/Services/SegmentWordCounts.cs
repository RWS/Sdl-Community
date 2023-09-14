using System;
using Trados.Community.Toolkit.LanguagePlatform;
using Sdl.Versioning;
using System.Globalization;
using System.IO;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Trados.Community.Toolkit.LanguagePlatform.Models;
using System.Reflection;

namespace Sdl.Community.StudioViews.Services
{
	public class SegmentWordCounts
	{
		private string _productName;
		private SegmentPairProcessor _segmentPairProcessor;
		private readonly CultureInfo _sourceLanguage;
		private readonly CultureInfo _targetLanguage;
		private Assembly asm;
		private Type t;

		public SegmentWordCounts(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
		

			_sourceLanguage = sourceLanguage;
			_targetLanguage = targetLanguage;
		}

		public WordCounts GetWordCounts(ISegmentPair segmentPair)
		{
			return SegmentPairProcessor.GetSegmentPairInfo(segmentPair.Source, segmentPair.Target)?.SourceWordCounts;
		}

		private SegmentPairProcessor SegmentPairProcessor
		{
			get
			{
				if (_segmentPairProcessor != null)
				{
					return _segmentPairProcessor;
				}

				if (_sourceLanguage == null || _targetLanguage == null)
				{
					throw new Exception(
						string.Format(PluginResources.Error_Message_Unable_To_Parse_File_Language_Null, _sourceLanguage == null
							? "Source" : "Target"));
				}

				var productName = GetProductName();
				var pathInfo = new PathInfo(productName);

				_segmentPairProcessor = new SegmentPairProcessor(
					new Settings(_sourceLanguage, _targetLanguage), pathInfo);

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
