using System.IO;
using System.Reflection;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using Sdl.Community.Toolkit.Core;

	/// <summary>
	/// Class contains extentsion methods to simplify work with strings
	/// </summary>
	public static class TextHelper
    {
		private static string _studioInstalledLocation = Path.GetDirectoryName(GetInstalledStudioVersion().InstallPath);

        /// <summary>
        /// Cutting substring from the string
        /// </summary>
        /// <param name="s">String to cut from</param>
        /// <param name="indexStart">Index of the start of substring</param>
        /// <param name="indexEnd">Index of the end of substring</param>
        /// <returns>Substring of the source string</returns>
        public static string Cut(this string s, int indexStart, int indexEnd)
        {
            return s.Substring(indexStart, indexEnd - indexStart + 1);
        }

        /// <summary>
        /// Removes substring from the given string
        /// </summary>
        /// <param name="s">The source string</param>
        /// <param name="indexStart">The position to begin deleting characters</param>
        /// <param name="indexEnd">The end position</param>
        /// <returns>New string</returns>
        public static string RemovePart(this string s, int indexStart, int indexEnd)
        {
            return s.Remove(indexStart, indexEnd - indexStart + 1);
        }

        /// <summary>
        /// Returns attribute value
        /// </summary>
        /// <param name="tagContent">Content of the tag</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <returns>Attribute value</returns>
        public static string AttributeValue(this string tagContent, string attributeName)
        {
            string attributeValue = string.Empty;

            int indexStart = tagContent.IndexOf(attributeName + @"=""");

            if (indexStart != -1)
            {
                indexStart = indexStart + attributeName.Length + 2;
                int indexForSearch = indexStart;

	            int indexOfEnd = tagContent.IndexOf('"', indexForSearch);

                attributeValue = tagContent.Cut(indexStart, indexOfEnd - 1);
            }

            return attributeValue;
        }

        /// <summary>
        /// Extracts all pairs which have the following format: key="value".
        /// </summary>
        /// <param name="text">The string from which values should be extracted.</param>
        /// <returns>The extracted values in pair. (eg. pair.Key="key" pair.Value="value")</returns>
        public static IEnumerable<KeyValuePair<string, string>> AttributeValues(this string text)
        {
            int p = 0;
            bool quoteOpened = false;
            List<string> attributes = new List<string>();

            for (int i = 0; i < text.Length; i++)
            {
                if (!quoteOpened && (text[i] == ' ' || text[i] == '<' || text[i] == '>'))
                {
                    p = i + 1;
                }
                else if (text[i] == '\"')
                {
                    if (quoteOpened)
                    {
                        attributes.Add(text.Cut(p, i));
                        quoteOpened = false;
                        p = -1;
                    }
                    else
                    {
                        if (p >= 0)
                        {
                            quoteOpened = true;
                        }
                    }
                }
            }

            foreach (string attribute in attributes)
            {
                char quote = '\"';
                int s = attribute.IndexOf("=" + quote);
                int e = attribute.IndexOf(quote, s + 2);

                if (s != -1 && e != -1 && attribute.LastIndexOf(quote) == e)
                {
                    string name = attribute.Substring(0, s);
                    string value = attribute.Cut(s + 2, e - 1);

                    yield return new KeyValuePair<string, string>(name, value);
                }
            }
        }

        /// <summary>
        /// Removes given attribute from the string.
        /// </summary>
        /// <param name="text">The source string</param>
        /// <param name="attribute">The attribute to remove</param>
        /// <returns>The source string without the given attribute</returns>
        public static string RemoveAttribute(this string text, string attribute)
        {
            char quote = '\"';
            int startIndex;
            while ((startIndex = text.IndexOf(attribute)) != -1)
            {
                int openQoute = text.IndexOf(quote, startIndex);
                int closeQuote = text.IndexOf(quote, openQoute + 1);
                text = text.RemovePart(startIndex, closeQuote);
                if (text[startIndex - 1] == ' ')
                {
                    text = text.Remove(startIndex - 1, 1);
                }
            }

            return text;
        }

        /// <summary>
        /// Replace value of attribute
        /// </summary>
        /// <param name="text">The source string</param>
        /// <param name="attribute">The attribute to remove</param>
        /// /// <param name="newValueOfAttribute">New value of attribute</param>
        /// <returns>The source string with replaced attribute</returns>
        public static string ReplaceAttribute(this string text, string attribute, string newValueOfAttribute)
        {
            string oldValue = text.AttributeValue(attribute);

            if (!string.IsNullOrEmpty(oldValue))
            {
                string oldAttribute = string.Format("{0}=\"{1}\"", attribute, oldValue);
                string newAttribute = string.Format("{0}=\"{1}\"", attribute, newValueOfAttribute);

                return text.Replace(oldAttribute, newAttribute);
            }

            return text;
        }

        /// <summary>
        /// Extracts value of the tag content
        /// </summary>
        /// <param name="tagContent">Content of the tag</param>
        /// <returns>The value of the tag content</returns>
        public static string ExtractValue(this string tagContent)
        {
            int startIndex = tagContent.IndexOf(">");
            if (startIndex == -1)
            {
                startIndex = 0;
            }
            else
            {
                startIndex++;
            }

            return TextHelper.Cut(tagContent, startIndex, tagContent.Length - 1);
        }

        /// <summary>
        /// Replace value between two tags
        /// </summary>
        /// <param name="text">text for replacment</param>
        /// <param name="tag1Start">Tag after which replacment is done. Tag can be specifyed fully or only by start of it.</param>
        /// <param name="tag2Start">Tag before which replacemnt is done. Tag can be specifyed fully or only by start of it.</param>
        /// <param name="newValue">New value that will be inserted between tags.</param>
        /// <returns>text with replaced value</returns>
        public static string ReplaceValue(this string text, string tag1Start, string tag2Start, string newValue)
        {
            int indexStart = text.IndexOf(tag1Start);

            if (indexStart > -1)
            {
                if (tag1Start.IndexOf("<") != -1)
                {
                    indexStart = text.IndexOf(">", indexStart);
                }
                else
                {
                    indexStart = indexStart + tag1Start.Length;
                }

                if (indexStart > -1)
                {
                    int indexTitleEnd = text.IndexOf(tag2Start);

                    if (indexTitleEnd > -1)
                    {
                        return string.Format("{0}{1}{2}", text.Substring(0, indexStart + 1), newValue, text.Substring(indexTitleEnd));
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Returns first integer, finiding from start position
        /// </summary>
        /// <param name="stringToSearch">String where to search</param>
        /// <param name="startPosition">Position from where to start searching</param>
        /// <returns>Integer if string contains it. -1 if there is no numbers in the string</returns>
        public static int ExtractInt(this string stringToSearch, int startPosition)
        {
            string parsed = string.Empty;

            int i = startPosition;

            while (i < stringToSearch.Length && Char.IsDigit(stringToSearch[i]))
            {
                parsed += stringToSearch[i];
                i++;
            }

            if (string.IsNullOrEmpty(parsed))
            {
                return -1;
            }

            return Convert.ToInt32(parsed);
        }

        /// <summary>
        /// Comparing string with pattern
        /// </summary>
        /// <param name="currentString">String to compare</param>
        /// <param name="pattern">Pattern to compare with</param>
        /// <returns>Returns true if string compares with pattern</returns>
        public static bool CompareWithPattern(this string currentString, string pattern)
        {
            bool isEqual = true;

            string[] parts = pattern.Split('%');

            if (parts.Length > 1)
            {
                int i = 0;
                while (i < parts.Length && isEqual)
                {
                    if (i == 0)
                    {
                        if (currentString.StartsWith(parts[i]))
                        {
                            currentString = currentString.Substring(parts[i].Length);
                        }
                        else
                        {
                            isEqual = false;
                        }
                    }
                    else
                    {
                        if (i == (parts.Length - 1))
                        {
                            isEqual = currentString.EndsWith(parts[i]);
                        }
                        else
                        {
                            int index = currentString.IndexOf(parts[i]);

                            if (index != -1)
                            {
                                index = index + parts[i].Length;
                                currentString = currentString.Substring(index, currentString.Length - index);
                            }
                            else
                            {
                                isEqual = false;
                            }
                        }
                    }

                    i++;
                }
            }
            else
            {
                isEqual = currentString == pattern;
            }

            return isEqual;
        }

        /// <summary>
        /// Adding metadata
        /// </summary>
        /// <param name="currentString">String where to add metadata</param>
        /// <param name="key">Key string</param>
        /// <param name="metadata">Metadata string</param>
        /// <returns>String with added metadata</returns>
        public static string AddMetadata(this string currentString, string key, string metadata)
        {
            if (!String.IsNullOrEmpty(metadata))
            {
                return String.Format(@"{0} {1}=""{2}""", currentString, key, metadata);
            }

            return currentString;
        }

        /// <summary>
        /// Returns metadata as "key=value" string
        /// </summary>
        /// <param name="metaData">Source metadata</param>
        /// <returns>String attributes list</returns>
        public static string MetaDataToAttributeList(IEnumerable<KeyValuePair<string, string>> metaData)
        {
            string info = string.Empty;

            foreach (KeyValuePair<string, string> data in metaData)
            {
                info = string.Format(@"{0} {1}=""{2}""", info, data.Key, data.Value);
            }

            return info;
        }

        /// <summary>
        /// Returns metadata as "key=value" string
        /// </summary>
        /// <param name="metaData">Source metadata</param>
        /// <returns>String attributes list</returns>
        public static string MetaDataToAttributeListEasyToParse(IEnumerable<KeyValuePair<string, string>> metaData)
        {
            string info = string.Empty;

            foreach (KeyValuePair<string, string> data in metaData)
            {
                if (!string.IsNullOrEmpty(info))
                {
                    info = string.Format(@"{0}|{1}==""{2}""", info, data.Key, data.Value);
                }
                else
                {
                    info = string.Format(@"{0}==""{1}""", data.Key, data.Value);
                }
            }

            return info;
        }

        /// <summary>
        /// Converts string to float
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Float value</returns>
        public static float ToFloat(this string value)
        {
            string separator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
            value = value.Replace(".", separator);
            value = value.Replace(",", separator);
            return float.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts string to double
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>double value</returns>
        public static double ToDouble(this string value)
        {
            string separator = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;
            value = value.Replace(".", separator);
            value = value.Replace(",", separator);
            return double.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Indicates whether tag is closed with "/>"
        /// </summary>
        /// <param name="value">Tag content value</param>
        /// <returns>Bool value indicating whether tag is finished with "/>"</returns>
        public static bool IsClosedTag(this string value)
        {
            return value.EndsWith("/>");
        }

        /// <summary>
        /// Indicates whether it is a close tag part
        /// </summary>
        /// <param name="value">Tag content value</param>
        /// <returns>Bool value indicating whether it is a close tag part</returns>
        public static bool IfCloseTagPart(this string value)
        {
            return value.StartsWith("</") && value.EndsWith(">");
        }

        /// <summary>
        /// Assure tag is closed with "/>"
        /// </summary>
        /// <param name="value">Tag string value</param>
        /// <returns>New tag close with "/>"</returns>
        public static string AssureTagClosed(this string value)
        {
            if (value.EndsWith("/>"))
            {
                return value;
            }

            if (value.EndsWith(">"))
            {
                return value.Insert(value.Length - 1, @"/");
            }

            return value + "/>";
        }

        /// <summary>
        /// Gets shorten version of the string
        /// </summary>
        /// <param name="value">string to be shortened</param>
        /// <param name="charCount">number of digits</param>
        /// <returns>shortened string</returns>
        public static string ShortenString(this string value, int charCount)
        {
            string result = value;
            if (!string.IsNullOrEmpty(result))
            {
                if (result.Length > charCount)
                {
                    result = result.Substring(0, charCount) + "...";
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if the given string begins with any of the given strings.
        /// </summary>
        /// <param name="text">The text to check.</param>
        /// <param name="values">The list of values to check.</param>
        /// <returns>Returns true if any of the values is prefix of the given string, otherwise returns false.</returns>
        public static bool StartsWithAny(this string text, params string[] values)
        {
            foreach (string value in values)
            {
                if (text.StartsWith(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Getting length of the string or 0 if null
        /// </summary>
        /// <param name="text">String to get length</param>
        /// <returns>Length of the string if it's not null, 0 if string is null or empty</returns>
        public static int GetLength(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            else
            {
                return text.Length;
            }
        }

        /// <summary>
        /// Gets the node name by given node.
        /// </summary>
        /// <param name="node">The node to process</param>
        /// <returns>The node's name. For closing nodes returns full node text, eg. 'lt;/closingNodegt;'</returns>
        public static string NodeName(this string node)
        {
            if (node.StartsWith("</"))
            {
                if (node.EndsWith(">"))
                {
                    return node;
                }
            }
            else if (node.StartsWith("<"))
            {
                int endIndex = node.IndexOfAny(new char[] { ' ', '/', '>' });
                if (endIndex > 0 && node.EndsWith(">"))
                {
                    return node.Cut(0, endIndex - 1);
                }
            }

            return string.Empty;
        }

        public static string NodeStartName(this string node)
        {
            return node.Replace("<", "");
        }

        // eng only
        public static int GetWordsCountEng(this string text)
        {
            int wordsCount = 0;
            int charsInWord = 0;
            text = text + " ";

            for (int i = 0; i < text.Length; i++)
                if (char.IsWhiteSpace(text[i]) || char.IsSeparator(text[i]) || char.IsPunctuation(text[i]))
                {
                    if (text[i] != '-' && text[i] != '\'' && charsInWord > 0)
                    {
                        charsInWord = 0;
                        wordsCount++;
                    }
                }
                else charsInWord++;

            return wordsCount;
        }

        // not exact
        public static int GetWordsCount2(this string text)
        {
            return text.Split(new char[] { ' ', '.', ',', '?', '!' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        // SDL libraries reuse
        public static int GetWordsCount(this string text, string culture)
        {
			int wordsNum = 0;
			var _Culture = new CultureInfo(culture);
			var _LinguaSegment = new Segment(_Culture);
	        Assembly languageProcessingAssembly;
	        Assembly translationMemoryToolsAssembly;

			// set values to the IText (used when calling builder.VisitText() method) 
			var _textSDL = new Text(text);
	        var txt = new Common.Text(text);
	        FileTypeSupport.Framework.BilingualApi.IText _iTxt = txt;
			var txtProperties = new Common.TextProperties(text);
	        _iTxt.Properties = txtProperties;

			// make setups from Sdl.LanguagePlatform.TranslationMemoryTools.dll
			//get assembly
			if (_studioInstalledLocation.Contains(@"Program Files (x86)\SDL\SDL Trados Studio"))
	        {
				// used in Trados Studio
				translationMemoryToolsAssembly = Assembly.LoadFrom(Path.Combine(_studioInstalledLocation, "Sdl.LanguagePlatform.TranslationMemoryTools.dll"));
			}
			else
	        {
				// used in the separate app (Sdl.Community.SplitMergeUIWix) generated by the Wix installer		    
				translationMemoryToolsAssembly = Assembly.LoadFrom(Path.Combine(_studioInstalledLocation, "Sdl.LanguagePlatform.TranslationMemoryTools.dll"));
			}

			//get object type 
			var linguaSegmentBuilderType =
		        translationMemoryToolsAssembly.GetType("Sdl.LanguagePlatform.TranslationMemoryTools.LinguaSegmentBuilder");

			//create constructor type
			Type[] constructorArgumentTypes = { typeof(Segment), typeof(bool), typeof(bool) };

			//get constructor
			ConstructorInfo linguaSegmentConstrutor = linguaSegmentBuilderType.GetConstructor(constructorArgumentTypes);

			// invoke constructor with its arguments and call method/set values from builder object
			dynamic builder = linguaSegmentConstrutor.Invoke(new object[] { _LinguaSegment, false, false });
			builder.VisitText(_iTxt);
			builder.Result.Elements.Add(_textSDL);

			// make setups from Sdl.Core.LanguageProcessing.dll
	        if (_studioInstalledLocation.Contains(@"Program Files (x86)\SDL\SDL Trados Studio"))
	        {
				// used in Trados Studio
		        languageProcessingAssembly = Assembly.LoadFrom(Path.Combine(_studioInstalledLocation, "Sdl.Core.LanguageProcessing.dll"));
			}
			else
	        {
				// used in the separate app (Sdl.Community.SplitMergeUIWix) generated by the Wix installer				
				languageProcessingAssembly = Assembly.LoadFrom(Path.Combine(_studioInstalledLocation, "Sdl.Core.LanguageProcessing.dll"));
			}

			//get object type
			var tokenizationFactoryType = languageProcessingAssembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.TokenizerSetupFactory");
			dynamic tokenizerFactory = tokenizationFactoryType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);

			var createMethod = tokenizerFactory[0];
	        if (createMethod != null)
	        {
		        dynamic setup = createMethod.Invoke(null, new object[] {_Culture});

		        setup.CreateWhitespaceTokens = true;
		        setup.BuiltinRecognizers =
			        LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNumbers |
			        LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeDates |
			        LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeTimes;

		        var tokenizerType = languageProcessingAssembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.Tokenizer");
		        Type[] constructorTokenizerArgumentTypes = {setup.GetType()};
		        ConstructorInfo tokenizerConstructor = tokenizerType.GetConstructor(constructorTokenizerArgumentTypes);
		        dynamic tokenizer = tokenizerConstructor.Invoke(new object[] {setup});

		        IList<LanguagePlatform.Core.Tokenization.Token> _tokens = tokenizer.Tokenize(_LinguaSegment);
		        foreach (LanguagePlatform.Core.Tokenization.Token _token in _tokens)
		        {
			        if (_token.IsWord)
			        {
				        wordsNum++;
			        }
		        }
	        }
	        return wordsNum;
		}

		/// <summary>
		/// Get installed version for Studio5(Studio 2017). Always use Studio5 because the loaded dll's contains needed logic.
		/// The call studio.GetStudioVersion() is not working when using the installer, because the app is running outside the Studio context,
		/// so the workaround is to get all the installed studio versions and use the needed one (Studio5)
		/// </summary>
		/// <returns></returns>
		private static StudioVersion GetInstalledStudioVersion()
		{
			var studio = new Studio();
			var allStudioVersions = studio.GetInstalledStudioVersion();
			
			return allStudioVersions.Where(v=>v.Version.Equals("Studio5")).FirstOrDefault();
		}
	}
}