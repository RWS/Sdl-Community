using System.Collections.Generic;

namespace Sdl.Community.IATETerminologyProvider.Model.ResponseModels
{
	public class JsonDomainResponseModel
	{
		public List<ItemsResponseModel> Items { get; set; } = new List<ItemsResponseModel>();
	}
}