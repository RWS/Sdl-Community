using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service.Model
{
	public class SqlBuilder : ISqlBuilder
	{
		private List<string> ColumnNames { get; set; }
		private List<string> InsertValues { get; set; }
		private (string, string, string) JoinParameters { get; set; }
		private string TableName { get; set; }
		private string WhereCondition { get; set; }
		private StatementType Type { get; set; }

		private enum StatementType
		{
			Select,
			Insert,
			Update
		}

		public string Build()
		{
			var sqlStatement = "";

			var columnNamesString =
				ColumnNames is not null && ColumnNames.Count > 0 ? string.Join(", ", ColumnNames) : "*";

			switch (Type)
			{
				case StatementType.Insert:
				{
					var insertValuesString = $@"""{InsertValues[0]}""";
					foreach (var value in InsertValues.Skip(1))
					{
						insertValuesString += $@", ""{value}""";
					}

					var insert = $"INSERT INTO {TableName} ({columnNamesString})";
					var values = $"VALUES ({insertValuesString})";
					sqlStatement = $"{insert} {values}";
					break;
				}
				case StatementType.Select:
				{
					var select = $"SELECT {columnNamesString}";

					var join = JoinParameters.Item1 is not null ? $" INNER JOIN {JoinParameters.Item1} ON {JoinParameters.Item2}={JoinParameters.Item3}" : null;
					var from = $" FROM {TableName}{@join}";
					var where = WhereCondition is not null ? $" WHERE {WhereCondition}" : null;

					sqlStatement = $"{@select}{@from}{@where}";
					break;
				}
				case StatementType.Update:
					if (ColumnNames is null || UpdateValues is null || ColumnNames.Count != UpdateValues.Count)
						return null;

					var pairs = new List<string>();

					for (var index = 0; index < ColumnNames.Count; index++)
					{
						if (!string.IsNullOrWhiteSpace(UpdateValues[index]) &&
						    !string.IsNullOrWhiteSpace(ColumnNames[index]))
							pairs.Add($@"{ColumnNames[index]} = ""{UpdateValues[index]}""");
					}

					var set = $" SET {string.Join(", ", pairs)}";

					var updateCondition = $" WHERE {WhereCondition}"; //will crash if no condition is given as this is too dangerous to be done without it
					sqlStatement = $"UPDATE {TableName}{set}{updateCondition}";
					break;
			}


			ResetFields();
			return sqlStatement;
		}

		public ISqlBuilder Columns(List<string> columnNames)
		{
			ColumnNames = columnNames;
			return this;
		}

		public ISqlBuilder InnerJoin<T>(T tableName, string firstId, string secondId)
		{
			var tableNameString = tableName.ToString();
			if (string.IsNullOrWhiteSpace(tableNameString) || string.IsNullOrWhiteSpace(firstId) ||
				string.IsNullOrWhiteSpace(secondId)) return this;

			JoinParameters = (tableNameString, firstId, secondId);
			return this;
		}

		public ISqlBuilder Insert(List<string> values)
		{
			Type = StatementType.Insert;
			InsertValues = values;
			return this;
		}

		public ISqlBuilder Table<T>(T tableName)
		{
			TableName = tableName.ToString();
			return this;
		}

		public ISqlBuilder Update(List<string> values)
		{
			Type = StatementType.Update;
			UpdateValues = values;
			return this;
		}

		private List<string> UpdateValues { get; set; }

		public ISqlBuilder Where(string condition)
		{
			WhereCondition = condition;
			return this;
		}

		private void ResetFields()
		{
			ColumnNames = null;
			TableName = null;
			WhereCondition = null;
			InsertValues = null;
			UpdateValues = null;
			Type = StatementType.Select;
			JoinParameters = (null, null, null);
		}
	}
}