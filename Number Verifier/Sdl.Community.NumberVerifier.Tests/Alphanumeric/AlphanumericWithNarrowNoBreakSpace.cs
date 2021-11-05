using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
	public class AlphanumericWithNarrowNoBreakSpace
    {
        [Theory]
        [InlineData("AB14 ab12 #Cd2 EF12", ",.", ",.")]
        public void AlphanumericsWithNormalSpaceAndNarrowNoBreakSpace(string text, string decimalSeparators,
            string thousandSeparators)
        {
            FindAlphanumericsNarrowNoBreakSpace(text, decimalSeparators, thousandSeparators);
        }

        [Theory]
        [InlineData("There are no alphanumerics here")]
        public void CantFindAlphanumericsNarrowNoBreakSpace(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.RequireLocalization();
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count == 0);
        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public void CheckIfAlphanumericsAreCorrectNarrowNoBreakSpacee(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var alphanumericsList = FindAlphanumericsNarrowNoBreakSpace(text, decimalSeparators, thousandSeparators);

            Assert.Contains("AB14", alphanumericsList);
            Assert.Contains("C12", alphanumericsList);
        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public List<string> FindAlphanumericsNarrowNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
            return textAlphanumericsList.Item2;
        }

        [Theory]
        [InlineData("!AB14 ab12 #Cd23 12 those aren't alphanumerics ")]
        public void FindAlphanumericsWithSpecialCharactersNarrowNoBreakSpace(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.RequireLocalization();
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
        }
    }
}