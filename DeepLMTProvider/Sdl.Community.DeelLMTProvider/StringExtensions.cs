using System;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider
{
	public static class StringExtensions
	{
		/// <summary>
		///     Returns a string array that contains the substrings in this instance that are delimited by specified indexes.
		/// </summary>
		/// <param name="source">The original string.</param>
		/// <param name="index">An index that delimits the substrings in this string.</param>
		/// <returns>An array whose elements contain the substrings in this instance that are delimited by one or more indexes.</returns>
		/// <exception cref="ArgumentNullException"><paramref name="index" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">An <paramref name="index" /> is less than zero or greater than the length of this instance.</exception>
		public static string[] SplitAt(this string source, params int[] index)
		{
			index = index.Distinct().OrderBy(x => x).ToArray();
			var output = new string[index.Length + 1];
			var pos = 0;

			for (var i = 0; i < index.Length; pos = index[i++])
				output[i] = source.Substring(pos, index[i] - pos);

			output[index.Length] = source.Substring(pos);
			return output;
		}
	}
}
