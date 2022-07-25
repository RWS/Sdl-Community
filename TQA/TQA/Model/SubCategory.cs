using System;
using System.Runtime.Serialization;
namespace Sdl.Community.TQA.Model
{
	[DataContract(Namespace = "http://www.sdl.com/projectapi", Name = "SubCategory")]
	public class SubCategory : Category
	{
		[DataMember(Name = "ParentId")]
		public Guid ParentId
		{
			get;
			set;
		}
	}
}
