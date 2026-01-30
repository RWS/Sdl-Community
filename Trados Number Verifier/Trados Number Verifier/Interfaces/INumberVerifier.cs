using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Model;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.NumberVerifier.Interfaces
{
	public interface INumberVerifier
	{
		INumberVerifierSettings VerificationSettings { get; }

		ITextGenerator TextGenerator { get; }

		List<ErrorReporting> Verify(ISegmentPair segmentPair, List<ExcludedRange> sourceExcludedRanges = null, List<ExcludedRange> targetExcludedRanges = null);
	}
}
