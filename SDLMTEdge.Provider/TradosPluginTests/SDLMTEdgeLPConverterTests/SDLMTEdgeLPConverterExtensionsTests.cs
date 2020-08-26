using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sdl.Community.MTEdge.LPConverter;

namespace Sdl.Community.MTEdge.UnitTests.SDLMTEdgeLPConverterTests
{
	[TestClass]
    public class SDLMTEdgeLPConverterExtensionsTests
	{
        /// <summary>
        /// Verifies that we are able to convert from CultureInfo objects to MTEdge codes for common cultures
        /// </summary>
        [TestMethod]
        public void ToMTEdgeCode_BasicCultures_AllOK()
        {
            Assert.AreEqual("eng", CultureInfo.GetCultureInfo("en").ToMTEdgeCode());
            Assert.AreEqual("spa", CultureInfo.GetCultureInfo("es-ES").ToMTEdgeCode());
            Assert.AreEqual("ger", CultureInfo.GetCultureInfo("de-de").ToMTEdgeCode());
            Assert.AreEqual("jpn", CultureInfo.GetCultureInfo("ja").ToMTEdgeCode());
        }

        /// <summary>
        /// Verifies that we are able to convert from CultureInfo objects to ETS codes for unusual culture edge cases
        /// </summary>
        [TestMethod]
        public void ToMTEdgeCode_EdgeCaseCultures_AllOK()
        {
            Assert.AreEqual("nor", CultureInfo.GetCultureInfo("nb-no").ToMTEdgeCode());
            Assert.AreEqual("nor", CultureInfo.GetCultureInfo("nn-no").ToMTEdgeCode());
            
			// Depending on the version of windows, it may not support nb-sj, so if a CultureNotFoundException is
            // thrown, there's little we can do about it.
            try
            {
                Assert.AreEqual("nor", CultureInfo.GetCultureInfo("nb-sj").ToMTEdgeCode());
            }
            catch (CultureNotFoundException)
            { }

            Assert.AreEqual("cht", CultureInfo.GetCultureInfo("zh-mo").ToMTEdgeCode());
            Assert.AreEqual("cht", CultureInfo.GetCultureInfo("zh-hk").ToMTEdgeCode());
            Assert.AreEqual("cht", CultureInfo.GetCultureInfo("zh-tw").ToMTEdgeCode());

            Assert.AreEqual("chi", CultureInfo.GetCultureInfo("zh-cn").ToMTEdgeCode());
            Assert.AreEqual("chi", CultureInfo.GetCultureInfo("zh-sg").ToMTEdgeCode());
        }

		/// <summary>
		/// Verifies that if we try to get the MTEdge code of a culture that doesn't exist (or isn't supported by MTEdge),
		/// it returns an empty string.
		/// </summary>
		[TestMethod]
        public void ToMTEdgeCode_NoSuchCulture_IsEmpty()
        {
            Assert.AreEqual(string.Empty, CultureInfo.GetCultureInfo(0x007F).ToMTEdgeCode());
        }
    }
}