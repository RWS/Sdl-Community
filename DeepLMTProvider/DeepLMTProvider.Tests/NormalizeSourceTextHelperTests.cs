using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.DeepLMTProvider;
using Xunit;

namespace DeepLMTProvider.Tests
{
	public class NormalizeSourceTextHelperTests
	{
		[Theory]
		[InlineData(true, "Document%20Description", "Document\tDescription")]
		[InlineData(false, "Document%09Description", "Document\tDescription")]
		[InlineData(null, "", "")]
		[InlineData(null, "Document%09Description", "Document\tDescription")]
		public void NormalizeText(bool removeTabs, string expected, string toBeNormalized)
		{
			// Arrange
			var normalizeSourceTextHelper = new NormalizeSourceTextHelper();
			var expectedText = expected;

			// Act
			var actualText = normalizeSourceTextHelper.NormalizeText(toBeNormalized, removeTabs);

			// Assert
			Assert.Equal(expectedText, actualText);
		}
	}
}
