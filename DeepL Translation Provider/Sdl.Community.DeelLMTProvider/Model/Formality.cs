using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public enum Formality
	{
		Default,
		Less,
		More,
        Prefer_Less,
        Prefer_More,
        Not_Supported
	}

	public static class FormalityEnumHelper
	{
		public static IEnumerable<Formality> Values
        {
            get
            {
                var values = Enum.GetValues(typeof(Formality));
                return values
                    .Cast<Formality>().Take(values.Length - 1);
            }
        }
    }
}