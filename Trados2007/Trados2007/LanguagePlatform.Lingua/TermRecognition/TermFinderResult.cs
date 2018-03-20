using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.Lingua.TermRecognition
{
	/// <summary>
	/// Represents information returned by a term recognition process.
	/// </summary>
	[DataContract]
	public class TermFinderResult
	{
		private List<SegmentRange> _MatchingRanges;
		private int _Score;

		/// <summary>
		/// Gets or sets the list of matching segment ranges.
		/// </summary>
		[DataMember]
		public List<SegmentRange> MatchingRanges
		{
			get { return _MatchingRanges; }
			set { _MatchingRanges = value; }
		}

		/// <summary>
		/// Gets or sets the score associated to this result.
		/// </summary>
		[DataMember]
		public int Score
		{
			get { return _Score; }
			set { _Score = value; }
		}
	}

}
