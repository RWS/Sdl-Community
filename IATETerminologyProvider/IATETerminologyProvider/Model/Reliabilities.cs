using System;
using System.Collections.Generic;

namespace Sdl.Community.IATETerminologyProvider.Model
{
	public static class Reliability
	{
		[Flags]
		public enum Reliabilities
		{
			Default = 0,
			Not = 1,
			DowngradePriorToDeletion = 2,
			NotVerified = 4,
			MinimumReliability = 8,
			Reliable = 16,
			VeryReliable = 32,
		}

		public static List<int> GetReliabilityCodes(Reliabilities reliabilities)
		{
			var codes = new List<int>();

			if (reliabilities.HasFlag(Reliabilities.DowngradePriorToDeletion))
				codes.Add(0);
			if (reliabilities.HasFlag(Reliabilities.NotVerified))
				codes.Add(1);
			if (reliabilities.HasFlag(Reliabilities.MinimumReliability))
				codes.Add(2);
			if (reliabilities.HasFlag(Reliabilities.Reliable))
				codes.Add(3);
			if (reliabilities.HasFlag(Reliabilities.VeryReliable))
				codes.Add(4);

			return codes;
		}
	}
}