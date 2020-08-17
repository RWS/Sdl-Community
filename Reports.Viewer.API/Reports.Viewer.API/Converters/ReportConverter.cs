using System;
using Newtonsoft.Json;
using Sdl.Reports.Viewer.API.Model;

namespace Sdl.Reports.Viewer.API.Converters
{
	public class ReportConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(Report));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return serializer.Deserialize(reader, typeof(Report));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, value, typeof(Report));
		}
	}
}
