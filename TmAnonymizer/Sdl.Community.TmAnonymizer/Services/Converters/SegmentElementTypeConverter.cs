using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.SdlTmAnonymizer.Services.Converters
{
	public class SegmentElementTypeConverter : JsonConverter
	{
		private static readonly Regex RegexWhitespace = new Regex(@"\s", RegexOptions.None);

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(SegmentElement);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var value = serializer.Deserialize(reader).ToString();

			if (IsTextValue(value))
			{
				return JsonConvert.DeserializeObject<Text>(value);
			}

			return JsonConvert.DeserializeObject<Tag>(value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value, typeof(SegmentElement));
		}

		private static bool IsTextValue(string value)
		{
			return RegexWhitespace.Replace(value, string.Empty).StartsWith("{\"Value\":");
		}
	}
}
