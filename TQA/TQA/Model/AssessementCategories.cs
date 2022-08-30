using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sdl.Community.TQA.Model
{
	[CollectionDataContract(Namespace = "http://www.sdl.com/projectapi", Name = "AssessmentCategories", ItemName = "Category")]
	public class AssessmentCategories : List<Category>
	{

	}
}
