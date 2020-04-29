using System.Linq;

namespace Sdl.Community.CleanUpTasks.Utilities
{
	public static class StringExtensions
	{
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
