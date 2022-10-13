using System.Collections.Generic;
using Multilingual.XML.FileType.Models;
using Newtonsoft.Json;
using Sdl.FileTypeSupport.Framework.Core.Settings.Serialization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.XML.FileType.FileType.Settings
{
	public class PlaceholderPatternSettings : AbstractSettingsClass
	{
		private const string PlaceablePatternsProcessSetting = "PlaceholderPatternsProcess";
		private const string PlaceablePatternsSetting = "PlaceholderPatterns";

		private readonly bool _placeablePatternsProcessDefault;
		private readonly List<PlaceholderPattern> _placeablePatternsDefault;

		public PlaceholderPatternSettings()
		{
			_placeablePatternsProcessDefault = false;
			_placeablePatternsDefault = new List<PlaceholderPattern>
			{
				new PlaceholderPattern
				{
					SegmentationHint = SegmentationHint.IncludeWithText,
					Pattern = @"{{*\s*[^\s]+\s*}}*", Selected = true, Order = 0,
					Description = @"Placeholder tags using one or more curly brackets and containing any content in the form {​​​​​​​​{​​​​​​​​{​​​​​​​​placehoder_text}​​​​​​​​}​​​​​​​​}​​​​​​​​"
				},
				new PlaceholderPattern
				{
					SegmentationHint = SegmentationHint.IncludeWithText,
					Pattern = @"{\d+}", Selected = true, Order = 1, 
					Description = @"Placeholder tags using a single pair of curly brackets and containing one or more consecutive numbers in the form {​​​​​​​​1}​​​​​​​​ or {​​​​​​​​276}​​​​​​​​ for example"
				},
				new PlaceholderPattern
				{
					SegmentationHint = SegmentationHint.IncludeWithText,
					Pattern = @"%\w+%", Selected = false, Order = 2, 
					Description = @"Placeholder tags using percentage symbols surrounding the placeholder variable in the form %placeholder%"
				},
				new PlaceholderPattern
				{
					SegmentationHint = SegmentationHint.IncludeWithText,
					Pattern = @"\\\+\w+\\-", Selected = false, Order = 3, 
					Description = @"Placeholder tags delimited by non-letter characters in the form \+placeholder\-"
				}
			};

			ResetToDefaults();
		}

		public bool PlaceablePatternsProcess { get; set; }

		public List<PlaceholderPattern> PlaceablePatterns { get; set; }

		public override string SettingName => "MultilingualXmlPlaceholderPatternsSettings";

		public override void Read(IValueGetter valueGetter)
		{
			PlaceablePatternsProcess = valueGetter.GetValue(PlaceablePatternsProcessSetting, _placeablePatternsProcessDefault);
			PlaceablePatterns = JsonConvert.DeserializeObject<List<PlaceholderPattern>>(valueGetter.GetValue(PlaceablePatternsSetting,
				JsonConvert.SerializeObject(_placeablePatternsDefault)));
		}

		public override void Save(IValueProcessor valueProcessor)
		{
			valueProcessor.Process(PlaceablePatternsProcessSetting, PlaceablePatternsProcess, _placeablePatternsProcessDefault);
			valueProcessor.Process(PlaceablePatternsSetting, JsonConvert.SerializeObject(PlaceablePatterns),
				JsonConvert.SerializeObject(_placeablePatternsDefault));
		}

		public override object Clone()
		{
			return new PlaceholderPatternSettings
			{
				PlaceablePatternsProcess = PlaceablePatternsProcess,
				PlaceablePatterns = new List<PlaceholderPattern>(PlaceablePatterns)
			};
		}

		public override bool Equals(ISettingsClass other)
		{
			return other is PlaceholderPatternSettings otherSetting &&
				   otherSetting.PlaceablePatternsProcess == PlaceablePatternsProcess &&
				   otherSetting.PlaceablePatterns == PlaceablePatterns;
		}

		public sealed override void ResetToDefaults()
		{
			PlaceablePatternsProcess = _placeablePatternsProcessDefault;
			PlaceablePatterns = _placeablePatternsDefault;
		}
	}
}
