using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
    public class AlphanumericWithNoBreakSpace
    {
        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public List<string> FindAlphanumericsNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);

            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
            return textAlphanumericsList.Item2;

        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public void CheckIfAlphanumericsAreCorrectNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var alphanumericsList = FindAlphanumericsNoBreakSpace(text, decimalSeparators, thousandSeparators);


            Assert.True(alphanumericsList.Contains("AB14"));
            Assert.True(alphanumericsList.Contains("C12"));
        }

        [Theory]
        [InlineData("There are no alphanumerics here", ",.", ",.")]
        public void CantFindAlphanumericsNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.RequireLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count == 0);
        }

        [Theory]
        [InlineData("!AB14 ab12 #Cd23 12 those aren't  alphanumerics ", ",.", ",.")]
        public void FindAlphanumericsWithSpecialCharactersNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
        {
			var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.RequireLocalization();
			NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
			var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

			var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

			Assert.True(textAlphanumericsList.Item2.Count != 0);
		}

        [Theory]
        [InlineData("AB14 ab12 #Cd23 EF12", ",.", ",.")]
        public void AlphanumericsWithNormalSpaceAndNoBreakSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            FindAlphanumericsNoBreakSpace(text, decimalSeparators, thousandSeparators);
        }
    }
}
