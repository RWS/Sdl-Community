using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
    public class AlphanumericWithThinSpace
    {
        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public List<string> FindAlphanumericsThinSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Count != 0);
            return textAlphanumericsList;

        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public void CheckIfAlphanumericsAreCorrectThinSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var alphanumericsList = FindAlphanumericsThinSpace(text, decimalSeparators, thousandSeparators);


            Assert.True(alphanumericsList.Contains("AB14"));
            Assert.True(alphanumericsList.Contains("C12"));
        }

        [Theory]
        [InlineData("There are no alphanumerics here", ",.", ",.")]
        public void CantFindAlphanumericsThinSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.RequireLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Count == 0);
        }

        [Theory]
        [InlineData("!AB14 ab12 #Cd23 12 those aren't alphanumerics ", ",.", ",.")]
        public void CantFindAlphanumericsWithSpecialCharactersThinSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            CantFindAlphanumericsThinSpace(text, decimalSeparators, thousandSeparators);
        }

        [Theory]
        [InlineData("AB14 ab12 #Cd23 EF12", ",.", ",.")]
        public void AlphanumericsWithNormalSpaceAndThinkSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            FindAlphanumericsThinSpace(text, decimalSeparators, thousandSeparators);
        }
    }
}
