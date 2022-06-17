using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Validator
{
	

	public static class NumberComparisonExtension
	{
		public static Comparer Compare(this NumberText first, NumberText second)
		{
			var comparer = new Comparer(first, second);
			comparer.Compare();

			return comparer;
		}
	}
}