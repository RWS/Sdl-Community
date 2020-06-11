using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sdl.Community.MTEdge.UnitTests.SDLMTEdgeLPConverterTests
{
	[TestClass]
	public class SDLMTEdgeLPConverterTests
	{
		/// <summary>
		/// Verifies that we are able to get the CultureInfo object for a common language via MTEdge Code
		/// </summary>
		[TestMethod]
		public void MTEdgeCodeToCulture_BasicCultures_AllOK()
		{
			Assert.AreEqual(CultureInfo.GetCultureInfo("en"), LPConverter.Converter.MTEdgeCodeToCulture("eng"));
			Assert.AreEqual(CultureInfo.GetCultureInfo("es"), LPConverter.Converter.MTEdgeCodeToCulture("spa"));
			Assert.AreEqual(CultureInfo.GetCultureInfo("fr"), LPConverter.Converter.MTEdgeCodeToCulture("fra"));
			Assert.AreEqual(CultureInfo.GetCultureInfo("de"), LPConverter.Converter.MTEdgeCodeToCulture("ger"));
		}

		/// <summary>
		/// Verifies that we are able to get the CultureInfo object for an uncommon language edge case via MTEdge Code
		/// </summary>
		[TestMethod]
		public void MTEdgeCodeToCulture_EdgeCaseCultures_AllOK()
		{
			Assert.AreEqual(CultureInfo.GetCultureInfo("nb-no"), LPConverter.Converter.MTEdgeCodeToCulture("nor"));
			Assert.AreEqual(CultureInfo.GetCultureInfo("zh-mo"), LPConverter.Converter.MTEdgeCodeToCulture("cht"));
			Assert.AreEqual(CultureInfo.GetCultureInfo("zh-cn"), LPConverter.Converter.MTEdgeCodeToCulture("chi"));
		}

		/// <summary>
		/// Verifies that if we try to get a CultureInfo object via an MTEdge code that doesn't exist, it returns
		/// an invalid CultureInfo object
		/// </summary>
		[TestMethod]
		public void MTEdgeCodeToCulture_NoSuchCulture_IsEmpty()
		{
			Assert.AreEqual(CultureInfo.GetCultureInfo(0x007F), LPConverter.Converter.MTEdgeCodeToCulture("abc"));
		}
	}
}