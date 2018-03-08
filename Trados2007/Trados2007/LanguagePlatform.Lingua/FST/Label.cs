using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.FST
{
	public class Label
	{
		public const int SpecialSymbolEpsilon = -1;
		public const int SpecialSymbolBeginningOfWord = -3;
		public const int SpecialSymbolEndOfWord = -4;
		public const int SpecialSymbolBeginningOfLine = -5;
		public const int SpecialSymbolEndOfLine = -6;
		public const int SpecialSymbolWhitespace = -7;
		public const int SpecialSymbolDigit = -8;

		public const int FirstUserDefinedSymbol = -1000;

		private int _Symbol;

		public Label(int symbol)
		{
			_Symbol = symbol;
		}

		public bool IsCharLabel
		{
			get { return _Symbol >= 0; }
		}

		public Label(Label other)
		{
			_Symbol = other._Symbol;
		}

		public int Symbol
		{
			get { return _Symbol; }
			set { _Symbol = value; }
		}

		public bool IsConsuming
		{
			get
			{
				return _Symbol >= 0
				  || _Symbol == SpecialSymbolWhitespace
				  || _Symbol == SpecialSymbolDigit
				  || _Symbol <= FirstUserDefinedSymbol;
			}
		}

		public bool IsEpsilon
		{
			get { return _Symbol == SpecialSymbolEpsilon; }
		}

		public bool Matches(string s, int position, bool ignoreCase)
		{
			char c;
			if (_Symbol >= 0 || _Symbol <= FirstUserDefinedSymbol)
			{
				if (position >= s.Length)
					return false;

				c = s[position];
				bool eq = (c == _Symbol);
				if (!eq && ignoreCase)
				{
					eq = Char.ToLowerInvariant(c) == Char.ToLowerInvariant((char)_Symbol);
				}

				return eq;
			}

			if (_Symbol == SpecialSymbolEpsilon)
				return true;

			if (position == 0 && (_Symbol == SpecialSymbolBeginningOfLine
				|| _Symbol == SpecialSymbolBeginningOfWord))
				// TODO BoW is arguable as there may be leading whitespace
				return true;

			if (position >= s.Length)
				// reached the end of the string. Non-consuming marks match, others don't.
				// TODO EoW is, again, arguable as it demands "in-word" preceding context
				return (_Symbol == SpecialSymbolEndOfLine || _Symbol == SpecialSymbolEndOfWord);

			// all cases apart from BoW and EoW are handled above.

			c = s[position];

			if (_Symbol == SpecialSymbolWhitespace)
				return Char.IsWhiteSpace(c);

			if (_Symbol == SpecialSymbolDigit)
				return Char.IsDigit(c);

			char wordChar = '\0';
			char contextChar = '\0';
			if (_Symbol == SpecialSymbolBeginningOfWord)
			{
				contextChar = position > 0 ? s[position - 1] : '\0';
				wordChar = c;
				return Char.IsLetterOrDigit(wordChar) && !Char.IsLetterOrDigit(contextChar);
			}
			else
			{
				System.Diagnostics.Debug.Assert(_Symbol == SpecialSymbolEndOfWord);
				wordChar = position > 0 ? s[position - 1] : '\0';
				contextChar = c;
				// only test for non-word character of the context char, i.e. don't care
				//  whether the preceding char is a word char or not
				return !Char.IsLetterOrDigit(contextChar);
			}
		}

		/// <summary>
		/// Tests whether the character c is matched by a list of symbols (which is e.g. a FIRST() set).
		/// </summary>
		/// <param name="c">The character to test</param>
		/// <param name="symbols">The list of symbols. If the list is not sorted, it will be sorted.</param>
		/// <returns>true iff the character is contained in the symbol list</returns>
		public static bool Matches(char c, List<int> symbols, bool ignoreCase)
		{
			if (symbols == null || symbols.Count == 0)
				return false;

			// test whether the symbol list is sorted
			int last = symbols[0];
			bool sorted = true;
			for (int i = 1; i < symbols.Count && sorted; ++i)
			{
				if (symbols[i] < last)
					sorted = false;
				last = symbols[i];
			}

			if (!sorted)
				symbols.Sort();

			bool contains = symbols.BinarySearch((int)c) >= 0;
			if (!contains && ignoreCase)
			{
				// if we ignore case, check whether the case variant is in the set

				if (Char.IsUpper(c))
				{
					contains = symbols.BinarySearch(Char.ToLowerInvariant(c)) >= 0;
				}
				else if (Char.IsLower(c))
				{
					contains = symbols.BinarySearch(Char.ToUpperInvariant(c)) >= 0;
				}
			}

			if (!contains && symbols.BinarySearch(SpecialSymbolWhitespace) >= 0)
				contains = Char.IsWhiteSpace(c);

			if (!contains && symbols.BinarySearch(SpecialSymbolDigit) >= 0)
				contains = Char.IsDigit(c);

			return contains;
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

			Label other = obj as Label;
			return _Symbol == other._Symbol;
		}

		/// <summary>
		/// <see cref="M:System.Object.GetHashCode(object)"/>
		/// </summary>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		/// <summary>
		/// <see cref="object.ToString()"/>
		/// </summary>
		/// <returns>A string representation of the object, for display purposes.</returns>
		public override string ToString()
		{
			if (_Symbol >= 0)
				return char.ToString((char)_Symbol);

			// TODO user-defined symbol

			switch (_Symbol)
			{
			case SpecialSymbolBeginningOfLine:
				return "^";
			case SpecialSymbolBeginningOfWord:
				return "#<";
			case SpecialSymbolEndOfLine:
				return "$";
			case SpecialSymbolEndOfWord:
				return "#>";
			case SpecialSymbolEpsilon:
				return String.Empty;
			case SpecialSymbolWhitespace:
				return "\\s";
			case SpecialSymbolDigit:
				return "\\d";
			default:
				throw new Exception("Invalid symbol");
			}
		}
	}

}
