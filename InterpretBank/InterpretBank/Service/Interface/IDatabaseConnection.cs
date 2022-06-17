﻿using System.Collections.Generic;
using System.Data.SQLite;

namespace InterpretBank.Service.Interface
{
	public interface IDatabaseConnection
	{
		bool IsSet { get; }

		void CreateDatabaseFile(string filePath);

		List<Dictionary<string, string>> ExecuteCommand(SQLiteCommand sql);

		void LoadDatabase(string filePath);
	}
}