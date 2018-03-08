using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	public class SimilarityComputers
	{
		public static double GetCharSimilarity(char a, char b)
		{
			if (a == b)
				return 1.0d;
			char la = Char.ToLowerInvariant(a);
			char lb = Char.ToLowerInvariant(b);

			if (la == lb)
				return 0.95d;

			char ba = Core.CharacterProperties.ToBase(la);
			char bb = Core.CharacterProperties.ToBase(lb);

			if (ba == bb)
				return 0.9d;

			return 0.0d;
		}

		public static double GetStringSimilarity(string a, string b)
		{
			if (a.Equals(b, StringComparison.Ordinal))
				return 1.0d;

			if (a.Equals(b, StringComparison.OrdinalIgnoreCase))
				return 0.95d;

			Lingua.EditDistance.EditDistanceComputer<char> edc
				= new Lingua.EditDistance.EditDistanceComputer<char>(GetCharSimilarity);

			Core.EditDistance.EditDistance result
				= edc.ComputeEditDistance(a.ToCharArray(), b.ToCharArray());

			return result.Score;
		}

		public static double GetPlaceableSimilarity(Core.Tokenization.Token a, Core.Tokenization.Token b, 
			Core.Tokenization.BuiltinRecognizers disabledAutoSubstitutions)
		{
			/*
			 * identical type and value - 1.0
			 * similar type and value - 0.85
			 * identical type - 0.7
			 * otherwise - 0.0
			 * */

			if (!(a.IsPlaceable && b.IsPlaceable))
			{
				System.Diagnostics.Debug.Assert(false, "Expect placeable types");
				return 0.0d;
			}

			// Can't compare different types
			if (a.Type != b.Type || a.GetType() != b.GetType())
				return 0.0d;

			Core.Tokenization.TagToken ta = a as Core.Tokenization.TagToken;
			Core.Tokenization.TagToken tb = b as Core.Tokenization.TagToken;

			if (ta != null || tb != null)
			{
				if (ta == null || tb == null)
					return -1.0d; // can't align two placeables of which one isn't a tag

				if (ta.Tag.Type != tb.Tag.Type)
					return -1.0d; // can't align tags of different types
			}

			if (disabledAutoSubstitutions != Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone)
			{
				bool requireEquality = false;

				switch (a.Type)
				{
				case Sdl.LanguagePlatform.Core.Tokenization.TokenType.Abbreviation:
					requireEquality = (disabledAutoSubstitutions & Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeAcronyms)
						!= Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone;
					break;
				case Sdl.LanguagePlatform.Core.Tokenization.TokenType.Date:
					requireEquality = (disabledAutoSubstitutions & Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeDates)
						!= Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone;
					break;
				case Sdl.LanguagePlatform.Core.Tokenization.TokenType.Time:
					requireEquality = (disabledAutoSubstitutions & Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeTimes)
						!= Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone;
					break;
				case Sdl.LanguagePlatform.Core.Tokenization.TokenType.Variable:
					requireEquality = (disabledAutoSubstitutions & Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeVariables)
						!= Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone;
					break;
				case Sdl.LanguagePlatform.Core.Tokenization.TokenType.Number:
					requireEquality = (disabledAutoSubstitutions & Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNumbers)
						!= Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone;
					break;
				case Sdl.LanguagePlatform.Core.Tokenization.TokenType.Measurement:
					requireEquality = (disabledAutoSubstitutions & Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeMeasurements)
						!= Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNone;
					break;
				default:
					requireEquality = false;
					break;
				}

				if (requireEquality)
				{
					if (a.Equals(b))
						return 1.0d;
					else
						return 0.7d;
				}
			}

			switch (a.GetSimilarity(b))
			{
			case Core.SegmentElement.Similarity.None:
				// in this case the tokens are not similar, but the token types are identical (checked above)
				return 0.7d;
			case Core.SegmentElement.Similarity.IdenticalType:
				return 0.85d;
			case Core.SegmentElement.Similarity.IdenticalValueAndType:
				return 1.0d;
			default:
				return 0.0d;
			}
		}

		public static double GetTokenSimilarity(Core.Tokenization.Token a, Core.Tokenization.Token b, 
			bool useStringEditDistance, 
			Core.Tokenization.BuiltinRecognizers disabledAutoSubstitutions)
		{
			/*
			 * identical form - 1.0
			 * identical case-insensitive - 0.9
			 * stem-identical or same placeable type - 0.85
			 * identical type - 0.4
			 * */

			// TODO consider normalization of token texts, or let the tokenizer store the text in normalized form. 

			Core.Tokenization.TagToken ta = a as Core.Tokenization.TagToken;
			Core.Tokenization.TagToken tb = b as Core.Tokenization.TagToken;

			bool aIsTag = (ta != null);
			bool bIsTag = (tb != null);

			if (aIsTag != bIsTag
				|| a.IsWhitespace != b.IsWhitespace
				|| a.IsPunctuation != b.IsPunctuation)
			{
				// comparing a tag with a non-tag results in no-change-allowed similarity (<0)
				// same for whitespace, punctuation
				return -1.0d;
			}

			if (aIsTag && bIsTag)
			{
				System.Diagnostics.Debug.Assert(ta.Tag != null && tb.Tag != null);
				if (ta.Tag.Type == tb.Tag.Type)
					// assignable
					return 0.95d;
				else if ((ta.Tag.Type == Sdl.LanguagePlatform.Core.TagType.Standalone
					 && tb.Tag.Type == Sdl.LanguagePlatform.Core.TagType.TextPlaceholder)
					|| 
					(ta.Tag.Type == Sdl.LanguagePlatform.Core.TagType.TextPlaceholder
					 && tb.Tag.Type == Sdl.LanguagePlatform.Core.TagType.Standalone))
				{
					// one placeholder, one text placeholder
					return 0.85d;
				}
				else
					// not assignable
					return -1.0d;
			}

			double malus = 0.0d;
			double sim = 0.0d;

			if (a.IsPlaceable && b.IsPlaceable)
				return GetPlaceableSimilarity(a, b, disabledAutoSubstitutions);

			if (a.Text == null || b.Text == null)
			{
				System.Diagnostics.Debug.Assert(false, "Expected non-null token text. Let Oli know if this assertion fires and provide test data.");
				return 0.0d;
			}

			if (a.IsWord != b.IsWord)
				// tokens of different types - reduce similarity accordingly
				// NOTE only checks whether both are words or non-words
				malus = 0.1d;

			if (a.Text.Equals(b.Text, StringComparison.Ordinal))
				sim = 1.0d;
			else if (a.IsWhitespace || a.IsPunctuation)
			{
				// slightly less than the SegmentEditDistanceComputer's move threshold, as
				//  we don't want to move them around
				sim = 0.94d;
			}
			else if (a.Text.Equals(b.Text, StringComparison.OrdinalIgnoreCase))
			{
				// we want to detect moves for such tokens, so:
				sim = 0.95d; // the SegmentEditDistanceComputer's move threshold
			}
			else if (a is Core.Tokenization.SimpleToken && b is Core.Tokenization.SimpleToken)
			{
				Core.Tokenization.SimpleToken ast = a as Core.Tokenization.SimpleToken;
				Core.Tokenization.SimpleToken bst = b as Core.Tokenization.SimpleToken;

				if (ast != null && bst != null
					&& ast.Stem != null && bst.Stem != null
					&& ast.Stem.Equals(bst.Stem, StringComparison.OrdinalIgnoreCase))
					sim = 0.85d;
				else
					sim = useStringEditDistance
						? 0.95d * GetThreshold(GetStringSimilarity(a.Text, b.Text))
						: 0.0d;
			}
			else
			{
				// strings are not identical or identical w/ ignore case
				sim = useStringEditDistance
					? 0.95d * GetThreshold(GetStringSimilarity(a.Text, b.Text))
					: 0.0d;
			}

			return Math.Max(0.0d, sim - malus);
		}

		private static double GetThreshold(double sim)
		{
			if (sim == 1.0d)
				return 1.0d;
			else if (sim >= 0.9d)
				return 0.9d;
			else if (sim >= 0.75d)
				return 0.75d;
			else if (sim >= 0.5d)
				return 0.5d;
			else
				return 0.0d;
		}

	}
}
