using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sdl.Community.AdaptiveMT.Service.Clients
{
	public  interface IJsonSerializer
	{
		string Serialize(object item);
		T Deserialize<T>(string json);
		JsonSerializerSettings SerializerSettings();
	}
}
