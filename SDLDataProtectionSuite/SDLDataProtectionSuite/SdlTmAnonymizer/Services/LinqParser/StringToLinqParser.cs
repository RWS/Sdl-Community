using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.LinqParser.Model;
using Expression = System.Linq.Expressions.Expression;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Services.LinqParser
{
	public static class StringToLinqParser
	{
		private static readonly string BetweenExpressionPattern = @$"(?<Property>{PropertyPattern})\s+(?<Negated>not\s+)?between\s+(?<FirstConstant>{ConstantPattern})\s+(,|and)\s+(?<SecondConstant>{ConstantPattern})";

		private const string ConstantPattern = @"\d*[^\s]*\d*";
		private const string OperatorsPattern = @"(?:[<>]=)|(?:[<>=])";
		private const string PropertyPattern = "[a-zA-Z]+_?[a-zA-Z]+";

		public static IEnumerable<T> Evaluate<T>(this IEnumerable<T> translationUnits, string expression)
		{
			try
			{
				if (string.IsNullOrWhiteSpace(expression)) throw new Exception("Expression empty");

				var firstExpression = GetFirstExpression(expression);

				if (firstExpression.IsAtomic)
					return RunAtomicExpression(firstExpression, translationUnits);

				switch (firstExpression.Operator.ToUpper().Trim())
				{
					case "AND":
						var andResult = translationUnits.ToList();
						return andResult.Evaluate(firstExpression.FirstTerm).Evaluate(firstExpression.SecondTerm);

					case "OR":
						if (string.IsNullOrWhiteSpace(firstExpression.FirstTerm))
							throw new Exception("First term is missing");
						var firstSet = translationUnits.Evaluate(firstExpression.FirstTerm).ToList();

						if (string.IsNullOrWhiteSpace(firstExpression.SecondTerm))
							throw new Exception("Second term is missing");
						var secondSet = translationUnits.Evaluate(firstExpression.SecondTerm).ToList();

						firstSet.AddRange(secondSet);
						return firstSet;

					default:
						throw new Exception("Logical operator not valid");
				}
			}
			catch (InnerExpressionException ex)
			{
				throw new Exception(ex.Message);
			}
			catch (Exception ex)
			{
				var message = ex.Message.Replace("Expression", "Subexpression").Replace(" ->", "  ->");
				throw new Exception(@$"Expression {{ {expression} }} could not be evaluated: {Environment.NewLine} ->{message}");
			}
		}

		private static int EndIndexOf(this string source, string value)
		{
			var index = source.LastIndexOf(value);
			if (index > -1)
				index += value.Length;

			return index;
		}

		private static int GetEndOfFirstTerm(string expression, int startAt, ref bool secondTermFound,
			ref int lastAndLocation, ref string @operator)
		{
			var i = startAt;
			for (; i < expression.Length; i++)
			{
				if (expression[i] == '(') i = GetEndOfParenthesis(expression, i) + 1;
				if (Regex.IsMatch(expression.Substring(i), "^and", RegexOptions.IgnoreCase))
				{
					lastAndLocation = i;
					@operator = "and";
					i = GetEndOfFirstTerm(expression, i + 3, ref secondTermFound, ref lastAndLocation, ref @operator);
					break;
				}
				if (Regex.IsMatch(expression.Substring(i), "^or", RegexOptions.IgnoreCase))
				{
					secondTermFound = true;
					@operator = "or";
					return i - 1;
				}
			}

			if (secondTermFound) return i;
			if (lastAndLocation != -1) return lastAndLocation;

			var operatorMatch = Regex.Match(expression, OperatorsPattern, RegexOptions.IgnoreCase);
			if (!operatorMatch.Success) throw new InnerExpressionException("Operator not found");

			@operator = operatorMatch.Value;
			return operatorMatch.Index;
		}

		private static int GetEndOfParenthesis(string expression, int startAt = 0)
		{
			var endOfParentheses = -1;

			var parenthesesOpenTotal = 0;
			for (var currentIndex = startAt; currentIndex < expression.Length; currentIndex++)
			{
				var ch = expression[currentIndex];
				if (ch == '(') parenthesesOpenTotal++;
				if (ch != ')') continue;

				parenthesesOpenTotal--;
				if (parenthesesOpenTotal != 0) continue;

				endOfParentheses = currentIndex;
				break;
			}

			if (parenthesesOpenTotal > 0) throw new Exception($"{parenthesesOpenTotal} opening parentheses not closed");

			return endOfParentheses;
		}

		private static string GetExpandedExpression(string expression)
		{
			var ternaryExpression = Regex.Match(expression, BetweenExpressionPattern, RegexOptions.IgnoreCase);
			if (!ternaryExpression.Success) return expression;

			var negated = ternaryExpression.Groups["Negated"].Value.ToUpper().Trim() == "NOT";
			var firstOperator = negated ? "<" : ">=";
			var secondOperator = negated ? ">" : "<=";
			var logicalOperator = negated ? "or" : "and";

			var property = ternaryExpression.Groups["Property"].Value;
			return RemoveRedundantParentheses(Regex.Replace(expression, BetweenExpressionPattern,
				$@"{property} {firstOperator} {ternaryExpression.Groups["FirstConstant"]} {logicalOperator} {property} {secondOperator} {ternaryExpression
					.Groups["SecondConstant"]}"));
		}

		private static Model.Expression GetFirstExpression(string expression)
		{
			expression = RemoveRedundantParentheses(expression.Trim());
			if (expression.Length == 0) return null;

			expression = GetExpandedExpression(expression);

			var secondTermFound = false;
			var lastAndLocation = -1;
			var @operator = "";
			var endOfFirstTerm = GetEndOfFirstTerm(expression, 0, ref secondTermFound, ref lastAndLocation, ref @operator);

			return new Model.Expression
			{
				FirstTerm = expression.Substring(0, endOfFirstTerm),
				Operator = @operator,
				SecondTerm = expression.Substring(expression.EndIndexOf(@operator))
			};
		}

		private static string RemoveRedundantParentheses(string expression)
		{
			if (expression.First() == '(' && expression.Last() == ')')
				expression = expression.Substring(1, expression.Length - 2);

			return expression;
		}

		private static IEnumerable<T> RunAtomicExpression<T>(Model.Expression expression, IEnumerable<T> objects)
		{
			var type = typeof(T);

			var propertyString = expression.FirstTerm.Trim();

			if (string.IsNullOrWhiteSpace(propertyString)) throw new Exception("The name of the property used for filtering is invalid");

			var filterByProperty = type.GetProperty(propertyString);
			if (filterByProperty is null) throw new Exception("Property used for filtering does not exist");

			var filterByPropertyType = filterByProperty.PropertyType;

			object comparisonConstant = null;
			try
			{
				comparisonConstant = filterByPropertyType.GetMethod("Parse", new[] { typeof(string) })?.Invoke(null,
					new[] { expression.SecondTerm.Trim() });
			}
			catch { }

			if (comparisonConstant is null) throw new Exception($"{{{expression.SecondTerm}}} couldn't be converted to a {filterByPropertyType}");
			var objectParameter = Expression.Parameter(type, "obj");


			BinaryExpression binaryExpression;
			if (filterByPropertyType == typeof(DateTime) || filterByPropertyType == typeof(int))
			{
				var isSpecificDay = filterByPropertyType == typeof(DateTime) && expression.SecondTerm.Trim().Split(' ').Length == 1;

				var systemField = Expression.Property(objectParameter, filterByProperty);

				var constantExpression = Expression.Constant(comparisonConstant);
				binaryExpression = expression.Operator
					switch
				{
					"<" => Expression.LessThan(systemField, constantExpression),
					">" => Expression.GreaterThan(systemField, constantExpression),
					"<=" => Expression.LessThanOrEqual(systemField, constantExpression),
					">=" => Expression.GreaterThanOrEqual(systemField, constantExpression),
					"=" =>  isSpecificDay ? GetInterval(systemField, (DateTime)comparisonConstant) : Expression.Equal(systemField, constantExpression),
					_ => throw new Exception("Comparison operator not allowed")
					};
			}
			else
			{
				throw new Exception("Type of comparison constant not allowed");
			}

			var comparison = Expression.Lambda<Func<T, bool>>(binaryExpression, objectParameter);

			var runAtomicExpression = objects.AsQueryable().Where(comparison);
			return runAtomicExpression;
		}

		private static BinaryExpression GetInterval(MemberExpression leftSide, DateTime comparisonConstant)
		{
			var greaterThanConstant = Expression.Constant(comparisonConstant);
			var lessThanConstant = Expression.Constant(comparisonConstant.AddDays(1));
			var greaterThanExpression = Expression.GreaterThanOrEqual(leftSide, greaterThanConstant);
			var lessThanExpression = Expression.LessThan(leftSide, lessThanConstant);

			return Expression.And(greaterThanExpression, lessThanExpression);
		}
	}
}