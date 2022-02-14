using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service.Model
{
	public class SqlBuilder : ISqlBuilder, IConditionBuilder
	{
		private string WhereCondition;
		private List<string> ColumnNames { get; set; }
		private List<string> InsertValues { get; set; }
		private (string, string, string) JoinParameters { get; set; }
		private string TableName { get; set; }
		//private string WhereCondition { get; set; }
		private StatementType Type { get; set; }

		private enum StatementType
		{
			Select,
			Insert,
			Update,
			Delete
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
				case StatementType.Delete:

					sqlStatement = $"DELETE FROM {TableName} WHERE {WhereCondition}";
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

		public ISqlBuilder Delete()
		{
			Type = StatementType.Delete;
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

		public IConditionBuilder Where() => this;

		private void ResetFields()
		{
			ColumnNames = null;
			TableName = null;
			InsertValues = null;
			UpdateValues = null;
			Type = StatementType.Select;
			JoinParameters = (null, null, null);
			Expressions = new List<Expression>();
			WhereCondition = null;
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

		public IConditionBuilder Equals(List<string> values, string columnName, string @operator = "AND")
		{
			if (values is null || values.Count == 0) return this;

			var equalsExpressions = values
				.Select(value => $@"{columnName} = ""{value}""").ToList();

			var equalsExpression = string.Join(" OR ", equalsExpressions);
			Expressions.Add(new Expression { Operator = @operator, Text = $"({equalsExpression})" });
			return this;
		}

		public IConditionBuilder Equals(string value, string columnName, string @operator = "AND")
		{
			if (string.IsNullOrWhiteSpace(value)) return this;

			Expressions.Add(new Expression { Operator = @operator, Text = $@"({columnName} = ""{value}"")" });
			return this;
		}

		public IConditionBuilder In(string columnName, List<string> glossaryNames, string @operator = "AND")
		{
			if (glossaryNames is null || glossaryNames.Count == 0) return this;
			var glossaryNamesSql = new List<string>(glossaryNames.Count);
			glossaryNamesSql.AddRange(glossaryNames.Select(glossaryName => $"'{glossaryName}'"));
			var inString = $@"{columnName} IN ({string.Join(",", glossaryNamesSql)})";

			Expressions.Add(new Expression { Operator = @operator, Text = inString });
			return this;
		}

		public IConditionBuilder In(string columnName, string sqlSelect, string @operator = "AND")
		{
			var inString = $@"({columnName} IN ({sqlSelect}))";
			Expressions.Add(new Expression { Operator = @operator, Text = inString });
			return this;
		}

		public bool IsEmpty() => Expressions.Count == 0;

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