using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.Core.EditDistance;
using Sdl.LanguagePlatform.Core.Tokenization;

namespace TMX_Lib.TokenizeUtil
{
    /*
     * the equivalent of
     *
        bool computeDiagonalOnly = false;
        TagAssociations alignedTags;
        bool charactersNormalizeSafely = true;
        bool applySmallChangeAdjustment = true;
        bool diagonalOnly = false;
        var computer = new SegmentEditDistanceComputer();
        var distance = computer.ComputeEditDistance(tokensSource, tokensTarget, computeDiagonalOnly, BuiltinRecognizers.RecognizeNone,
            out alignedTags, charactersNormalizeSafely, applySmallChangeAdjustment, diagonalOnly);
     *
     */
    public class ComputeEditDistance
    {
        // If number of tokens is equivalent, only the diagonal's similarities are computed.
        public bool computeDiagonalOnly = false;

        //Set to false if the language is not considered 'char-based' like Chinese and Japanese are 
        // (i.e. uses space as separator), but does consist of complex characters producing strings of more than 1 significant char 
        // when Unicode-normalised (NormalizationForm.FormD)
        public bool charactersNormalizeSafely = true;

        public bool applySmallChangeAdjustment = true;

        // Unlike <paramref name="computeDiagonalOnly"/>, should only be set true if number of tokens is equivalent, and if 
        // the only edit operations of interest are 'identity' and 'change'. This provides a fast way to score segments with matching
        // identity strings and feature-token placement.
        public bool diagonalOnly = false;

        private TokenizeText _tokenizer = new TokenizeText();

        public EditDistance Compute(string originalText, string resultingText)
        {
			var ignore = CultureInfo.CurrentCulture;
            var original = _tokenizer.CreateTokenizedSegment(originalText, ignore);
            var resulting = _tokenizer.CreateTokenizedSegment(resultingText, ignore);
            return Compute(original.Tokens, resulting.Tokens);
        }
        private EditDistance Compute(List<Token> tokensSource, List<Token> tokensTarget)
        {
            var tokenType = typeof(Token);
            var iListTokenType = typeof(IList<>).MakeGenericType(new Type[] { tokenType });

            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            var assemblyLingua = Assembly.LoadFile($"{programFiles}\\Trados\\Trados Studio\\Studio17\\Sdl.LanguagePlatform.Lingua.dll");
            var computerType = assemblyLingua.GetType("Sdl.LanguagePlatform.Lingua.SegmentEditDistanceComputer");
            var computerCtor = computerType.GetConstructor(new Type[] { });
            var computer = computerCtor.Invoke(new object[] { });

            var tagAssociationType = assemblyLingua.GetType("Sdl.LanguagePlatform.Lingua.TagAssociations");
            var tagAssociationTypeRef = tagAssociationType.MakeByRefType();

            var builtinRecognizersType = typeof(BuiltinRecognizers);
            var computeMethod = computerType.GetMethod("ComputeEditDistance", new Type[]
            {
                iListTokenType, iListTokenType, typeof(bool), builtinRecognizersType, tagAssociationTypeRef,
                typeof(bool), typeof(bool), typeof(bool),
            });
            var distance = computeMethod.Invoke(computer, new object[]
            {
                tokensSource, tokensTarget,
                computeDiagonalOnly,
                BuiltinRecognizers.RecognizeNone,
                null, charactersNormalizeSafely, applySmallChangeAdjustment, diagonalOnly,
            });

            return (EditDistance)distance;
        }
    }
}
