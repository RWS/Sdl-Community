using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.FST
{
	internal class Parser
	{
		private enum Symbol
		{
			EOF,
			Char,
			LParen,
			RParen,
			LAngle,
			RAngle,
			OpDisj,
			OpKleene,
			OpPlus,
			OpOpt,
			Colon,
			AnchorBOL,
			AnchorEOL,
			AnchorBOW,
			AnchorEOW,
			SpecialDigit,
			SpecialWhitespace,
			None // used for some internal handling
		}

		private Symbol _CurrentSymbol;
		private char _CurrentChar;

		private string _Input;
		private int _InputPosition;
		private int _InputLength;

		public Parser()
		{
		}

		public FST Parse(string expression)
		{
			_Input = expression;
			_InputLength = expression.Length;
			_InputPosition = 0;

			Scan(false);
			if (_CurrentSymbol == Symbol.EOF)
				throw new Exception("Empty input expression");

			Node result = ParseRX();

			if (_CurrentSymbol != Symbol.EOF)
				throw new Exception("Trailing garbage");

			return result.GetFST();
		}

		private Node ParseRX()
		{
			Node result = ParseTerm();
			if (_CurrentSymbol == Symbol.OpDisj)
			{
				DisjunctionNode n = new DisjunctionNode();
				n.Add(result);
				result = n;

				while (_CurrentSymbol == Symbol.OpDisj)
				{
					Scan(false);
					Node rhs = ParseTerm();
					if (rhs == null)
						throw new Exception("Premature End of Input");
					n.Add(rhs);
				}
			}
			return result;
		}

		private Node ParseTerm()
		{
			Node result = ParseFactor();
			if (_CurrentSymbol == Symbol.LParen
				|| _CurrentSymbol == Symbol.Char
				|| _CurrentSymbol == Symbol.SpecialDigit
				|| _CurrentSymbol == Symbol.SpecialWhitespace
				|| _CurrentSymbol == Symbol.AnchorBOL
				|| _CurrentSymbol == Symbol.AnchorEOL
				|| _CurrentSymbol == Symbol.AnchorBOW
				|| _CurrentSymbol == Symbol.AnchorEOW
				|| _CurrentSymbol == Symbol.LAngle)
			{
				SequenceNode n = new SequenceNode();
				n.Add(result);
				result = n;

				while (_CurrentSymbol == Symbol.LParen
					|| _CurrentSymbol == Symbol.Char
					|| _CurrentSymbol == Symbol.SpecialDigit
					|| _CurrentSymbol == Symbol.SpecialWhitespace
					|| _CurrentSymbol == Symbol.AnchorBOL
					|| _CurrentSymbol == Symbol.AnchorEOL
					|| _CurrentSymbol == Symbol.AnchorBOW
					|| _CurrentSymbol == Symbol.AnchorEOW
					|| _CurrentSymbol == Symbol.LAngle)
				{
					Node rhs = ParseFactor();
					if (rhs == null)
						throw new Exception("Premature end of input");
					n.Add(rhs);
				}

			}
			return result;
		}

		private Node ParseFactor()
		{
			Node result = ParseSingle();

			while (_CurrentSymbol == Symbol.OpKleene
				|| _CurrentSymbol == Symbol.OpPlus
				|| _CurrentSymbol == Symbol.OpOpt)
			{
				if (_CurrentSymbol == Symbol.OpKleene)
				{
					Scan(false);
					RepetitiveNode rep = new RepetitiveNode(0, RepetitiveNode.INFINITY);
					rep.Content = result;
					result = rep;
				}
				else if (_CurrentSymbol == Symbol.OpPlus)
				{
					Scan(false);
					RepetitiveNode rep = new RepetitiveNode(1, RepetitiveNode.INFINITY);
					rep.Content = result;
					result = rep;
				}
				else if (_CurrentSymbol == Symbol.OpOpt)
				{
					Scan(false);
					RepetitiveNode rep = new RepetitiveNode(0, 1);
					rep.Content = result;
					result = rep;
				}
				else
					System.Diagnostics.Debug.Assert(false); // can't be as all symbols checked above
			}
			return result;
		}

		private void Expect(Symbol sym)
		{
			Expect(sym, false);
		}

		private void Expect(char c)
		{
			Expect(c, false);
		}

		private void Expect(Symbol sym, bool insideTransducerSymbol)
		{
			if (_CurrentSymbol != sym)
				throw new Exception("Expected symbol " + sym.ToString());
			Scan(insideTransducerSymbol);
		}

		private void Expect(char c, bool insideTransducerSymbol)
		{
			if (!(_CurrentSymbol == Symbol.Char && _CurrentChar == c))
				throw new Exception("Expected char " + c);
			Scan(insideTransducerSymbol);
		}

		private Node ParseSingle()
		{
			switch (_CurrentSymbol)
			{
			case Symbol.EOF:
				throw new Exception("Premature end of input");
			case Symbol.LParen:
				Scan(false);
				Node result = ParseRX();
				Expect(Symbol.RParen);
				return result;
			case Symbol.LAngle:
				{
					// a transducer symbol <a:b>
					Label input = null;
					Label output = null;

					Scan(true);

					// optional char (upper)
					if (_CurrentSymbol == Symbol.Colon)
						input = new Label(Label.SpecialSymbolEpsilon);
					else
						input = ParseSymbol(true);

					Expect(Symbol.Colon, true);

					// optional char (lower)
					if (_CurrentSymbol == Symbol.RAngle)
						output = new Label(Label.SpecialSymbolEpsilon);
					else
						output = ParseSymbol(true);

					// expect ec_RAngle
					Expect(Symbol.RAngle);

					// emit transducer transition
					return new TransitionNode(input, output);
				}
			default:
				{
					Label input = ParseSymbol(false);
					if (input == null)
						throw new Exception("Invalid transducer symbol syntax");
					return new TransitionNode(input, new Label(input));
				}
			}
		}

		private Label ParseSymbol(bool insideTransducerSymbol)
		{
			if (_CurrentSymbol == Symbol.EOF)
				throw new Exception("Premature end of input");

			if (_CurrentSymbol == Symbol.Char)
			{
				// simple char
				Label l = new Label(_CurrentChar);
				Scan(insideTransducerSymbol);
				return l;
			}

			switch (_CurrentSymbol)
			{
			case Symbol.SpecialDigit:
				Scan(false);
				return new Label(Label.SpecialSymbolDigit);
			case Symbol.SpecialWhitespace:
				Scan(false);
				return new Label(Label.SpecialSymbolWhitespace);
			case Symbol.AnchorBOL:
				Scan(false);
				return new Label(Label.SpecialSymbolBeginningOfLine);
			case Symbol.AnchorEOL:
				Scan(false);
				return new Label(Label.SpecialSymbolEndOfLine);
			case Symbol.AnchorBOW:
				Scan(false);
				return new Label(Label.SpecialSymbolBeginningOfWord);
			case Symbol.AnchorEOW:
				Scan(false);
				return new Label(Label.SpecialSymbolEndOfWord);
			}

			// no label found
			return null;
		}

		private void SetCurrentChar(char c)
		{
			_CurrentSymbol = Symbol.Char;
			_CurrentChar = c;
		}


		/// <summary>
		/// Reads the next input symbol from the expression, sets _CurrentSymbol
		/// and, if that's a Symbol.Char, also _CurrentChar
		/// </summary>
		private void Scan(bool insideTransducerSymbol)
		{
			if (_InputPosition >= _InputLength)
			{
				_CurrentSymbol = Symbol.EOF;
				return;
			}

			ReadUnicodeEscape();
			if (_CurrentSymbol == Symbol.Char)
			{
				// it was indeed a Unicode escape -- nothing else to do
				return;
			}

			char current = CharAt(_InputPosition);
			++_InputPosition;

			switch (current)
			{
			case 'U':
			case 'u':
				{
					// TODO shouldn't this be handled by ReadUnicodeEscape() above?
					int d1;
					int d2;
					int d3;
					int d4;

					if (_InputPosition + 4 < _InputPosition
						&& CharAt(_InputPosition) == '+'
						&& (d1 = GetHexValue(CharAt(_InputPosition + 1))) >= 0
						&& (d2 = GetHexValue(CharAt(_InputPosition + 2))) >= 0
						&& (d3 = GetHexValue(CharAt(_InputPosition + 3))) >= 0
						&& (d4 = GetHexValue(CharAt(_InputPosition + 4))) >= 0)
					{
						_InputPosition += 5;
						_CurrentSymbol = Symbol.Char;
						_CurrentChar = (char)(d1 * 4096 + d2 * 256 + d3 * 16 + d4);
					}
					else
					{
						_CurrentSymbol = Symbol.Char;
						_CurrentChar = current;
					}
				}
				break;
			case '\\':
				if (_InputPosition < _InputLength)
				{
					_CurrentChar = CharAt(_InputPosition);
					switch (_CurrentChar)
					{
					case 's':
						_CurrentSymbol = Symbol.SpecialWhitespace;
						break;
					case 'd':
						_CurrentSymbol = Symbol.SpecialDigit;
						break;
					default:
						_CurrentSymbol = Symbol.Char;
						break;
					}
					++_InputPosition;
				}
				else
					throw new Exception("Trailing backslash");
				break;
			case '(':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.LParen;
				break;
			case ')':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.RParen;
				break;
			case '<':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.LAngle;
				break;
			case '>':
				_CurrentSymbol = Symbol.RAngle;
				break;
			case ':':
				_CurrentSymbol = Symbol.Colon;
				break;
			case '|':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.OpDisj;
				break;
			case '*':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.OpKleene;
				break;
			case '+':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.OpPlus;
				break;
			case '?':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.OpOpt;
				break;
			case '^':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.AnchorBOL;
				break;
			case '$':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
					_CurrentSymbol = Symbol.AnchorEOL;
				break;
			case '#':
				if (insideTransducerSymbol)
					SetCurrentChar(current);
				else
				{
					_CurrentSymbol = Symbol.Char;
					_CurrentChar = current;

					if (_InputPosition < _InputLength)
					{
						char next = _Input[_InputPosition];
						if (next == '<')
						{
							_CurrentSymbol = Symbol.AnchorBOW;
							++_InputPosition;
						}
						else if (next == '>')
						{
							_CurrentSymbol = Symbol.AnchorEOW;
							++_InputPosition;
						}
					}
				}
				break;

			default:
				SetCurrentChar(current);
				break;
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

		private void ReadUnicodeEscape()
		{
			// TODO we may need to use UInt32 for Unicode values beyond 0xFFFF, 
			//  and will then also need to extend the number of allowed digits.

			// "U+XXXX"
			if (_InputPosition + 5 >= _InputLength)
			{
				_CurrentSymbol = Symbol.None;
				return;
			}

			if (_Input[_InputPosition] != '\\' || _Input[_InputPosition + 1] != 'u')
			{
				_CurrentSymbol = Symbol.None;
				return;
			}

			string digitSequence = _Input.Substring(_InputPosition + 2, 4);

			ushort r;
			if (UInt16.TryParse(digitSequence, System.Globalization.NumberStyles.AllowHexSpecifier,
				System.Globalization.CultureInfo.InvariantCulture, out r))
			{
				char result = Convert.ToChar(r);
				// skip digits and "U+" prefix 
				_InputPosition += 6;
				// TODO we don't distinguish "no \u escape" from "\u0000"
				_CurrentSymbol = Symbol.Char;
				_CurrentChar = result;
			}
			else
				_CurrentSymbol = Symbol.None;
		}

		private char CharAt(int position)
		{
			if (position >= _InputLength)
				return '\0';
			return _Input[position];
		}

	}

}
