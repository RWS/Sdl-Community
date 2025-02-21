using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace InterpretBank.GlossaryService.Interface
{
	public interface IDatabaseConnection : IDisposable
	{
		bool IsSet { get; }

		void CreateDatabaseFile(string filePath);

		List<Dictionary<string, string>> ExecuteSelectCommand(SQLiteCommand sql);

		void LoadDatabase(string filePath);
        void ExecuteNonSelectCommand(SQLiteCommand cmdQuery);
    }
}