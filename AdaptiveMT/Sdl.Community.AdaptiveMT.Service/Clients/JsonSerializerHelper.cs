using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sdl.Community.AdaptiveMT.Service.Clients
{
	public class JsonSerializerHelper:IJsonSerializer
	{
		public string Serialize(object item)
		{
			return JsonConvert.SerializeObject(item, Formatting.Indented,
				new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
		}

		public T Deserialize<T>(string json)
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public JsonSerializerSettings SerializerSettings()
		{
			return new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
		}
	}
}
