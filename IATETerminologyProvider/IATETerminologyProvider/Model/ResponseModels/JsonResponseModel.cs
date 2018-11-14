using System.Collections.Generic;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class JsonResponseModel
	{
		public MetaResponseModel Meta { get; set; }
		public List<ItemsResponseModel> Items { get; set; }
		public int Offset { get; set; }
		public int Limit { get; set; }
		public int Size { get; set; }
		public NextResponseModel Next { get; set; }
		public RequestResponseModel Request { get; set; }
	}
}