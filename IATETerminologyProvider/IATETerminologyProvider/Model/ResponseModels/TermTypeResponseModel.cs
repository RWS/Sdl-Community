using System.Collections.Generic;

namespace IATETerminologyProvider.Model.ResponseModels
{
	public class TermTypeResponseModel
    {
		public MetaResponseModel Meta { get; set; }
		public List<ItemsResponseModel> Items { get; set; }
		public int Offset { get; set; }
		public int Limit { get; set; }
		public int Size { get; set; }
	}
}