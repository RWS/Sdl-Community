using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	/// <summary>
	/// A simple, fully serializable class which holds a tokenizer's settings. This is just
	/// a data container for serialization which excuses that all fields are public.
	/// </summary>
	public class TokenizerSetup
	{
		private System.Globalization.CultureInfo _Culture;

		/// <summary>
		/// TODO: Oli
		/// </summary>
		[System.Xml.Serialization.XmlAttribute("createWhitespaceTokens")]
		public bool CreateWhitespaceTokens;

		/// <summary>
		/// TODO: Oli
		/// </summary>
		[System.Xml.Serialization.XmlAttribute("breakOnWhitespace")]
		public bool BreakOnWhitespace;

		/// <summary>
		/// Gets or sets the language in the form of a locale string.
		/// </summary>
		[System.Xml.Serialization.XmlAttribute("culture")]
		public string CultureName
		{
			get { return _Culture.Name; }
			set { _Culture = Core.CultureInfoExtensions.GetCultureInfo(value); }
		}

		/// <summary>
		/// If true, selected clitics will be stripped by the fallback recognizer,
		/// for romance languages (leading, l' d') as well as English (trailing 've 's, 'm, 'll n't 're). 
		/// Note that this is not yet fully implemented. Also, separated clitics are not always correctly
		/// classified as stop words.
		/// </summary>
		public bool SeparateClitics;

		[System.Xml.Serialization.XmlIgnore]
		public System.Globalization.CultureInfo Culture
		{
			get { return _Culture; }
			set { _Culture = value; }
		}

		/// <summary>
		/// Gets or sets a value representing which recognizers are enabled.
		/// </summary>
		public Core.Tokenization.BuiltinRecognizers BuiltinRecognizers;
	}
}
