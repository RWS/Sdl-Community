using System;
using System.Collections.Generic;
using System.Text;

// disable warnings about missing XML comments for internal classes:
// #pragma warning disable 1591

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	internal abstract class Recognizer
	{
		protected Core.Tokenization.TokenType _Type;
		protected int _Priority;
		private bool _OnlyIfFollowedByNonwordCharacter;
		private string _TokenClassName;
		private string _RecognizerName;
		protected Core.CharacterSet _AdditionalTerminators;
		protected bool _IsFallbackRecognizer;
		protected bool _OverrideFallbackRecognizer;
		protected bool _AutoSubstitutable;

		public Recognizer(Core.Tokenization.TokenType t, 
			int priority, 
			string tokenClassName, 
			string recognizerName)
			: this(t, priority, tokenClassName, recognizerName, false)
		{
		}

		public Recognizer(Core.Tokenization.TokenType t,
			int priority,
			string tokenClassName,
			string recognizerName, 
			bool autoSubstitutable)
		{
			_Type = t;
			_Priority = priority;
			_TokenClassName = tokenClassName;
			_RecognizerName = recognizerName;
			_AdditionalTerminators = null;
			_IsFallbackRecognizer = false;
			_OverrideFallbackRecognizer = false;
			_AutoSubstitutable = autoSubstitutable;
		}

		public abstract Core.Tokenization.Token Recognize(string s, int from, bool allowTokenBundles, ref int consumedLength);

		public Core.Tokenization.TokenType Type
		{
			get { return _Type; }
		}

		public bool IsFallbackRecognizer
		{
			get { return _IsFallbackRecognizer; }
		}

		/// <summary>
		/// If <c>true</c>, tokens recognized by this recognizer will override the tokens recognized
		/// by the default fallback recognizer, even if they are shorter. If <c>false</c>, the longest
		/// match will win.
		/// </summary>
		public bool OverrideFallbackRecognizer
		{
			get { return _OverrideFallbackRecognizer; }
			set { _OverrideFallbackRecognizer = value; }
		}

		public string TokenClassName
		{
			get { return _TokenClassName; }
		}

		public string RecognizerName
		{
			get { return _RecognizerName; }
		}

		public int Priority
		{
			get { return _Priority; }
		}

		// TODO extend that in a post-context RX, and make it culture-dependent
		public bool OnlyIfFollowedByNonwordCharacter
		{
			get { return _OnlyIfFollowedByNonwordCharacter; }
			set { _OnlyIfFollowedByNonwordCharacter = value; }
		}

		protected Core.CharacterSet AdditionalTerminators
		{
			get { return _AdditionalTerminators; }
			set { _AdditionalTerminators = value; }
		}

		/// <summary>
		/// Checks whether additional terminating context constraints are fulfilled and 
		/// returns true if so, or if none are set.
		/// </summary>
		protected bool VerifyContextConstraints(string s, int p)
		{
			// TODO extend into post-context RX or add other classes (punct, etc.) to the test.

			if (_AdditionalTerminators != null)
			{
				if (p < s.Length)
				{
					if (_AdditionalTerminators.Contains(s[p]))
						return true;
				}
			}

			if (_OnlyIfFollowedByNonwordCharacter)
			{
				// NOTE we apply a heuristics that "strange" scripts will also trigger a break. This 
				//  is important for Japanese measurement recognition. 
				// TODO refine the applicable context restritions (e.g. add "script change" rule, etc.)
				if (p < s.Length)
				{
					if (System.Char.IsWhiteSpace(s, p)
						|| System.Char.IsPunctuation(s, p)
						|| System.Char.IsSeparator(s, p)
						|| System.Char.IsSymbol(s, p) // new 20090320 ochrist
						|| s[p] >= '\u0100'
						)
					{
						return true;
					}
					else
						return false;
				}
			}

			return true;
		}

	}
}
