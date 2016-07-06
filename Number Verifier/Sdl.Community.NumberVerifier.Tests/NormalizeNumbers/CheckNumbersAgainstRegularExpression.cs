using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
    public class CheckNumbersAgainstRegularExpression
    {
        ///// <summary>
        ///// We didn't selected the space, so the method will split the number
        ///// The error message will be "Number modified/unlocalized"
        ///// </summary>
        ///// <param name="text"></param>
        ///// <param name="thousandSep"></param>
        ///// <param name="decimalSep"></param>
        ///// <param name="noSeparator"></param>
        //[Theory]
        //[InlineData("1 245.67", ".,", ",", false)]
        //public void NormalizeNumberWithCommaAndPeriodChecked(string text, string thousandSep, string decimalSep, bool noSeparator)
        //{
        //    var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
        //    numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

        //    var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

        //    //run initialize method in order to set chosen separators
        //    var docPropMock = new Mock<IDocumentProperties>();
        //    numberVerifierMain.Initialize(docPropMock.Object);

        //    var normalizedNumberCollection = new List<string>();
        //    var numberCollection = new List<string>();

        //    var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
        //    var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);
        //    numberVerifierMain.NormalizeAlphanumerics(text,numberCollection,normalizedNumberCollection, thousandSeparators, decimalSeparators,false,false);

        //    Assert.Equal("1",normalizedNumberCollection[0]);
        //    Assert.Equal("245d67", normalizedNumberCollection[1]);
        //}

        /// <summary>
        /// In case of -(space)number the dash should be ignored because is not a negative number  
        /// is a item in a list
        /// </summary>
        /// <param name="text"></param>
        /// <param name="thousandSep"></param>
        /// <param name="decimalSep"></param>
        /// <param name="noSeparator"></param>
        [Theory]
        [InlineData("- 34", ".,", ",", false)]
        public void SkippTheDash(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var normalizedNumberCollection = new List<string>();
            var numberCollection = new List<string>();

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);
            numberVerifierMain.NormalizeAlphanumerics(text, numberCollection, normalizedNumberCollection, thousandSeparators, decimalSeparators, false, false);

            Assert.Equal("34",numberCollection[0]);
            Assert.Equal("34", normalizedNumberCollection[0]);
            
        }

        [Theory]
        [InlineData("−74,5", ".,", ",", false)]
        public void NormalizeNegativeNumbers(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var normalizedNumberCollection = new List<string>();
            var numberCollection = new List<string>();

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);
            numberVerifierMain.NormalizeAlphanumerics(text, numberCollection, normalizedNumberCollection, thousandSeparators, decimalSeparators, false, false);

            Assert.Equal("−74,5",numberCollection[0]);
            Assert.Equal("m74d5", normalizedNumberCollection[0]);

        }

        [Theory]
        [InlineData("This ab46 is not an alphanumeric, the plugin will recognize only the number", ".,", ",", false)]
        public void FindNumbersWithinTheWords(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var normalizedNumberCollection = new List<string>();
            var numberCollection = new List<string>();

            var thousandSeparators = numberVerifierMain.AddCustomSeparators(thousandSep,true);
            var decimalSeparators = numberVerifierMain.AddCustomSeparators(decimalSep,true);
            numberVerifierMain.NormalizeAlphanumerics(text, numberCollection, normalizedNumberCollection, thousandSeparators, decimalSeparators, false, false);

            Assert.Equal("46", numberCollection[0]);
            Assert.Equal("46", normalizedNumberCollection[0]);

        }
    }
}
