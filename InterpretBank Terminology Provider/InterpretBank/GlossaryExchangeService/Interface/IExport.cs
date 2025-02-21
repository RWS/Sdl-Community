using System.Collections.Generic;

namespace InterpretBank.GlossaryExchangeService.Interface
{
	public interface IExport
	{
		void ExportTerms(IEnumerable<string[]> terms, string glossaryName = null, string subGlossaryName = null);
	}
}