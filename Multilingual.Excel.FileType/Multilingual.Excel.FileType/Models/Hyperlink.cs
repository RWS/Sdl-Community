using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multilingual.Excel.FileType.Models
{
	public class Hyperlink
	{
		public string Id { get; set; }

		public string Url { get; set; }

		public string ToolTip { get; set; }

		public string Display { get; set; }

		public string Reference { get; set; }

		public string IsExternal { get; set; }

		public bool IsEmail { get; set; }

		public string Email { get; set; }

		public string Subject { get; set; }
	}
}
