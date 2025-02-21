using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
	public class AlphanumericWithThinSpace
    {
        [Theory]
        [InlineData("AB14 ab12 #Cd23 EF12", ",.", ",.")]
        public void AlphanumericsWithNormalSpaceAndThinkSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            FindAlphanumericsThinSpace(text, decimalSeparators, thousandSeparators);
        }

        [Theory]
        [InlineData("There are no alphanumerics here")]
        public void CantFindAlphanumericsThinSpace(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.RequireLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count == 0);
        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public void CheckIfAlphanumericsAreCorrectThinSpace(string text, string decimalSeparators,
            string thousandSeparators)
        {
            var alphanumericsList = FindAlphanumericsThinSpace(text, decimalSeparators, thousandSeparators);

            Assert.Contains("AB14", alphanumericsList);
            Assert.Contains("C12", alphanumericsList);
        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public List<string> FindAlphanumericsThinSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
            return textAlphanumericsList.Item2;
        }

        [Theory]
        [InlineData("!AB14 ab12 #Cd23 12 those aren't alphanumerics ")]
        public void FindFindAlphanumericsWithSpecialCharactersThinSpace(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.RequireLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
        }
    }
}