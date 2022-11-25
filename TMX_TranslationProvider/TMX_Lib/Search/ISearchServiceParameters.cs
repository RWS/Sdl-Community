using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Search
{
	public interface ISearchServiceParameters
	{
		string FileName { get; set; }
		string DbConnectionNoPassword { get; set; }
		string Password { get; set; }
		string DbName { get; set; }
	}
}
