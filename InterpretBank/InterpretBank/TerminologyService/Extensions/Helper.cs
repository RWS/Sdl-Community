using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InterpretBank.GlossaryService.DAL;

namespace InterpretBank.TerminologyService.Extensions;

public static class Helper
{
	public static int FuzzyMatch(string databaseTerm, string textWord)
	{
		if (databaseTerm == null || textWord == null)
			return 0;

		databaseTerm = databaseTerm.ToLower();
		textWord = textWord.ToLower();

		//if (Math.Abs(databaseTerm is null ? 0 : databaseTerm.Length - textWord.Length) > tolerance)
		//	// If the difference in length is greater than the tolerance, the strings cannot match
		//	return 0;

		var matrix = new int[databaseTerm.Length + 1, textWord.Length + 1];

		// Initialize the first row and column of the matrix
		for (var i = 0; i <= databaseTerm.Length; i++)
			matrix[i, 0] = i;

		for (var j = 0; j <= textWord.Length; j++)
			matrix[0, j] = j;

		// Fill in the rest of the matrix
		for (var i = 1; i <= databaseTerm.Length; i++)
			for (var j = 1; j <= textWord.Length; j++)
			{
				var cost = databaseTerm[i - 1] == textWord[j - 1] ? 0 : 1;
				matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), matrix[i - 1, j - 1] + cost);
			}

		// The Levenshtein distance is the value in the bottom right corner of the matrix
		var distance = matrix[databaseTerm.Length, textWord.Length];

		var max = Math.Max(databaseTerm.Length, textWord.Length);
		var score = (int)(((double)max - distance) / max * 100);
		return score;
	}

	public static IEnumerable<(DbGlossaryEntry, string, int)> WhereFuzzy(this IQueryable<DbGlossaryEntry> source, string propertyName, string word)
	{
		var parameter = Expression.Parameter(typeof(DbGlossaryEntry), "term");
		var termProperty = Expression.Property(parameter, propertyName);
		var convertExpression = Expression.Convert(termProperty, typeof(string));
		var accessProperty = Expression.Lambda<Func<DbGlossaryEntry, string>>(convertExpression, parameter);

		var getTermValue = accessProperty.Compile();
		foreach (var dbTerm in source)
		{
			var termValue = getTermValue(dbTerm);
			yield return (dbTerm, termValue, FuzzyMatch(termValue, word));
		}
	}
}