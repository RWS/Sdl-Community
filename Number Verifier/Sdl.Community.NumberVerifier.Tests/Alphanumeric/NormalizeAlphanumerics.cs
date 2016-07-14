using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.Alphanumeric
{
    public class NormalizeAlphanumerics
    {
        
        public List<ErrorReporting> ReportModifiedAlphanumerics(string source, string target,NumberVerifierMain numberVerifierMain)
        {     
            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            return numberVerifierMain.CheckSourceAndTarget(source, target);

           
        }

        [Theory]
        [InlineData("This is an alphanumeric CDE45", "Wrong alphanumeric CE45")]
        public void WrongAlphanumeric(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target,numberVerifierMain);
            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_AlphanumericsModified);
        }

        [Theory]
        [InlineData("12AC and CDE45", "12AB Wrong alphanumeric CE45")]
        public void WrongAlphanumerics(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessages = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            foreach (var errorMessage in errorMessages)
            {
                Assert.Equal(errorMessage.ErrorMessage, PluginResources.Error_AlphanumericsModified);
            }
        }

        /// <summary>
        /// This are not aplhanumerics, number verifier should check only the 
        /// numbers in this case
        /// No error message
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("mAB23", "mCD23")]
        public void CheckIfIsAnAlphanumeric(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessages = ReportModifiedAlphanumerics(source, target, numberVerifierMain);

            Assert.True(errorMessages.Count==0);
        }

        /// <summary>
        /// Error number modified
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("mAB23", "mCD24")]
        public void ModifiedNumberError(string source, string target)
        {
            //target settings
            var numberVerifierSettings = NumberVerifierRequireLocalizationSettings.CommaPeriod();
            numberVerifierSettings.Setup(d => d.TargetDecimalComma).Returns(true);

            //source settings
            numberVerifierSettings.Setup(s => s.SourceThousandsSpace).Returns(true);
            numberVerifierSettings.Setup(s => s.SourceDecimalComma).Returns(true);

            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessages = ReportModifiedAlphanumerics(source, target, numberVerifierMain);

            Assert.Equal(errorMessages[0].ErrorMessage,PluginResources.Error_NumbersNotIdentical);
        }

        /// <summary>
        /// Alphanumerics which are items in a list
        /// source and target normal -
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        [Theory]
        [InlineData("some text -AB23", "another text -AC23")]
        public void AlphanumericsInList(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_AlphanumericsModified);
        }


        [Theory]
        [InlineData("some word -AB23", "another text -AB23")]
        public void AlphanumericsInListSpecialSpace(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count==0);
        }

        ///// <summary>
        ///// Alphanumerics which are items in a list
        ///// source normal - , target special −
        ///// see http://www.fileformat.info/info/unicode/char/2212/browsertest.htm
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="target"></param>
        [Theory]
        [InlineData("some text -AB23", "another text −AC23")]
        public void AlphanumericsInListDifferentLineReportsModified(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.Equal(errorMessage[0].ErrorMessage, PluginResources.Error_AlphanumericsModified);
        }

        [Theory]
        [InlineData("some text -AB23", "another text −AB23")]
        public void SameAlphanumericsInListDifferentMinusNoError(string source, string target)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.CommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            var errorMessage = ReportModifiedAlphanumerics(source, target, numberVerifierMain);
            Assert.True(errorMessage.Count==0);
        }
    }
}
