using System.Globalization;
using ETSLPConverter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TradosPluginTests
{
	[TestClass]
    public class ETSLPConverterExtensionsTests
    {
        /// <summary>
        /// Verifies that we are able to convert from CultureInfo objects to ETS codes for common cultures
        /// </summary>
        [TestMethod]
        public void ToETSCode_BasicCultures_AllOK()
        {
            Assert.AreEqual("eng", CultureInfo.GetCultureInfo("en").ToETSCode());
            Assert.AreEqual("spa", CultureInfo.GetCultureInfo("es-ES").ToETSCode());
            Assert.AreEqual("ger", CultureInfo.GetCultureInfo("de-de").ToETSCode());
            Assert.AreEqual("jpn", CultureInfo.GetCultureInfo("ja").ToETSCode());
        }

        /// <summary>
        /// Verifies that we are able to convert from CultureInfo objects to ETS codes for unusual culture edge cases
        /// </summary>
        [TestMethod]
        public void ToETSCode_EdgeCaseCultures_AllOK()
        {
            Assert.AreEqual("nor", CultureInfo.GetCultureInfo("nb-no").ToETSCode());
            Assert.AreEqual("nor", CultureInfo.GetCultureInfo("nn-no").ToETSCode());
            
			// Depending on the version of windows, it may not support nb-sj, so if a CultureNotFoundException is
            // thrown, there's little we can do about it.
            try
            {
                Assert.AreEqual("nor", CultureInfo.GetCultureInfo("nb-sj").ToETSCode());
            }
            catch (CultureNotFoundException)
            { }

            Assert.AreEqual("cht", CultureInfo.GetCultureInfo("zh-mo").ToETSCode());
            Assert.AreEqual("cht", CultureInfo.GetCultureInfo("zh-hk").ToETSCode());
            Assert.AreEqual("cht", CultureInfo.GetCultureInfo("zh-tw").ToETSCode());

            Assert.AreEqual("chi", CultureInfo.GetCultureInfo("zh-cn").ToETSCode());
            Assert.AreEqual("chi", CultureInfo.GetCultureInfo("zh-sg").ToETSCode());
        }

        /// <summary>
        /// Verifies that if we try to get the ETS code of a culture that doesn't exist (or isn't supported by ETS),
        /// it returns an empty string.
        /// </summary>
        [TestMethod]
        public void ToETSCode_NoSuchCulture_IsEmpty()
        {
            Assert.AreEqual(string.Empty, CultureInfo.GetCultureInfo(0x007F).ToETSCode());
        }
    }
}