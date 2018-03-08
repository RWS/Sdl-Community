using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	public enum TrailingContext
	{
		/// <summary>
		/// Indicates that no particular trailing context is required.
		/// </summary>
		None,
		/// <summary>
		/// Indicates that a trailing nonword character is always required, independent of the culture
		/// </summary>
		RequireNonwordCharacter,
		/// <summary>
		/// Indicates that a trailing nonword character is required if the current culture uses
		/// blanks as word separators, but that no such character is required if that's not the
		/// case.
		/// </summary>
		Auto
	}

	public class GenericRecognizerConfigurations
	{
		public GenericRecognizerConfigurations()
		{
			Configurations = new List<GenericRecognizerConfiguration>();
		}

		public void Add(GenericRecognizerConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException();

			if (Configurations == null)
				Configurations = new List<GenericRecognizerConfiguration>();

			Configurations.Add(configuration);
		}

		[System.Xml.Serialization.XmlElement("GenericRecognizer")]
		public List<GenericRecognizerConfiguration> Configurations;

		public void Save(string fileName)
		{
			using (System.IO.TextWriter wtr = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.UTF8))
			{
				Save(wtr);
			}
		}

		public void Save(System.IO.TextWriter wtr)
		{
			System.Xml.Serialization.XmlSerializer ser
				= new System.Xml.Serialization.XmlSerializer(this.GetType());
			ser.Serialize(wtr, this);
		}

		public static GenericRecognizerConfigurations Load(string fileName)
		{
			using (System.IO.TextReader rdr = new System.IO.StreamReader(fileName, System.Text.Encoding.UTF8, true))
			{
				return Load(rdr);
			}
		}

		public static GenericRecognizerConfigurations Load(System.IO.TextReader rdr)
		{
			System.Xml.Serialization.XmlSerializer ser
				= new System.Xml.Serialization.XmlSerializer(typeof(GenericRecognizerConfigurations));
			return ser.Deserialize(rdr) as GenericRecognizerConfigurations;
		}
	}

	public class GenericRecognizerConfiguration
	{
		public GenericRecognizerConfiguration()
		{
		}

		public GenericRecognizerConfiguration(bool enabled, int priority, bool ignoreCase,
			bool autoSubstitutable, TrailingContext tc, string tokenClass, string firstSet, string rx)
		{
			if (String.IsNullOrEmpty(tokenClass))
				throw new ArgumentNullException("tokenClass");
			if (String.IsNullOrEmpty(rx))
				throw new ArgumentNullException("rx");

			Enabled = enabled;
			Priority = priority;
			IgnoreCase = ignoreCase;
			AutoSubstitutable = autoSubstitutable;
			TrailingContext = tc;
			TokenClass = tokenClass;
			Regex = rx;
			First = firstSet;

			Cultures = null;
		}

		[System.Xml.Serialization.XmlAttribute("enabled")]
		public bool Enabled;

		[System.Xml.Serialization.XmlAttribute("priority")]
		public int Priority;

		[System.Xml.Serialization.XmlAttribute("ignorecase")]
		public bool IgnoreCase;

		[System.Xml.Serialization.XmlAttribute("autosubstitutable")]
		public bool AutoSubstitutable;

		[System.Xml.Serialization.XmlAttribute("trailingcontext")]
		public TrailingContext TrailingContext;

		[System.Xml.Serialization.XmlElement("tokenclass")]
		public string TokenClass;


		[System.Xml.Serialization.XmlElement("cultures")]
		public string Cultures;

		[System.Xml.Serialization.XmlElement("firstset")]
		public string First;

		[System.Xml.Serialization.XmlElement("regex")]
		public string Regex;
	}
}
