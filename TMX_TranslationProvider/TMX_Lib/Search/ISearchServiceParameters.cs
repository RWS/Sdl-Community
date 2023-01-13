using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Search
{
	public interface ISearchServiceParameters
	{
		string FullFileName { get;  }
		string DbConnectionNoPassword { get;  }
		string DbName { get;  }
		bool QuickImport { get; }
	}
}
