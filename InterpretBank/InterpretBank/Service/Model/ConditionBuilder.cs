using System.Collections.Generic;
using System.Linq;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service.Model
{
	public class ConditionBuilder : IConditionBuilder
	{
		private List<Expression> Expressions { get; set; } = new();

		public string Build()
		{
			var aggregate = Expressions.Count == 0
				? null
				: Expressions.Skip(1).Aggregate(Expressions[0].Text,
					(current, expression) => current + $@" {expression.Operator} {expression.Text}");

			ResetFields();
			return aggregate;
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

		private void ResetFields()
		{
			Expressions = new List<Expression>();
		}
	}
}