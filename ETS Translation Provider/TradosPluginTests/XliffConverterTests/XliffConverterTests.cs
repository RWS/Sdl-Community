using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TradosPluginTests
{
    [TestClass]
    public class XliffConverterTests
    {
        /// <summary>
        /// Parses an xliff string and verifies that the correct values have been set
        /// </summary>
        [DeploymentItem("SampleXliff.xliff")]
        [TestMethod]
        public void ParseFile_SampleXliff_NoErrors()
        {
            string text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleXliff.xliff"));
            XliffConverter.xliff xliff = XliffConverter.Converter.ParseXliffString(text);

            Assert.AreEqual("ETS", xliff.File.Header.Tools[0].ToolID);
            Assert.AreEqual("SDL ETS", xliff.File.Header.Tools[0].ToolName);
            Assert.AreEqual("5.4", xliff.File.Header.Tools[0].ToolVersion);

            Assert.AreEqual("ar", xliff.File.SourceLanguage);
            Assert.AreEqual("en", xliff.File.TargetLanguage);

            Assert.AreEqual(5, xliff.File.Body.TranslationUnits.Count);

            Assert.AreEqual(1, xliff.File.Body.TranslationUnits[1].ID);
            Assert.AreEqual("Wu aru duuply cancurnud avur thu lasting canflict in Basnia and Hurzugavina.",
                xliff.File.Body.TranslationUnits[1].SourceText);

            XliffConverter.TranslationOption translationOption = xliff.File.Body.TranslationUnits[1].TranslationList.First();
            Assert.AreEqual("ETS", translationOption.ToolID);
            Assert.AreEqual("We Are Deeply Concerned Over The Lasting Conflict In Bosnia And Herzegovina.",
                xliff.File.Body.TranslationUnits[1].TranslationList.First().Translation.Text);

            Assert.AreEqual("en", translationOption.Translation.TargetLanguage);
        }

        /// <summary>
        /// Converts a file from an xliff string to an xliff object, then prints out the ToString equivalent, then
        /// checks that the two xliff strings are equal
        /// </summary>
        [DeploymentItem("SampleXliff.xliff")]
        [TestMethod]
        public void PrintXliff_SampleXliff_SameAsInput()
        {
            string text = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleXliff.xliff"));
            XliffConverter.xliff xliff = XliffConverter.Converter.ParseXliffString(text);
            string printedXliff = XliffConverter.Converter.PrintXliff(xliff);
            Assert.AreEqual(Regex.Replace(text, "\r|\n", ""), Regex.Replace(text, "\r|\n", ""));

        }
    }
}
