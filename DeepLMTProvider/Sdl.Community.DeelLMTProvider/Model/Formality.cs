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
        Prefer_More
	}

	public static class FormalityEnumHelper
	{
		public static IEnumerable<Formality> Values =>
            Enum.GetValues(typeof(Formality))
                .Cast<Formality>();
    }
}