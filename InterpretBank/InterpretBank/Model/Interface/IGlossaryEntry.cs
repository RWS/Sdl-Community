using System.Collections.Generic;

namespace InterpretBank.Model.Interface
{
	public interface IGlossaryEntry
	{
		string this[string property] { get; set; }

		List<string> GetColumns();
		List<string> GetValues();

		Dictionary<string, string> ToRow();
	}
}