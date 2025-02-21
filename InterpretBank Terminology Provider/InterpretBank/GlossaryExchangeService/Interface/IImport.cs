using System.Collections.Generic;

namespace InterpretBank.GlossaryExchangeService.Interface
{
	public interface IImport
	{
		IEnumerable<string[]> ImportTerms();
	}
}