using System.Collections.Generic;

namespace InterpretBank.Model.Interface
{
	public interface IGlossaryEntry
	{
		string ID { get; set; }
		string this[string property] { get; set; }

		List<string> GetColumns(bool includeId = false);

		List<string> GetValues();
	}
}