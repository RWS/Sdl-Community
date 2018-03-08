using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	public class FSATransition : IComparable<FSATransition>
	{
		private int _Source;
		private int _Target;
		private Label _Input;

		public FSATransition(int source, int target, Label input)
		{
			_Source = source;
			_Target = target;
			_Input = input;
		}

		public int Source
		{
			get { return _Source; }
			set { _Source = value; }
		}

		public int Target
		{
			get { return _Target; }
			set { _Target = value; }
		}

		public Label Input
		{
			get { return _Input; }
			set { _Input = value; }
		}

		public bool IsEpsilon
		{
			get { return _Input.IsEpsilon; }
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

			FSATransition other = obj as FSATransition;
			return CompareTo(other) == 0;
		}

		/// <summary>
		/// <see cref="M:System.Object.GetHashCode(object)"/>
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// Defines an order on transitions. They are sorted by the source state ASC, 
		/// then by input symbol ASC, then by target state ASC.
		/// </summary>
		public int CompareTo(FSATransition other)
		{
			int result = _Source - other._Source;
			if (result == 0)
				result = _Input.Symbol - other._Input.Symbol;
			if (result == 0)
				result = _Target - other._Target;
			return result;
		}
	}
}
