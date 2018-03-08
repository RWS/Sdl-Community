using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	public class FSTTransition : IComparable<FSTTransition>
	{
		private int _Source;
		private int _Target;
		private Label _Input;
		private Label _Output;

		public FSTTransition(int source, int target, Label input, Label output)
		{
			_Source = source;
			_Target = target;
			_Input = input;
			_Output = output;
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

		public Label Output
		{
			get { return _Output; }
			set { _Output = value; }
		}

		public bool IsEpsilon
		{
			get { return _Input.IsEpsilon && _Output.IsEpsilon; }
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

			FSTTransition other = obj as FSTTransition;
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
		/// then by input symbol ASC, then by output symbol ASC, then by target
		/// state ASC.
		/// </summary>
		public int CompareTo(FSTTransition other)
		{
			int result = _Source - other._Source;
			if (result == 0)
				result = _Input.Symbol - other._Input.Symbol;
			if (result == 0)
				result = _Output.Symbol - other._Output.Symbol;
			if (result == 0)
				result = _Target - other._Target;
			return result;
		}

		public bool CanTraverse(Matcher.MatchMode mode, 
			string input, int position, bool ignoreCase, 
			out Label output, out bool consumedInput)
		{
			consumedInput = false;
			output = null;

			Label matchLabel;
			Label outputLabel;

			switch (mode)
			{
			case Matcher.MatchMode.Analyse:
				matchLabel = _Input;
				outputLabel = _Output;
				break;
			case Matcher.MatchMode.Generate:
				matchLabel = _Output;
				outputLabel = _Input;
				break;
			default:
				throw new Exception("Illegal case constant");
			}

			if (matchLabel.Matches(input, position, ignoreCase))
			{
				consumedInput = matchLabel.IsConsuming;
				output = outputLabel;
				return true;
			}
			else
				return false;
		}
	}
}
