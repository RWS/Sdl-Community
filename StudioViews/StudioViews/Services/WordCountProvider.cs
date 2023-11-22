using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.LanguagePlatform.TranslationMemory;

namespace Sdl.Community.StudioViews.Services
{
	public class WordCountProvider
	{
		private readonly MethodInfo _createTokenizerSetup;
		private readonly FieldInfo _builtinRecognizerProp;
		private readonly object _builtinRecognizeAllEnumValue;
		private readonly MethodInfo _tokenizerGetTokensAsync;
		private readonly MethodInfo _tokenizerCreateAsync;


		public WordCountProvider()
		{
			var location = Assembly.GetEntryAssembly()?.Location;

			var assemblyLanguageProcessing = Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(location), "Sdl.Core.LanguageProcessing.dll"));
			var tokenizerSetupFactoryType = assemblyLanguageProcessing.GetType("Sdl.Core.LanguageProcessing.Tokenization.TokenizerSetupFactory");
			_createTokenizerSetup = tokenizerSetupFactoryType.GetMethod("Create", new[] { typeof(CultureCode) });

			var cultureCode = GetCultureCode(CultureInfo.CurrentCulture.Name);

			var setup = _createTokenizerSetup?.Invoke(null, new object[] { cultureCode });

			_builtinRecognizerProp = setup?.GetType().GetField("BuiltinRecognizers");
			var fieldInfo = _builtinRecognizerProp?.FieldType.GetFields().First(f => f.Name == "RecognizeAll");
			_builtinRecognizeAllEnumValue = Enum.ToObject(_builtinRecognizerProp.FieldType, fieldInfo.GetRawConstantValue());

			var tokenizerType = assemblyLanguageProcessing.GetType("Sdl.Core.LanguageProcessing.Tokenization.Tokenizer");

			var cultureMetadataManager = LanguageRegistryApi.Instance.CultureMetadataManager;
			_tokenizerCreateAsync = tokenizerType.GetMethod("CreateAsync",
				BindingFlags.Public | BindingFlags.Static, null, new Type[] { setup.GetType(), cultureMetadataManager.GetType() }, null);
			_tokenizerGetTokensAsync = tokenizerType.GetMethod("GetTokensAsync",
				BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Segment), typeof(bool) }, null);
		}

		public WordCounts GetWordCounts(ISegment segment, CultureInfo language)
		{
			var languagePlatformSegment = GetLanguagePlatformSegment(segment, language);
			return GetWordCounts(languagePlatformSegment, language);
		}

		public WordCounts GetWordCounts(Segment segment, CultureInfo language)
		{
			TokenizeSegment(segment, language);
			return GetWordCounts(segment.Tokens, language);
		}

		private void TokenizeSegment(Segment segment, CultureInfo language)
		{
			var setup = GetTokenizerSetup(language);
			var tokenizer = GetTokenizer(setup);
			var tokens = GetTokens(segment, tokenizer);

			segment.Tokens = tokens;
		}

		private object GetTokenizerSetup(CultureInfo language)
		{
			var cultureCode = GetCultureCode(language.Name);
			var setup = _createTokenizerSetup.Invoke(null, new object[] { cultureCode });

			_builtinRecognizerProp.SetValue(setup, _builtinRecognizeAllEnumValue);
			setup.GetType().GetField("CreateWhitespaceTokens").SetValue(setup, true);
			return setup;
		}

		private object GetTokenizer(object setup)
		{
			var cultureMetadataManager = LanguageRegistryApi.Instance.CultureMetadataManager;
			var tokenizerTask = (System.Threading.Tasks.Task)_tokenizerCreateAsync.Invoke(null, new object[] { setup, cultureMetadataManager });
			tokenizerTask.Wait();
			var tokenizerResult = tokenizerTask.GetType().GetProperty("Result")?.GetValue(tokenizerTask);
			return tokenizerResult;
		}

		private List<Token> GetTokens(Segment segment, object tokenizerResult)
		{
			var tokensTask = (System.Threading.Tasks.Task)_tokenizerGetTokensAsync.Invoke(tokenizerResult, new object[] { segment, true });
			tokensTask.Wait();
			var tokensResult = tokensTask.GetType().GetProperty("Result")?.GetValue(tokensTask);
			return tokensResult as List<Token>;
		}

		private WordCounts GetWordCounts(IList<Token> tokens, CultureInfo language)
		{
			var cultureCode = GetCultureCode(language.Name);
			var cultureMetadataManager = LanguageRegistryApi.Instance.CultureMetadataManager;
			var wordCountsOptions = new WordCountsOptions
			{
				BreakAdvancedTokensByCharacter = false,
				BreakOnApostrophe = false,
				BreakOnDash = false,
				BreakOnHyphen = false,
				BreakOnTag = true
			};

			var wordCounts = WordCounts.CreateWordCountsAsync(tokens, wordCountsOptions, cultureCode, cultureMetadataManager).GetAwaiter().GetResult();
			return wordCounts;
		}

		private Segment GetLanguagePlatformSegment(ISegment segment, CultureInfo culture)
		{
			var visitor = new LanguagePlatformSegmentVisitor(culture, false);
			visitor.VisitSegment(segment);

			return visitor.Segment;
		}

		private CultureCode GetCultureCode(string cultureIsoCode)
		{
			try
			{
				// Language registry contains all the languages that are supported in Studio               
				var language = LanguageRegistryApi.Instance.GetLanguage(cultureIsoCode);
				return new CultureCode(language.CultureInfo);
			}
			catch (UnsupportedLanguageException)
			{
				// In case the language is not supported an exception is thrown
				return null;
			}
		}
	}
}
