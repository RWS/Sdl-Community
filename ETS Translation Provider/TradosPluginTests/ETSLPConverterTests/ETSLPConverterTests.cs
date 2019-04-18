using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradosPluginTests
{
	[TestClass]
    public class ETSLPConverterTests
    {
        /// <summary>
        /// Verifies that we are able to get the CultureInfo object for a common language via ETS Code
        /// </summary>
        [TestMethod]
        public void ETSCodeToCulture_BasicCultures_AllOK()
        {
            Assert.AreEqual(CultureInfo.GetCultureInfo("en"), ETSLPConverter.Converter.ETSCodeToCulture("eng"));
            Assert.AreEqual(CultureInfo.GetCultureInfo("es"), ETSLPConverter.Converter.ETSCodeToCulture("spa"));
            Assert.AreEqual(CultureInfo.GetCultureInfo("fr"), ETSLPConverter.Converter.ETSCodeToCulture("fra"));
            Assert.AreEqual(CultureInfo.GetCultureInfo("de"), ETSLPConverter.Converter.ETSCodeToCulture("ger"));
        }

        /// <summary>
        /// Verifies that we are able to get the CultureInfo object for an uncommon language edge case via ETS Code
        /// </summary>
        [TestMethod]
        public void ETSCodeToCulture_EdgeCaseCultures_AllOK()
        {
            Assert.AreEqual(CultureInfo.GetCultureInfo("nb-no"), ETSLPConverter.Converter.ETSCodeToCulture("nor"));
            Assert.AreEqual(CultureInfo.GetCultureInfo("zh-mo"), ETSLPConverter.Converter.ETSCodeToCulture("cht"));
            Assert.AreEqual(CultureInfo.GetCultureInfo("zh-cn"), ETSLPConverter.Converter.ETSCodeToCulture("chi"));
        }

        /// <summary>
        /// Verifies that if we try to get a CultureInfo object via an ETS code that doesn't exist, it returns
        /// an invalid CultureInfo object
        /// </summary>
        [TestMethod]
        public void ETSCodeToCulture_NoSuchCulture_IsEmpty()
        {
            Assert.AreEqual(CultureInfo.GetCultureInfo(0x007F), ETSLPConverter.Converter.ETSCodeToCulture("abc"));
        }
    }
}