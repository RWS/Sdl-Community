using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sdl.Community.AdaptiveMT.Service.Clients
{
	public class JsonSerializerHelper
	{
		public JsonSerializerSettings SerializerSettings()
		{
			return new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver(),
				DefaultValueHandling = DefaultValueHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore
			};
		}
	}
}
