using System;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class CustomerModel : BaseModel, ICloneable
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public object Clone()
		{
			var model = new CustomerModel
			{
				Id = Id,
				Name = Name,
				Email = Email
			};
			return model;
		}
	}
}
