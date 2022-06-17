﻿using System.Collections.Generic;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
	public class AlphanumericWithNormalSpace
    {
        [Theory]
        [InlineData("*BB-1254AE word")]
        public List<string> AlphanumericCustomsSeparators(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
            return textAlphanumericsList.Item2;
        }

        [Theory]
        [InlineData("There are no alphanumerics here")]
        public void CantFindAlphanumericsNormalSpace(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.RequireLocalization();
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count == 0);
        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public void CheckIfAlphanumericsAreCorrectNormalSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var alphanumericsList = FindAlphanumericsNormalSpace(text, decimalSeparators, thousandSeparators);

            Assert.Contains("AB14", alphanumericsList);
            Assert.Contains("C12", alphanumericsList);
        }

        [Theory]
        [InlineData("BB1254AE word")]
        public List<string> FindAlphanumericsLettersCombination(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
            return textAlphanumericsList.Item2;
        }

        [Theory]
        [InlineData("AB14 word C12", ",.", ",.")]
        public List<string> FindAlphanumericsNormalSpace(string text, string decimalSeparators, string thousandSeparators)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.AllowLocalization();
            NumberVerifierLocalizationsSettings.InitSeparators(iNumberSettingsMock);
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
            return textAlphanumericsList.Item2;
        }

        [Theory]
        [InlineData(" !AB14 ab12 #Cd23 those aren't  alphanumerics ")]
        public void FindAlphanumericsWithSpecialCharacters(string text)
        {
            var iNumberSettingsMock = NumberVerifierLocalizationsSettings.RequireLocalization();
            var numberVerifier = new NumberVerifierMain(iNumberSettingsMock.Object);

            var textAlphanumericsList = numberVerifier.GetAlphanumericList(text);

            Assert.True(textAlphanumericsList.Item2.Count != 0);
        }
    }
}