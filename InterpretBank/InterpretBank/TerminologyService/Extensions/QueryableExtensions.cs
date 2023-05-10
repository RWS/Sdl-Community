using System;
using System.Linq;
using System.Linq.Expressions;

namespace InterpretBank.TerminologyService.Extensions;

public static class QueryableExtensions
{
	public static IQueryable<T> WhereFuzzy<T>(this IQueryable<T> source, string propertyName, object value)
	{
		var parameter = Expression.Parameter(typeof(T));
		var property = Expression.Property(parameter, propertyName);
		var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
		var call = Expression.Call(property, method, Expression.Constant(value));
		var lambda = Expression.Lambda<Func<T, bool>>(call, parameter);

		return source.Where(lambda);
	}

	public static bool FuzzyMatch(string str1, string str2, int tolerance)
	{
		if (Math.Abs(str1.Length - str2.Length) > tolerance)
			// If the difference in length is greater than the tolerance, the strings cannot match
			return false;

		var matrix = new int[str1.Length + 1, str2.Length + 1];

		// Initialize the first row and column of the matrix
		for (var i = 0; i <= str1.Length; i++) matrix[i, 0] = i;

		for (var j = 0; j <= str2.Length; j++) matrix[0, j] = j;

		// Fill in the rest of the matrix
		for (var i = 1; i <= str1.Length; i++)
		for (var j = 1; j <= str2.Length; j++)
		{
			var cost = str1[i - 1] == str2[j - 1] ? 0 : 1;
			matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), matrix[i - 1, j - 1] + cost);
		}

		// The Levenshtein distance is the value in the bottom right corner of the matrix
		var distance = matrix[str1.Length, str2.Length];

		return distance <= tolerance;
	}
}