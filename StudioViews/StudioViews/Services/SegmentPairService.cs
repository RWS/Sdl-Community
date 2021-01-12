using System;
using System.Globalization;
using Sdl.Community.Toolkit.LanguagePlatform;
using Sdl.Community.Toolkit.LanguagePlatform.Models;
using Sdl.Versioning;

namespace StudioViews.Services
{
	public class SegmentPairService
	{
		private string _productName;
		private SegmentPairProcessor _processor;

		public SegmentPairService(CultureInfo sourceLanguage, CultureInfo targetLanguage)
		{
			SourceLanguage = sourceLanguage;
			TargetLanguage = targetLanguage;
		}

		public CultureInfo SourceLanguage { get; }

		public CultureInfo TargetLanguage { get; }
		
		public SegmentPairProcessor Processor
		{
			get
			{
				if (_processor != null)
				{
					return _processor;
				}

				if (SourceLanguage == null || TargetLanguage == null)
				{
					throw new Exception(
						string.Format(PluginResources.WarningMessage_UnableToParseFileLanguageCannotBeNull, SourceLanguage == null 
						? PluginResources.Label_Source : PluginResources.Label_Target));
				}

				var productName = GetProductName();
				var pathInfo = new PathInfo(productName);
				var settings = new Settings(SourceLanguage, TargetLanguage);

				_processor = new SegmentPairProcessor(settings, pathInfo);
				

				return _processor;
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
