using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.LanguagePlatform.Lingua.Alignment;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	internal class SubstringAlignmentDisambiguator : IExtensionDisambiguator
	{
		public SubstringAlignmentDisambiguator()
		{
		}

		public AlignedSubstring PickExtension(List<AlignedSubstring> path, 
			List<AlignedSubstring> candidates)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (candidates == null)
				throw new ArgumentNullException("candidates");

			if (candidates.Count == 0)
				return null;
			if (candidates.Count == 1)
				return candidates[0];

			// if we don't yet have a path, pick any candidate (can't attach)
			if (path.Count == 0)
				return candidates[0];

			AlignedSubstring result = null;
			int minCost = 0;

			foreach (AlignedSubstring cand in candidates)
			{
				int cost = ComputeCosts(path, cand);
				if (result == null || cost < minCost)
				{
					minCost = cost;
					result = cand;
				}
			}

			return result;
		}

		private int ComputeCosts(List<AlignedSubstring> path, AlignedSubstring candidate)
		{
			if (candidate == null)
				throw new ArgumentNullException("candidate");

			// currently we simply compute the distance of the candidate to the nearest path element. 
			//  We may later compute "contributions" to existing token ranges (i.e. how well a token is covered)

			int minDist = -1;

			foreach (AlignedSubstring alg in path)
			{
				int srcDist = 0;
				int trgDist = 0;

				if (alg.Source.Start < candidate.Source.Start)
					srcDist = candidate.Source.Start - alg.Source.Start - alg.Source.Length;
				else
					srcDist = alg.Source.Start - candidate.Source.Start - candidate.Source.Length;

				if (alg.Target.Start < candidate.Target.Start)
					trgDist = candidate.Target.Start - alg.Target.Start - alg.Target.Length;
				else
					trgDist = alg.Target.Start - candidate.Target.Start - candidate.Target.Length;

				System.Diagnostics.Debug.Assert(srcDist >= 0);
				System.Diagnostics.Debug.Assert(trgDist >= 0);

				int dist = Math.Max(srcDist, trgDist);
				if (minDist < 0 || dist < minDist)
					minDist = dist;
			}

			System.Diagnostics.Debug.Assert(minDist >= 0);
			return minDist;
		}
	}
}
