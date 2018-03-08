using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Sdl.LanguagePlatform.Core;

namespace Sdl.LanguagePlatform.Lingua
{
	/// <summary>
	/// Represents a pair of aligned substrings in some underlying sequence.
	/// </summary>
	[DataContract]
	public class AlignedSubstring
	{
		/// <summary>
		/// Instantiates a new object and initializes it with the specified values.
		/// </summary>
		/// <param name="src">The substring in the source segment</param>
		/// <param name="trg">The substring in the target segment</param>
		public AlignedSubstring(Substring src, Substring trg)
			: this(src, trg, 0, 0)
		{
		}

		/// <summary>
		/// Instantiates a new object and initializes it with the specified values.
		/// </summary>
		/// <param name="src">The substring in the source segment</param>
		/// <param name="trg">The substring in the target segment</param>
		/// <param name="score">The score to assign to the aligned pair</param>
		public AlignedSubstring(Substring src, Substring trg, int score)
			: this(src, trg, score, 0)
		{
		}

		/// <summary>
		/// Instantiates a new object and initializes it with the specified values.
		/// </summary>
		/// <param name="src">The substring in the source segment</param>
		/// <param name="trg">The substring in the target segment</param>
		/// <param name="score">The score to assign to the aligned pair</param>
		/// <param name="length">A length to assign to the aligned pair (which may differ from the substring lengths)</param>
		public AlignedSubstring(Substring src, Substring trg, int score, int length)
		{
			Source = src;
			Target = trg;
			Score = score;
			Length = length;
		}

		/// <summary>
		/// Instantiates a new object and initializes it with the specified values.
		/// </summary>
		/// <param name="sourcePos">The start position of the source substring</param>
		/// <param name="sourceLen">The length of the source substring</param>
		/// <param name="targetPos">The start position of the target substring</param>
		/// <param name="targetLen">The length of the target substring</param>
		public AlignedSubstring(int sourcePos, int sourceLen, int targetPos, int targetLen)
			: this(new Substring(sourcePos, sourceLen), new Substring(targetPos, targetLen), 0, 0)
		{
		}

		/// <summary>
		/// Instantiates a new object and initializes it with the specified values.
		/// </summary>
		/// <param name="sourcePos">The start position of the source substring</param>
		/// <param name="sourceLen">The length of the source substring</param>
		/// <param name="targetPos">The start position of the target substring</param>
		/// <param name="targetLen">The length of the target substring</param>
		/// <param name="score">A score to assign to the alignment</param>
		public AlignedSubstring(int sourcePos, int sourceLen, int targetPos, int targetLen, int score)
			: this(new Substring(sourcePos, sourceLen), new Substring(targetPos, targetLen), score, 0)
		{
		}

		/// <summary>
		/// Instantiates a new object and initializes it with the specified values.
		/// </summary>
		/// <param name="sourcePos">The start position of the source substring</param>
		/// <param name="sourceLen">The length of the source substring</param>
		/// <param name="targetPos">The start position of the target substring</param>
		/// <param name="targetLen">The length of the target substring</param>
		/// <param name="score">A score to assign to the alignment</param>
		/// <param name="length">A length to assign to the alignment, which may differ from the lengths
		/// of the substrings.</param>
		public AlignedSubstring(int sourcePos, int sourceLen, int targetPos, int targetLen, int score, int length)
			: this(new Substring(sourcePos, sourceLen), new Substring(targetPos, targetLen), score, length)
		{
		}

		/// <summary>
		/// Gets or sets the source substring.
		/// </summary>
		[DataMember]
		public Substring Source;
		/// <summary>
		/// Gets or sets the target substring.
		/// </summary>
		[DataMember]
		public Substring Target;

		/// <summary>
		/// The score of the alignment (defaults to 0).
		/// </summary>
		[DataMember]
		public int Score;

		/// <summary>
		/// The length of the aligned sequence (defaults to 0), which may differ from the substring lengths.
		/// </summary>
		[DataMember]
		public int Length;

		/// <summary>
		/// <see cref="object.ToString()"/>
		/// </summary>
		/// <returns>A string representation of the object, for display purposes.</returns>
		public override string ToString()
		{
			return String.Format("({0},{1}-{2},{3},{4})",
				Source.Start, Source.Start + Source.Length - 1,
				Target.Start, Target.Start + Target.Length - 1, 
				Score);
		}
	}
}
