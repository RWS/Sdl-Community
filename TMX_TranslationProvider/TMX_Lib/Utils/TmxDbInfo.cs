using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TMX_Lib.Utils
{
	public class TmxDbInfo
	{
		public string DbName { get; set; } = "";
		public string FullFileName { get; set; } = "";

		// at this time, we allow only local connections , so this is pretty much always localhost
		// later, we may allow for cloud atlas connections
		public string DbConnectionNoPassword { get; set; } = "localhost:27017";

		[JsonIgnore]
		public string FileName => Path.GetFileName(FullFileName);
		[JsonIgnore]
		public string Folder => Path.GetDirectoryName(FullFileName);
	}
}
