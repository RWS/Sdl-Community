using System;
using System.Runtime.Serialization;


namespace Sdl.Community.TQA.Model
{
	[DataContract(Namespace = "http://www.sdl.com/projectapi", Name = "Category")]
	public class Category
	{
		[DataMember(Name = "Id")]
		public Guid Id
		{
			get;
			set;
		}

		[DataMember(Name = "Name")]
		public string Name
		{
			get;
			set;
		}

		[DataMember(Name = "Abbreviation")]
		public string Abbreviation
		{
			get;
			set;
		}

		[DataMember(Name = "Description")]
		public string Description
		{
			get;
			set;
		}

		[DataMember(Name = "CommentHint")]
		public string CommentHint
		{
			get;
			set;
		}

		[DataMember(Name = "CommentRequired", EmitDefaultValue = false)]
		public bool CommentRequired
		{
			get;
			set;
		}

		[DataMember(Name = "SubCategories")]
		public SubCategories SubCategories
		{
			get;
			set;
		}

	}
}
