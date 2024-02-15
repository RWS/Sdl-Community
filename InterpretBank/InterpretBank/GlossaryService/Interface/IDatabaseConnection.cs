using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace InterpretBank.GlossaryService.Interface
{
	public interface IDatabaseConnection : IDisposable
	{
		bool IsSet { get; }

		void CreateDatabaseFile(string filePath);

		List<Dictionary<string, string>> ExecuteCommand(SQLiteCommand sql);

		void LoadDatabase(string filePath);
	}
}