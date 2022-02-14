using System.Collections.Generic;
using System.Data.SQLite;

namespace InterpretBank.Service.Interface
{
	public interface IDatabaseConnection
	{
		List<Dictionary<string, string>> ExecuteCommand(SQLiteCommand sql);
	}
}