using System.Collections.Generic;
using System.Linq;
using InterpretBank.Service.Interface;

namespace InterpretBank.Service
{
	public class ConditionBuilder : IConditionBuilder
	{
		private List<Expression> Expressions { get; } = new();

		public string Build()
		{
			return Expressions.Count == 0
				? null
				: Expressions.Skip(1).Aggregate(Expressions[0].Text,
					(current, expression) => current + $@" {expression.Operator} {expression.Text}");
		}

		public IConditionBuilder In(string columnName, List<string> glossaryNames, string @operator = "AND")
		{
			var inString = $@"({columnName} IN {string.Join(",", glossaryNames)})";
			Expressions.Add(new Expression { Operator = @operator, Text = inString });
			return this;
		}

		public IConditionBuilder In(string columnName, string sqlSelect, string @operator = "AND")
		{
			var inString = $@"({columnName} IN {sqlSelect})";
			Expressions.Add(new Expression { Operator = @operator, Text = inString });
			return this;
		}

		public IConditionBuilder Like(string likeString, List<int> languageIndices, string @operator = "AND")
		{
			if (string.IsNullOrWhiteSpace(likeString)) return null;

			var likeExpressions = new List<string>();
			foreach (var index in languageIndices)
			{
				likeExpressions.Add($@"Term{index} like ""%{likeString}%""");
				likeExpressions.Add($@"Comment{index}a like ""%{likeString}%""");
				likeExpressions.Add($@"Comment{index}b like ""%{likeString}%""");
			}

			likeString = string.Join(" OR ", likeExpressions);

			Expressions.Add(new Expression { Operator = @operator, Text = $@"({likeString})" });
			return this;
		}
	}
}