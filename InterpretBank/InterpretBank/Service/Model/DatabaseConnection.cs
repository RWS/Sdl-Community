using System.Collections.Generic;
using System.Data.SQLite;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service.Model
{
	public class DatabaseConnection : IDatabaseConnection
	{
		private readonly SQLiteConnection _connection;

		public DatabaseConnection(string filepath)
		{
			_connection = new SQLiteConnection($"Data Source='{filepath}';Cache=Shared");
		}

		public List<Dictionary<string, string>> ExecuteCommand(string sql)
		{
			_connection.Open();

			var cmdQuery = new SQLiteCommand(sql, _connection);
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
	}
}