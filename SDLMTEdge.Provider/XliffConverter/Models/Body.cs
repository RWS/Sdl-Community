using Sdl.Community.MTEdge.Provider.XliffConverter.Converter;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Sdl.Community.MTEdge.Provider.XliffConverter.Models
{
	public class Body
	{		
		public Body() { }

		[XmlElement("trans-unit")]
		public List<TranslationUnit> TranslationUnits { get; set; } = new List<TranslationUnit>();

        internal void Add(Segment sourceSegment)
		{
			if (sourceSegment is null)
			{
				throw new NullReferenceException("Source segment cannot be null");
			}

			TranslationUnits.Add(new TranslationUnit
			{
				Id = TranslationUnits.Count,
				SourceText = sourceSegment.ToXliffString(),
			});
		}

		internal void Add(Segment sourceSegment, Segment targetSegment, string toolId)
		{
			if (sourceSegment is null)
			{
				throw new NullReferenceException("Source segment cannot be null");
			}

			if (targetSegment is null)
			{
				throw new NullReferenceException("Target segment cannot be null");
			}


			var translation = new TargetTranslation
			{
				TargetCulture = targetSegment.Culture,
				Text = targetSegment.ToXliffString()
			};

            var translationOption = new TranslationOption
			{
				ToolId = toolId,
				Translation = translation
            };

			TranslationUnits.Add(new TranslationUnit()
			{
				Id = TranslationUnits.Count,
				SourceText = sourceSegment.ToXliffString(),
				TranslationList = new List<TranslationOption>{translationOption}				
			});
		}
	}
}