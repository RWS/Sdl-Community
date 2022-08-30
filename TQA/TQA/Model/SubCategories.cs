using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.Community.TQA.Model
{

	[CollectionDataContract(Namespace = "http://www.sdl.com/projectapi", Name = "SubCategories", ItemName = "SubCategory")]
	public class SubCategories : List<SubCategory>
	{
		
	}
}
