using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using InterpretBank.GlossaryService.DAL;

namespace InterpretBank.TerminologyService.Extensions;

public static class Helper
{
    public static int FuzzyMatch(string databaseTerm, string textWord, int minScore, string text = null)
    {
        if (databaseTerm == null || textWord == null)
            return 0;

        databaseTerm = databaseTerm.ToLower();
        textWord = textWord.ToLower();

        int score;
        var max = Math.Max(databaseTerm.Length, textWord.Length);

        if (databaseTerm.Contains(textWord) && text.ToLower().Contains(databaseTerm))
            return 100;


        var matrix = new int[databaseTerm.Length + 1, textWord.Length + 1];

        for (var i = 0; i <= databaseTerm.Length; i++)
            matrix[i, 0] = i;

        for (var j = 0; j <= textWord.Length; j++)
            matrix[0, j] = j;

        for (var i = 1; i <= databaseTerm.Length; i++)
            for (var j = 1; j <= textWord.Length; j++)
            {
                var cost = databaseTerm[i - 1] == textWord[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1), matrix[i - 1, j - 1] + cost);

                score = (int)(((double)max - cost) / max * 100);
                if (score < minScore) return score;
            }

        var distance = matrix[databaseTerm.Length, textWord.Length];

        score = (int)(((double)max - distance) / max * 100);
        return score;
    }

    public static IEnumerable<(DbGlossaryEntry, string, int)> WhereFuzzy(this IQueryable<DbGlossaryEntry> source,
        string propertyName, string word, int minScore)
    {
        var parameter = Expression.Parameter(typeof(DbGlossaryEntry), "term");
        var termProperty = Expression.Property(parameter, propertyName);
        var convertExpression = Expression.Convert(termProperty, typeof(string));
        var accessProperty = Expression.Lambda<Func<DbGlossaryEntry, string>>(convertExpression, parameter);

        var getTermValue = accessProperty.Compile();
        foreach (var dbTerm in source)
        {
            var termValue = getTermValue(dbTerm);

            var score = FuzzyMatch(termValue, word, minScore);

            if (score >= minScore) yield return (dbTerm, termValue, score);
        }
    }

    //public static IEnumerable<(DbGlossaryEntry, string, int)> WhereFuzzy(this List<DbGlossaryEntry> source, string propertyName, string word)
    //{
    //    var parameter = Expression.Parameter(typeof(DbGlossaryEntry), "term");
    //    var termProperty = Expression.Property(parameter, propertyName);
    //    var convertExpression = Expression.Convert(termProperty, typeof(string));
    //    var accessProperty = Expression.Lambda<Func<DbGlossaryEntry, string>>(convertExpression, parameter);

    //    var getTermValue = accessProperty.Compile();
    //    foreach (var dbTerm in source)
    //    {
    //        var termValue = getTermValue(dbTerm);

    //        var contains = termValue.ToLower().Contains(word.ToLower());
    //        //var score = FuzzyMatch(termValue, word);

    //        /*if (score > 50)*/
    //        if (contains) yield return (dbTerm, termValue, /*score*/100);
    //    }
    //}

    public static IEnumerable<(SimpleEntry, string, int)> WhereFuzzy(this List<SimpleEntry> source, string word, int minScore, string text)
    {
        foreach (var simpleTerm in source)
        {
            //var contains = simpleTerm.Source.ToLower().Contains(word.ToLower());

            var score = FuzzyMatch(simpleTerm.Source, word, minScore, text);
            if (score >= minScore) /*if (contains)*/ yield return (simpleTerm, simpleTerm.Source, /*score*/score);
        }
    }
}