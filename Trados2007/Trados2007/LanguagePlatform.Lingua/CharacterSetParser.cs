using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	/// <summary>
	/// A parser to create a <see cref="CharacterSet"/> from its textual representation.
	/// Simplified Character Sets contain chars, ranges, and Unicode class references. They can be 
	/// optionally negated. Their string representation is as follows:
	/// <list type="table">
	/// <item>cs = '[' Neg? (char | range | class)* ']'</item>
	/// <item>char = EscapedChar | UnicodeChar | SimpleChar</item>
	/// <item>Neg = '^'</item>
	/// <item>range = char '-' char</item>
	/// <item>EscapedChar = '\' SimpleChar</item>
	/// <item>UnicodeChar = 'U+' HexDigit{4}</item>
	/// </list>
	/// </summary>
	/// <remarks>This class is internal and not supposed to be used directly in your code.</remarks>
	public class CharacterSetParser
	{
		private class ParsePosition
		{
			private int _Position = 0;

			public ParsePosition(int i)
			{
				_Position = i;
			}

			public int Position
			{
				get { return _Position; }
				set { _Position = value; }
			}

			public void Advance()
			{
				++_Position;
			}

			public void Advance(int offset)
			{
				_Position += offset;
			}
		}

		/// <summary>
		/// Attempts to parse a character set, given the input <paramref name="input"/> 
		/// and starting from the start position <paramref name="p"/>. The position will 
		/// be updated if the parse is successful and then point to the first character 
		/// after the closing bracket in the input.
		/// <param name="input">The input string for the parser</param>
		/// <param name="p">The current position in the input string</param>
		/// </summary>
		/// <returns>A character set, if the parse is successful. If no valid character
		/// set can be parsed, an exception will be thrown.</returns>
		public static Core.CharacterSet Parse(string input, ref int p)
		{
			ParsePosition position = new ParsePosition(p);
			CharacterSetParser parser = new CharacterSetParser(input, position);
			Core.CharacterSet result = parser.Parse();
			p = position.Position;
			return result;
		}

		private string _Input;
		private int _InputLength;
		private ParsePosition _Position;

		private CharacterSetParser(string input, ParsePosition position)
		{
			_Input = input;
			_InputLength = input.Length;
			_Position = position;
		}

		private Core.CharacterSet Parse()
		{
			Core.CharacterSet result = new Core.CharacterSet();

			Expect('[');
			if (LookingAt() == '^')
			{
				result.Negated = true;
				_Position.Advance();
			}

			const int startState = 0;
			const int charclassState = 1;
			const int finalState = 99;

			int state = startState;

			while (state != finalState)
			{
				char lookAt = LookingAt();

				switch (state)
				{
					case startState:
						switch (lookAt)
						{
							case '[':
								// character class
								_Position.Advance();
								if (LookingAt() == ':')
								{
									_Position.Advance();
									state = charclassState;
								}
								else if (LookingAt() != '\0')
								{
									result.Add('[');
									// don't advance
								}
								break;
							case '\0':
								// unexpected end
								throw new Core.LanguagePlatformException(Core.ErrorCode.TokenizerInvalidCharacterSet, _Input);
							case ']':
								// right bracket - move on to final state
								_Position.Advance();
								state = finalState;
								break;
							default:
								{
									// a plain character (escaped, simple, or Unicode hex)
									char lower = ScanChar();
									char upper = '\0';

									if (LookingAt() == '-')
									{
										_Position.Advance();
										if (LookingAt() == ']')
										{
											// dash at end of input - add literal dash to charset, skip ], and go to final state
											result.Add(lower);
											result.Add('-');
											_Position.Advance();
											state = finalState;
										}
										else
										{
											upper = ScanChar();
											result.Add(lower, upper);
										}
									}
									else
									{
										result.Add(lower);
										state = startState;
									}
								}
								break;
						}
						break;
					case charclassState:
						// just got '[' followed by ':'

						if (LookingAt() == '\0')
						{
							throw new Core.LanguagePlatformException(Core.ErrorCode.TokenizerInvalidCharacterSet, _Input);
						}
						else
						{
							StringBuilder className = new StringBuilder();
							while (char.IsLetter(LookingAt()))
							{
								className.Append(LookingAt());
								_Position.Advance();
							}

							Nullable<System.Globalization.UnicodeCategory> category;

							if (className.Length == 0
								|| (category = Core.CharacterProperties.GetUnicodeCategoryFromName(className.ToString().ToLower())) == null
								|| !category.HasValue)
							{
								throw new Core.LanguagePlatformException(Core.ErrorCode.TokenizerInvalidCharacterSet, _Input);
							}
							else
							{
								result.Add(category.Value);
							}

							Expect(':');
							Expect(']');

							state = startState;
						}
						break;
					default:
						// unexpected state
						throw new Core.LanguagePlatformException(Core.ErrorCode.TokenizerInvalidCharacterSet, _Input);
				}
			}

			return result;
		}

		private void Expect(char c)
		{
			if (_Position.Position >= _InputLength || _Input[_Position.Position] != c)
				throw new Core.LanguagePlatformException(Core.ErrorCode.TokenizerInvalidCharacterSet, _Input);
			_Position.Advance();
		}

		private char ScanChar()
		{
			char current = LookingAt();
			if (current == 'U' || current == 'u')
			{
				int d1;
				int d2;
				int d3;
				int d4;

				if (_Position.Position + 5 < _InputLength
					&& _Input[_Position.Position + 1] == '+'
					&& (d1 = GetHexValue(_Input[_Position.Position + 2])) >= 0
					&& (d2 = GetHexValue(_Input[_Position.Position + 3])) >= 0
					&& (d3 = GetHexValue(_Input[_Position.Position + 4])) >= 0
					&& (d4 = GetHexValue(_Input[_Position.Position + 5])) >= 0)
				{
					_Position.Advance(5);
					return (char)(d1 * 4096 + d2 * 256 + d3 * 16 + d4);
				}
				else
				{
					_Position.Advance();
					return current;
				}
			}
			else if (current == '\\' && _Position.Position + 1 < _InputLength)
			{
				_Position.Advance(2);
				return _Input[_Position.Position - 1];
			}
			else
			{
				_Position.Advance();
				return current;
			}
		}

		private int GetHexValue(char c)
		{
			if (c >= '0' && c <= '9')
				return c - '0';

			if (c >= 'a' && c <= 'f')
				return c - 'a' + 10;

			if (c >= 'A' && c <= 'F')
				return c - 'A' + 10;

			return -1;
		}

		private char LookingAt()
		{
			return LookingAt(0);
		}

		private char LookingAt(int offset)
		{
			return (_Position.Position + offset >= _InputLength) ? '\0' : _Input[_Position.Position + offset];
		}
	}
}
