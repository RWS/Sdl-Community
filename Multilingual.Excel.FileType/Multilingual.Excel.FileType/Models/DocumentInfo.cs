using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multilingual.Excel.FileType.Models
{
	public class DocumentInfo
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public bool IsLoadedOnce { get; set; }

		public string Language { get; set; }
	}
}
