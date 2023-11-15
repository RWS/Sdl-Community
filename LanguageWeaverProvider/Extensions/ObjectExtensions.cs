using System.Collections.Generic;

namespace LanguageWeaverProvider.Extensions
{
	public static class ObjectExtensions
	{
		public static List<KeyValuePair<string, string>> ToKeyValuePairList(this object obj)
		{
			var type = obj.GetType();
			var properties = type.GetProperties();
			var keyValuePairs = new List<KeyValuePair<string, string>>();
			foreach (var property in properties)
			{
				var key = property.Name;
				var value = property.GetValue(obj);
				if (value is null || value is not string stringValue)
				{
					continue;
				}

				keyValuePairs.Add(new(key, stringValue));
			}

			return keyValuePairs;
		}
	}
}
