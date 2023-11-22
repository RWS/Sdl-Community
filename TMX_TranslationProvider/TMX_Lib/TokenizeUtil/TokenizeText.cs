﻿using Sdl.LanguagePlatform.Core.Resources;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TMX_Lib.TokenizeUtil
{
    /*
    Splits text into tokens. 
    
    the equivalend of:

    var setup = TokenizerSetupFactory.Create(CultureInfo.CurrentCulture);
    setup.CreateWhitespaceTokens = true;
    setup.BuiltinRecognizers = BuiltinRecognizers.RecognizeAll;
    var tokenizerParameters = new TokenizerParameters(setup, null);
    var tokenizer = new Tokenizer(tokenizerParameters);
    result.Tokens = tokenizer.GetTokens(segment, true);
     */
    public class TokenizeText
    {
        private readonly MethodInfo _createTokenizerSetup;
        private readonly FieldInfo _builtinRecognizerProp;
        private readonly object _builtinRecognizeAllEnumValue;
        private readonly ConstructorInfo _tokenizerParamsCtor;
        private readonly ConstructorInfo _tokenizerCtor;
        private readonly MethodInfo _tokenizerGetTokens;

        public TokenizeText()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var assembly = Assembly.LoadFile($"{programFiles}\\Trados\\Trados Studio\\Studio17\\Sdl.Core.LanguageProcessing.dll");
            var tokenizerSetupFactoryType = assembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.TokenizerSetupFactory");
            _createTokenizerSetup = tokenizerSetupFactoryType.GetMethod("Create", new[] { typeof(CultureInfo) });

            var setup = _createTokenizerSetup.Invoke(null, new[] { CultureInfo.CurrentCulture });
            _builtinRecognizerProp = setup.GetType().GetField("BuiltinRecognizers");
            var fieldInfo = _builtinRecognizerProp.FieldType.GetFields().First(f => f.Name == "RecognizeAll");
            _builtinRecognizeAllEnumValue = Enum.ToObject(_builtinRecognizerProp.FieldType, fieldInfo.GetRawConstantValue());

            var tokenizerParamsType = assembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.TokenizerParameters");
            _tokenizerParamsCtor = tokenizerParamsType.GetConstructor(new Type[] { setup.GetType(), typeof(IResourceDataAccessor) });

            var tokenizerType = assembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.Tokenizer");
            _tokenizerCtor = tokenizerType.GetConstructor(new Type[] { tokenizerParamsType });
            
            _tokenizerGetTokens = tokenizerType.GetMethod("GetTokens", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(Segment), typeof(bool) }, null);
        }
        public void TokenizeSegment(Segment result, CultureInfo language)
        {
	        // var setup = TokenizerSetupFactory.Create(CultureInfo.CurrentCulture);
	        var setup = _createTokenizerSetup.Invoke(null, new[] { language });

	        // setup.BuiltinRecognizers = BuiltinRecognizers.RecognizeAll;
	        _builtinRecognizerProp.SetValue(setup, _builtinRecognizeAllEnumValue);

	        // setup.CreateWhitespaceTokens = true;
	        setup.GetType().GetField("CreateWhitespaceTokens").SetValue(setup, true);

	        // var tokenizerParameters = new TokenizerParameters(setup, null);
	        var tokenizerParams = _tokenizerParamsCtor.Invoke(new object[] { setup, null });

	        // var tokenizer = new Tokenizer(tokenizerParameters);
	        var tokenizer = _tokenizerCtor.Invoke(new object[] { tokenizerParams });

	        // result.Tokens = tokenizer.GetTokens(segment, true);
	        var tokens = _tokenizerGetTokens.Invoke(tokenizer, new object[] { result, true });
	        result.Tokens = (List<Token>)tokens;
        }

		public Segment CreateTokenizedSegment(string text, CultureInfo language)
        {
            var result = new Segment(language);
            result.Add(text);
            TokenizeSegment(result, language);
            return result;
        }
    }
}
