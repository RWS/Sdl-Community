using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.Converters
{
	public class FieldValueTypeConverter : JsonConverter
	{
		private static readonly Regex RegexValueType = new Regex("\"ValueType\"\\s*:\\s*(?<digit>\\d+)", RegexOptions.None);

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(Model.FieldDefinitions.FieldValue);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var value = serializer.Deserialize(reader).ToString();

			switch (GetValueType(value))
			{
				case 1: return JsonConvert.DeserializeObject<Model.FieldDefinitions.SingleStringFieldValue>(value);
				case 2: return JsonConvert.DeserializeObject<Model.FieldDefinitions.MultipleStringFieldValue>(value);
				case 3: return JsonConvert.DeserializeObject<Model.FieldDefinitions.DateTimeFieldValue>(value);
				case 4: return JsonConvert.DeserializeObject<Model.FieldDefinitions.SinglePicklistFieldValue>(value);
				case 5: return JsonConvert.DeserializeObject<Model.FieldDefinitions.MultiplePicklistFieldValue>(value);
				case 6: return JsonConvert.DeserializeObject<Model.FieldDefinitions.IntFieldValue>(value);
				default: return null; //Unknown
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value, typeof(Model.FieldDefinitions.FieldValue));
		}

		private static int GetValueType(string value)
		{
			var match = RegexValueType.Match(value);
			if (match.Success)
			{
				try
				{
					return Convert.ToInt32(match.Groups["digit"].Value);
				}
				catch { }
			}

			return 0;
		}
	}
}
