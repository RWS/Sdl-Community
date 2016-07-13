using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
    public class AlpahanumericWithNormalSpace
    {

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public List<string> FindAlphanumericsNormalSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.AllowLocalization();
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Count != 0);
            return textAlphanumericsList;

        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public void CheckIfAlphanumericsAreCorrectNormalSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var alphanumericsList = FindAlphanumericsNormalSpace(text, decimalSeparators, thousandSeparators);


            Assert.True(alphanumericsList.Contains("AB14"));
            Assert.True(alphanumericsList.Contains("C12"));
        }

        [Theory]
        [InlineData("There are no alphanumerics here", ",.", ",.")]
        public void CantFindAlphanumericsNormalSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = Utilities.NumberVerifierLocalizationsSettings.RequireLocalization();
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Count == 0);
        }

        [Theory]
        [InlineData(" !AB14 ab12 #Cd23 those aren't  alphanumerics ", ",.", ",.")]
        public void CantFindAlphanumericsWithSpecialCharacters(string text, string decimalSeparators, string thousandSeparators)
        {
            CantFindAlphanumericsNormalSpace(text, decimalSeparators, thousandSeparators);
        }

    }
}
