using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using LanguageWeaverProvider.XliffConverter.Converter;
using Sdl.LanguagePlatform.Core;

namespace LanguageWeaverProvider.XliffConverter.Models
{
	public class Body
	{		
		public Body()
		{
			TranslationUnits = new List<TranslationUnit>();
		}

		[XmlElement("trans-unit")]
		public List<TranslationUnit> TranslationUnits { get; set; }

		internal void Add(Segment sourceSegment)
		{
			if (sourceSegment == null)
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
			if (sourceSegment == null)
			{
				throw new NullReferenceException("Source segment cannot be null");
			}

			if (targetSegment == null)
			{
				throw new NullReferenceException("Target segment cannot be null");
			}


			var translationOption = new TranslationOption
			{
				ToolId = toolId,
				Translation = new TargetTranslation
				{
					TargetCulture = targetSegment.Culture,
					Text = targetSegment.ToXliffString()
				}
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
