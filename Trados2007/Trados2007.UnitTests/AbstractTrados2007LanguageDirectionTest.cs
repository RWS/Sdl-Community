namespace Trados2007.UnitTests
{
    using System.Globalization;

    using Rhino.Mocks.Interfaces;

    using Sdl.LanguagePlatform.Core;
    using Sdl.TranslationStudio.Plugins.Trados2007;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Sdl.LanguagePlatform.TranslationMemory;
    using Rhino.Mocks;

    using Action = Sdl.LanguagePlatform.TranslationMemory.Action;

    [TestClass()]
    public class AbstractTrados2007LanguageDirectionTest
    {
        internal virtual AbstractTrados2007LanguageDirection CreateAbstractTrados2007LanguageDirection()
        {
            var provider = MockRepository.GenerateStub<ITrados2007TranslationProvider>();
            provider.Stub(_ => _.LanguageDirection).Return(new LanguagePair("en-US", "de-DE"));

            var target = MockRepository.GenerateStub<AbstractTrados2007LanguageDirection>(provider);
            target.Stub(_ => _.AddOrUpdateTranslationUnits(null, null, null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            target.Stub(_ => _.AddOrUpdateTranslationUnitsMasked(null, null, null, null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            target.Stub(_ => _.AddTranslationUnit(null, null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            target.Stub(_ => _.AddTranslationUnits(null, null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            target.Stub(_ => _.AddTranslationUnitsMasked(null, null, null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            target.Stub(_ => _.UpdateTranslationUnit(null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);
            target.Stub(_ => _.UpdateTranslationUnits(null)).CallOriginalMethod(OriginalCallOptions.NoExpectation);

            return target;
        }

        [TestMethod()]
        public void AbstractTrados2007LanguageDirection_Properties_OK()
        {
            var target = this.CreateAbstractTrados2007LanguageDirection();

            Assert.IsFalse(target.CanReverseLanguageDirection);
            Assert.AreEqual(CultureInfo.GetCultureInfo("en-US"), target.SourceLanguage);
            Assert.AreEqual(CultureInfo.GetCultureInfo("de-DE"), target.TargetLanguage);
        }

        [TestMethod()]
        public void AddOrUpdateTranslationUnitsTest()
        {
            var target = CreateAbstractTrados2007LanguageDirection();
            TranslationUnit[] units = new[] { new TranslationUnit(), new TranslationUnit() };
            int[] hashes = new[] { 1201, 121212 };
            var settigns = new ImportSettings { CheckMatchingSublanguages = true };
            var expected = new[] { new ImportResult { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation } ,
            new ImportResult { Action = Action.Add, ErrorCode = ErrorCode.InvalidOperation }};

            var actual = target.AddOrUpdateTranslationUnits(units, hashes, settigns);

            Assert.AreEqual(expected.Length, actual.Length);
            Assert.AreEqual(expected[0].ErrorCode, actual[0].ErrorCode);
            Assert.AreEqual(expected[1].Action, actual[1].Action);
        }
    }
}
