using System;
using System.Collections.Generic;
using System.Text;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace Sdl.LanguagePlatform.Lingua.Tokenization
{
	public class TokenizerSetupFactory
	{
		/// <summary>
		/// Create a default tokenizer setup object for the given culture. Will recognize acronyms, 
		/// dates, measurements, numbers, and time expressions, and uses default fallback recognizers.
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public static TokenizerSetup Create(System.Globalization.CultureInfo culture)
		{
			BuiltinRecognizers defaultRecognizers = BuiltinRecognizers.RecognizeAcronyms
				| BuiltinRecognizers.RecognizeDates
				| BuiltinRecognizers.RecognizeMeasurements
				| BuiltinRecognizers.RecognizeNumbers
				| BuiltinRecognizers.RecognizeTimes;

			return Create(culture, defaultRecognizers);
		}

		/// <summary>
		/// Create a default tokenizer setup object for the given culture, using the specified 
		/// recognizers and the fallback recognizer
		/// </summary>
		/// <param name="culture"></param>
		/// <returns></returns>
		public static TokenizerSetup Create(System.Globalization.CultureInfo culture, BuiltinRecognizers recognizers)
		{
			TokenizerSetup setup = new TokenizerSetup();

			setup.BreakOnWhitespace = Core.CultureInfoExtensions.UseBlankAsWordSeparator(culture);

			setup.Culture = culture;
			setup.CreateWhitespaceTokens = false;
			setup.BuiltinRecognizers = recognizers;

			return setup;
		}

		public static void Write(TokenizerSetup setup, System.IO.Stream output)
		{
			// TODO switch to DataContractSerializer 
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(setup.GetType());
			ser.Serialize(output, setup);
		}

		public static void Write(TokenizerSetup setup, System.IO.TextWriter output)
		{
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(setup.GetType());
			ser.Serialize(output, setup);
		}

		public static void Write(TokenizerSetup setup, string path)
		{
			using (System.IO.StreamWriter wtr = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8))
			{
				Write(setup, wtr);
			}
		}

		public static TokenizerSetup Load(System.IO.Stream input)
		{
			// TODO versioning and backwards/forwards compatibility
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(TokenizerSetup));
			return ser.Deserialize(input) as TokenizerSetup;
		}

		public static TokenizerSetup Load(System.IO.StreamReader input)
		{
			// TODO versioning and backwards/forwards compatibility
			System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(TokenizerSetup));
			return ser.Deserialize(input) as TokenizerSetup;
		}

		public static TokenizerSetup Load(Core.Resources.IResourceDataAccessor accessor,
			System.Globalization.CultureInfo culture)
		{
			using (System.IO.Stream dataStream = accessor.ReadResourceData(culture,
				Core.Resources.LanguageResourceType.TokenizerSettings, true))
			{
				return Load(dataStream);
			}
		}
		public static TokenizerSetup Load(string path)
		{
			using (System.IO.StreamReader rdr = new System.IO.StreamReader(path, System.Text.Encoding.UTF8, true))
			{
				return Load(rdr);
			}
		}


	}
}
