using System;
using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.DeepLMTProvider.Model
{
	public enum Formality
	{
		Default = 0,
		Less = 1,
		More = 2,
	}

	public static class FormalityEnumHelper
	{
		public static IEnumerable<Formality> Values =>
            Enum.GetValues(typeof(Formality))
                .Cast<Formality>();
    }
}