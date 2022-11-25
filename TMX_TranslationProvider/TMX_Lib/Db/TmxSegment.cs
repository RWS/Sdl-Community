using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.Db
{
	public class TmxSegment
	{
		public Db.TmxTranslationUnit DbTU;
		public Db.TmxText DbSourceText, DbTargetText;
		// note: the db text is in lower case. These contain the correct-case texts
		public string SourceText, TargetText;
	}
}
