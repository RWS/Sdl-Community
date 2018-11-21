using System.Collections.Generic;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class JsonDomainResponseModel
	{
		public MetaResponseModel Meta { get; set; }
		public List<ItemsResponseModel> Items { get; set; }
	}
}