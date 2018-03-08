using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Sdl.LanguagePlatform.Lingua
{
	/// <summary>
	/// Represents a substring in an ordered sequence, using the zero-based start position and the 
	/// substring length.
	/// </summary>
	[DataContract]
	public struct Substring
	{
		/// <summary>
		/// Initializes a new instance with the specified values. No checks are performed whether 
		/// the values are consistent.
		/// </summary>
		/// <param name="start">The start position of the substring.</param>
		/// <param name="length">The length of the substring.</param>
		public Substring(int start, int length)
		{
			Start = start;
			Length = length;
		}

		/// <summary>
		/// The start position of the substring.
		/// </summary>
		[DataMember]
		public int Start;

		/// <summary>
		/// The end position of the substring.
		/// </summary>
		[DataMember]
		public int Length;
	}
}
