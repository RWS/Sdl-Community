using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service.Model
{
	public class SqlBuilder : ISqlBuilder, IConditionBuilder
	{
		private string WhereCondition { get; set; }
		private List<string> ColumnNames { get; set; }
		private List<string> InsertValues { get; set; } = new();
		private (string, string, string) JoinParameters { get; set; }
		private string TableName { get; set; }
		private StatementType Type { get; set; }

		private enum StatementType
		{
			Select,
			Insert,
			Update,
			Delete
		}

		public SQLiteCommand Build()
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
				case StatementType.Delete:

					sqlStatement = $"DELETE FROM {TableName} WHERE {WhereCondition}";
					break;
			}

			var sqlCommand = new SQLiteCommand(sqlStatement);

			foreach (var variable in Variables)
			{
				var dbType = variable.Value switch
				{
					int _ => DbType.Int64,
					_ => DbType.String
				};

				sqlCommand.Parameters.Add(variable.Key, dbType);
				sqlCommand.Parameters[variable.Key].Value = variable.Value;
			}

			ResetFields();
			return sqlCommand;
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

		public ISqlBuilder Insert(List<object> values)
		{
			Type = StatementType.Insert;
			values.ForEach(v => InsertValues.Add(StoreAndGetReference(v)));
			return this;
		}

		public ISqlBuilder Table<T>(T tableName)
		{
			TableName = tableName.ToString();
			return this;
		}

		public ISqlBuilder Delete()
		{
			Type = StatementType.Delete;
			return this;
		}

		public ISqlBuilder Update(List<object> values)
		{
			Type = StatementType.Update;
			values.ForEach(v => UpdateValues.Add(StoreAndGetReference(v)));
			return this;
		}

		private List<string> UpdateValues { get; set; } = new();

		public ISqlBuilder Where(string condition)
		{
			WhereCondition = condition;
			return this;
		}

		public IConditionBuilder Where() => this;

		private void ResetFields()
		{
			ColumnNames = new();
			TableName = null;
			InsertValues = new();
			UpdateValues = new();
			Type = StatementType.Select;
			JoinParameters = (null, null, null);
			Expressions = new();
			WhereCondition = null;
			Variables = new();
		}

		private List<Expression> Expressions { get; set; } = new();

		public ISqlBuilder EndCondition()
		{
			WhereCondition = Expressions.Count == 0
				? null
				: Expressions.Skip(1).Aggregate(Expressions[0].Text,
					(current, expression) => current + $@" {expression.Operator} {expression.Text}");

			return this;
		}

		private Dictionary<string, object> Variables { get; set; } = new();

		private string StoreAndGetReference(object value)
		{
			var key = $"@{Variables.Count}";
			Variables.Add(key, value);

			return key;
		}

		public IConditionBuilder Equals(object value, string columnName, string @operator = "AND")
		{
			if (value is null) return this;

			var reference = StoreAndGetReference(value);

			Expressions.Add(new Expression { Operator = @operator, Text = $@"({columnName} = {reference})" });
			return this;
		}

		public IConditionBuilder In(string columnName, List<object> values, string @operator = "AND")
		{
			if (values is null || values.Count == 0) return this;

			var valueReferences = new List<string>();
			values.ForEach(v => valueReferences.Add(StoreAndGetReference(v)));

			var glossaryNamesSql = new List<string>(valueReferences.Count);
			glossaryNamesSql.AddRange(valueReferences.Select(glossaryName => $"{glossaryName}"));
			var inString = $@"{columnName} IN ({string.Join(",", glossaryNamesSql)})";

			Expressions.Add(new Expression { Operator = @operator, Text = inString });
			return this;
		}

		public IConditionBuilder In(string columnName, SQLiteCommand sqlSelect, string @operator = "AND")
		{
			foreach (SQLiteParameter sqlSelectParameter in sqlSelect.Parameters)
			{
				var newName = StoreAndGetReference(sqlSelectParameter.Value);
				sqlSelect.CommandText = sqlSelect.CommandText.Replace(sqlSelectParameter.ParameterName, newName);
			}

			var inString = $@"{columnName} IN ({sqlSelect.CommandText})";

			Expressions.Add(new Expression { Operator = @operator, Text = inString});

			return this;
		}

		public IConditionBuilder Like(string likeString, List<string> columns, string @operator = "AND")
		{
			if (string.IsNullOrWhiteSpace(likeString)) return this;

			var likeExpressions = columns.Select(column => $@"{column} like ""%{likeString}%""").ToList();

			likeString = string.Join(" OR ", likeExpressions);

			Expressions.Add(new Expression { Operator = @operator, Text = $@"({likeString})" });
			return this;
		}
	}
}