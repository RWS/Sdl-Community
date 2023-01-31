using System;
using System.Linq;
using System.Linq.Expressions;

namespace InterpretBank.TermSearch.Extensions
{
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

		//...

		//var terms = DataContext.GetTable<GlossaryData>();
		//var result = terms.WherePropertyContains("Term1", word);

	}
}