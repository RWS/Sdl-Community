﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Sdl.Community.ApplyTMTemplate.Models.Interfaces;
using Sdl.LanguagePlatform.Core.Segmentation;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class SegmentationRulesResource : IResource
	{
		private readonly XmlNode _resource;

		public SegmentationRulesResource(XmlNode resource)
		{
			_resource = resource;
		}

		public void AddLanguageResourceToBundle(LanguageResourceBundle langResBundle)
		{
			var segRules = Encoding.UTF8.GetString(Convert.FromBase64String(_resource.InnerText));

			var xmlSerializer = new XmlSerializer(typeof(SegmentationRules));

			var stringReader = new StringReader(segRules);
			var segmentRules = (SegmentationRules)xmlSerializer.Deserialize(stringReader);

			langResBundle.SegmentationRules = segmentRules;
		}
	}
}