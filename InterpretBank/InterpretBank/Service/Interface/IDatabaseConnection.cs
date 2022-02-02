using System.Collections.Generic;

namespace InterpretBank.Service.Interface
{
	public interface IDatabaseConnection
	{
		List<Dictionary<string, string>> ExecuteCommand(string sql);
	}
}