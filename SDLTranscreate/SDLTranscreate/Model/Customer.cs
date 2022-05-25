using System;

namespace Trados.Transcreate.Model
{
	public class Customer : BaseModel, ICloneable
	{
		private string _name;

		public string Id { get; set; }

		public string Name
		{
			get => _name;
			set
			{
				if (_name == value)
				{
					return;
				}

				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		public string Email { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public object Clone()
		{
			var model = new Customer
			{
				Id = Id,
				Name = Name,
				Email = Email
			};
			return model;
		}
	}
}
