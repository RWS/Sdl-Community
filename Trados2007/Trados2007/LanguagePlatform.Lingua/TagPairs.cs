using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	public class PairedTag
	{
		public PairedTag(int s, int e, int anchor)
		{
			Start = s;
			End = e;
			Anchor = anchor;
		}

		public int Start;
		public int End;
		public int Anchor;

		public Core.EditDistance.EditOperation StartTagOperation
			= Core.EditDistance.EditOperation.Undefined;
		public Core.EditDistance.EditOperation EndTagOperation
			= Core.EditDistance.EditOperation.Undefined;

		/// <summary>
		/// <see cref="object.ToString()"/>
		/// </summary>
		/// <returns>A string representation of the object, for display purposes.</returns>
		public override string ToString()
		{
			return String.Format("({0},{1};i={2})", Start, End, Anchor);
		}

		/// <summary>
		/// <see cref="M:System.Object.Equals(object)"/>
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object;
		/// otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			if (obj.GetType() != this.GetType())
				return false;

			PairedTag other = obj as PairedTag;
			return other.Start == this.Start && other.End == this.End && other.Anchor == this.Anchor;
		}

		/// <summary>
		/// <see cref="M:System.Object.GetHashCode(object)"/>
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}

	internal class TagPairs : IEnumerable<PairedTag>
	{
		// maps from start tag position to _Pairings position
		private Dictionary<int, int> _StartIndex;

		// maps from end tag position to _Pairings position
		private Dictionary<int, int> _EndIndex;

		// maps from anchor to _Pairings position
		private Dictionary<int, int> _AnchorIndex;

		private List<PairedTag> _Pairings;

		public TagPairs()
		{
			_StartIndex = new Dictionary<int, int>();
			_EndIndex = new Dictionary<int, int>();
			_AnchorIndex = new Dictionary<int, int>();
			_Pairings = new List<PairedTag>();
		}

		public PairedTag this[int p]
		{
			get { return _Pairings[p]; }
		}

		public void Add(int startTagPosition, int endTagPosition, int anchor)
		{
			_Pairings.Add(new PairedTag(startTagPosition, endTagPosition, anchor));
			int p = _Pairings.Count;
			_StartIndex.Add(startTagPosition, p);
			_EndIndex.Add(endTagPosition, p);
			_AnchorIndex.Add(anchor, p);
		}

		public bool IsStartTag(int position)
		{
			return _StartIndex.ContainsKey(position);
		}

		public bool IsEndTag(int position)
		{
			return _EndIndex.ContainsKey(position);
		}

		public int GetStartPosition(int endPosition)
		{
			int result;
			if (!_EndIndex.TryGetValue(endPosition, out result))
				return -1;
			return _Pairings[result].Start;
		}

		public int GetEndPosition(int startPosition)
		{
			int result;
			if (!_StartIndex.TryGetValue(startPosition, out result))
				return -1;
			return _Pairings[result].End;
		}

		public PairedTag GetByAnchor(int anchor)
		{
			int p;
			if (!_AnchorIndex.TryGetValue(anchor, out p))
				return null;
			return _Pairings[p];
		}

		public PairedTag GetByStartPosition(int startPosition)
		{
			return _Pairings[_StartIndex[startPosition]];
		}

		public PairedTag GetByEndPosition(int endPosition)
		{
			return _Pairings[_EndIndex[endPosition]];
		}

		public int Count
		{
			get { return _Pairings.Count; }
		}

		public IEnumerator<PairedTag> GetEnumerator()
		{
			return _Pairings.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _Pairings.GetEnumerator();
		}

	}

}
