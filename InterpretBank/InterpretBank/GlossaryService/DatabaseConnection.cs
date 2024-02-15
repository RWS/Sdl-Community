using System;
using System.Collections.Generic;
using System.Data.SQLite;
using InterpretBank.GlossaryService.Interface;

namespace InterpretBank.GlossaryService
{
	public class DatabaseConnection : IDatabaseConnection
	{
		private readonly SQLiteConnection _connection;

		public DatabaseConnection()
		{
			_connection = new SQLiteConnection();
		}

		public DatabaseConnection(string filePath)
		{
			_connection = new SQLiteConnection($"Data Source='{filePath}';Cache=Shared");
		}

		public bool IsSet => !string.IsNullOrWhiteSpace(_connection.ConnectionString);

		public void CreateDatabaseFile(string filePath)
		{
			try
			{
				SQLiteConnection.CreateFile(filePath);
			}
			catch
			{
				throw new Exception("Database could not be created");
			}
		}

		public List<Dictionary<string, string>> ExecuteCommand(SQLiteCommand cmdQuery)
		{
			_connection.Open();

			cmdQuery.Connection = _connection;
			var rdrSelect = cmdQuery.ExecuteReader();

			var rows = new List<Dictionary<string, string>>(rdrSelect.FieldCount);
			while (rdrSelect.Read())
			{
				var row = new Dictionary<string, string>(rdrSelect.FieldCount);
				for (var i = 0; i < rdrSelect.FieldCount; i++)
				{
					row[rdrSelect.GetName(i)] = rdrSelect[i].ToString();
				}

				rows.Add(row);
			}

			_connection.Close();
			return rows;
		}

		public void LoadDatabase(string filePath)
		{
			_connection.ConnectionString = $"Data Source='{filePath}';Cache=Shared";
		}

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}