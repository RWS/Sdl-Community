using System;

namespace Multilingual.Excel.FileType.Models
{
	public class Hyperlink: ICloneable
	{
		public string Id { get; set; }

		public string Url { get; set; }

		public string Tooltip { get; set; }

		public string Display { get; set; }

		public string Reference { get; set; }

		public string Location { get; set; }

		public bool IsExternal { get; set; }

		public bool IsEmail { get; set; }

		public string Email { get; set; }

		public string Subject { get; set; }

		public object Clone()
		{
			return new Hyperlink
			{
				Id = Id,
				Url = Url,
				Tooltip = Tooltip,
				Display = Display,
				Reference = Reference,
				Location = Location,
				IsExternal = IsExternal,
				IsEmail = IsEmail,
				Email = Email,
				Subject = Subject
			};
		}
	}
}
