using System;

namespace Sdl.LanguagePlatform.Lingua.Alignment
{
	/// <summary>
	/// Represents the basic types of supported alignments. Note that general n:m alignments 
	/// are currently not supported.
	/// </summary>
	public enum AlignmentOperation
	{
		/// <summary>
		/// Unknown (for initialization)
		/// </summary>
		None,
		/// <summary>
		/// Represents a 1:1 alignment
		/// </summary>
		Substitute,
		/// <summary>
		/// Represents a 0:1 alignment
		/// </summary>
		Insert,
		/// <summary>
		/// Represents a 1:0 alignment
		/// </summary>
		Delete,
		/// <summary>
		/// Represents a 2:1 alignment
		/// </summary>
		Contract,
		/// <summary>
		/// Represents a 1:2 alignment
		/// </summary>
		Expand,
		/// <summary>
		/// Represents a 2:2 alignment
		/// </summary>
		Merge, 
		/// <summary>
		/// Represents an invalid alignment (used for fixpoints/fixed ranges)
		/// </summary>
		Invalid, 
	}

}
